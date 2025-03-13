using System.Text;
using GIGANTECLIENTCORE.Context;
using Serilog;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using System.IO;
using DotEnv.Core;
using GIGANTECLIENTCORE.Utils;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Google.Cloud.Storage.V1;
using Microsoft.Data.SqlClient;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

new EnvLoader()
    .AddEnvFile("development.env")
    .Load();

var config = 
    new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

//Logs Configuración
Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo
    .File("logs/GiganteClienteCoreLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Host.UseSerilog();

// Google Cloud SQL Connection String Builder for PostgreSQL
string BuildGoogleCloudPostgreSqlConnectionString()
{
    var instanceConnectionName = Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME");
    var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "";
    var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    
    // For development local with Cloud SQL Proxy
    return $"Host=127.0.0.1;Port=5432;Database={dbName};Username={dbUser};Password={dbPass};";
}

//Configuracion de la base de datos 
// Use Google Cloud SQL connection if environment variables are set
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME")))
{
    builder.Services.AddDbContext<MyDbContext>(options =>
        options.UseNpgsql(BuildGoogleCloudPostgreSqlConnectionString()));
    
    Log.Information("Using Google Cloud PostgreSQL connection");
}
else
{
    // Fall back to the standard PostgreSQL connection
    builder.Services.AddDbContext<MyDbContext>(options =>
        options.UseNpgsql(Environment.GetEnvironmentVariable("DATA_BASE_CONNECTION_STRING")));
    
    Log.Information("Using standard PostgreSQL connection");
}

//Controlador Servicios
builder.Services.AddControllers(option => option.ReturnHttpNotAcceptable = true)
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
    .AddXmlDataContractSerializerFormatters();

//Configuracion JWT
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;  // Asegúrate de que RequireHttpsMetadata esté configurado correctamente
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")))
        }; 
    });

//Configuracion de politicas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireClienteRole", policy => policy.RequireRole("Cliente"));
});

//Configuracion Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GIGANTECLIENTE API", Version = "v1" });

    // Configuración para JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//Configuración Cors para poder hacerlo con ReactJS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:5173") // URL de tu app React
            .AllowAnyMethod()     // Permite todos los métodos HTTP (GET, POST, PUT, DELETE, etc.)
            .AllowAnyHeader()
            .AllowCredentials());
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // 10MB
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GIGANTE CLIENTES API v1");
    c.ConfigObject.DisplayRequestDuration = true;
    c.RoutePrefix = "swagger"; // Esto es importante
});

app.UseRouting();
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
//Usamos la autenticación antes de la autorización  
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var exceptionHandlerFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            var exception = exceptionHandlerFeature.Error;
            Log.Error(exception, "Error no manejado");
            
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new 
            {
                error = "Se produjo un error interno",
                detail = app.Environment.IsDevelopment() ? exception.ToString() : null
            }));
        }
    });
});

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

// Add a diagnostic endpoint for Google Cloud SQL connection
app.MapGet("/api/diagnostico/cloudsqltest", async () => 
{
    try {
        string connectionString = "";
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME")))
        {
            connectionString = BuildGoogleCloudPostgreSqlConnectionString();
        }
        else
        {
            connectionString = Environment.GetEnvironmentVariable("DATA_BASE_CONNECTION_STRING");
        }
        
        var result = "No se intentó la conexión";
        
        if (!string.IsNullOrEmpty(connectionString))
        {
            try {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    result = "Conexión exitosa";
                    
                    // PostgreSQL version query
                    using (var command = new NpgsqlCommand("SELECT version()", connection))
                    {
                        var version = await command.ExecuteScalarAsync();
                        result += $" - Versión: {version}";
                    }
                }
            }
            catch (Exception ex) {
                result = $"Error: {ex.Message}";
                if (ex.InnerException != null) {
                    result += $" | Inner: {ex.InnerException.Message}";
                }
            }
        }
        
        return Results.Ok(new { 
            TestResult = result,
            IsCloudSql = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME"))
        });
    }
    catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});

app.MapGet("/api/diagnostico/sqltest", async () => 
{
    try {
        string connectionString = "";
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME")))
        {
            connectionString = BuildGoogleCloudPostgreSqlConnectionString();
        }
        else
        {
            connectionString = Environment.GetEnvironmentVariable("DATA_BASE_CONNECTION_STRING");
        }
        
        var result = "No se intentó la conexión";
        
        if (!string.IsNullOrEmpty(connectionString))
        {
            try {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    result = "Conexión exitosa";
                    
                    // PostgreSQL version query
                    using (var command = new NpgsqlCommand("SELECT version()", connection))
                    {
                        var version = await command.ExecuteScalarAsync();
                        result += $" - Versión: {version}";
                    }
                }
            }
            catch (Exception ex) {
                result = $"Error: {ex.Message}";
                if (ex.InnerException != null) {
                    result += $" | Inner: {ex.InnerException.Message}";
                }
            }
        }
        
        return Results.Ok(new { 
            TestResult = result,
            IsCloudSql = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("INSTANCE_CONNECTION_NAME"))
        });
    }
    catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});

app.MapGet("/api/diagnostico/external-ip", async () => 
{
    try {
        string externalIp = "No se pudo determinar";
        
        try {
            using (var httpClient = new HttpClient())
            {
                externalIp = await httpClient.GetStringAsync("https://api.ipify.org");
            }
        }
        catch (Exception ex) {
            externalIp = $"Error: {ex.Message}";
        }
        
        return Results.Ok(new { 
            ExternalIp = externalIp
        });
    }
    catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});

app.Run();