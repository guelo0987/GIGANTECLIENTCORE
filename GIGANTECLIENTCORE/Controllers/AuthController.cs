using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.DTO;
using GIGANTECLIENTCORE.Models;
using GIGANTECLIENTCORE.Utils;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LoginRequest = GIGANTECLIENTCORE.DTO.LoginRequest;
using Octetus.ConsultasDgii;


namespace GIGANTECLIENTCORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, MyDbContext db, IConfiguration configuration)
        {
            _db = db;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var client = _db.UsuarioClientes
                .Include(o => o.Rol)
                .FirstOrDefault(o => o.Email == loginRequest.Mail);

            if (client == null)
            {
                _logger.LogError("Usuario no encontrado");
                return NotFound("Usuario no Encontrado");
            }

            if (loginRequest.Password != client.Password)
            {
                _logger.LogError("Contraseña invalida");
                return Unauthorized("Credenciales Invalidas");
            }

            if (client.Rol.Name != "Cliente")
            {
                _logger.LogError("Rol no valido");
                return Unauthorized("Rol no valido");
            }

            // Crear los claims para el token JWT
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", client.Id.ToString()),
                new Claim(ClaimTypes.Role, client.Rol.Name)
            };

            _logger.LogInformation("Login Exitoso");

            return GenerateToken(claims, new UserClienteDTO
            {
                Id = client.Id,
                UserName = client.UserName,
                Email = client.Email,
                Direccion = client.Direccion,
                Ciudad = client.Ciudad,
                Apellidos = client.Apellidos,
                Telefono = client.Telefono,
                Rnc = client.Rnc,
                Dob = client.Dob,
                FechaIngreso = client.FechaIngreso,
                RolId = client.RolId
            }, client.Rol.Name);
        }

       [HttpPost("register")]
public IActionResult Register([FromBody] RegisterDTO registerRequest)
{
    // Verificar si el correo ya está registrado
    if (_db.UsuarioClientes.Any(u => u.Email == registerRequest.Email))
    {
        _logger.LogError("El correo ya está registrado");
        return Conflict("El correo ya está registrado");
    }

    // Obtener el rol "Cliente" de la base de datos
    var rol = _db.Roles.FirstOrDefault(r => r.Name == "Cliente");
    if (rol == null)
    {
        _logger.LogError("Rol 'Cliente' no encontrado en la base de datos");
        return StatusCode(500, "Error en la configuración del servidor");
    }
    
    // Crear la entidad de usuario a partir de los datos del registro
    var usuario = new UsuarioCliente
    {
        UserName = registerRequest.UserName,
        Email = registerRequest.Email,
        Password = registerRequest.Password, // **Nota:** En producción, encripta la contraseña.
        Direccion = registerRequest.Direccion,
        Ciudad = registerRequest.Ciudad,
        Apellidos = registerRequest.Apellidos,
        Telefono = registerRequest.Telefono,
        Rnc = registerRequest.Rnc,
        Dob = registerRequest.Dob,
        FechaIngreso = DateTime.UtcNow,
        RolId = rol.IdRol,
        Rol = rol
    };

    // Si se envía el RNC, consultar la API de la DGII para obtener información de la compañía
    if (!string.IsNullOrWhiteSpace(registerRequest.Rnc))
    {
        var dgiService = new DgiiService();
        string nombre = dgiService.ConsultarContribuyente(registerRequest.Rnc);

        var comp = new Compañium
        {
            Rnc = registerRequest.Rnc,
            Name = nombre
        };

        _db.Compañia.Add(comp);
        _db.SaveChanges();

        
        
    }
    _db.UsuarioClientes.Add(usuario);
    _db.SaveChanges();

    // Crear los claims para el token JWT
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("Id", usuario.Id.ToString()),
        new Claim(ClaimTypes.Role, rol.Name)
    };

    // Crear el DTO para la respuesta
    var userClienteDto = new UserClienteDTO
    {
        Id = usuario.Id,
        UserName = usuario.UserName,
        Email = usuario.Email,
        Direccion = usuario.Direccion,
        Ciudad = usuario.Ciudad,
        Apellidos = usuario.Apellidos,
        Telefono = usuario.Telefono,
        Rnc = usuario.Rnc,
        Dob = usuario.Dob,
        FechaIngreso = usuario.FechaIngreso,
        RolId = usuario.RolId
    };

    _logger.LogInformation("Registro exitoso para el usuario: {Email}", usuario.Email);

    // Generar y retornar el token (permite iniciar sesión automáticamente después del registro)
    return GenerateToken(claims, userClienteDto, rol.Name);
}

        // Método privado para generar el Token JWT
        private IActionResult GenerateToken(Claim[] claims, UserClienteDTO userClienteDto, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signIn
            );

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("Operación exitosa de autenticación");

            return Ok(new { Token = tokenValue, User = userClienteDto, Role = role });
        }
    }
}
