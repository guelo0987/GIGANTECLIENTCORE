using GIGANTECLIENTCORE.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECLIENTCORE.Controllers;




[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly ILogger<ProductoController> _logger;
    
    public ProductoController(MyDbContext db, ILogger<ProductoController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProductos()
    {
        var products = await _db.Productos
            .Include(o => o.Categoria)
            .ThenInclude(o => o.SubCategoria)
            .Where(p => !string.IsNullOrEmpty(p.ImageUrl))
            .ToListAsync();
        _logger.LogInformation("Obteniendo Productos con imágenes");
        return Ok(products);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> ProductoId(int id)
    {
        var products = await _db.Productos
            .Include(o => o.Categoria)
            .ThenInclude(o => o.SubCategoria)
            .FirstOrDefaultAsync(u => u.Codigo == id && !string.IsNullOrEmpty(u.ImageUrl));

        if (products == null)
        {
            _logger.LogError("Producto no Encontrado o sin imagen.");
            return NotFound("Producto no encontrado o sin imagen.");
        }

        return Ok(products);
    }
    
    //Productos por categoria 
    [HttpGet("porcategoria/{categoriaId}")]
    public async Task<IActionResult> GetProductosByCategoria(int categoriaId)
    {
        var productos = await _db.Productos
            .Include(s => s.Categoria)
            .Where(s => s.CategoriaId == categoriaId && !string.IsNullOrEmpty(s.ImageUrl))
            .ToListAsync();
    
        if (!productos.Any())
        {
            _logger.LogInformation($"No se encontraron Productos con imágenes para la categoría con Id {categoriaId}");
            return NotFound($"No se encontraron Productos con imágenes para la categoría con Id {categoriaId}.");
        }
    
        return Ok(productos);
    }
    
    
    //Productos por SubCategoria 
    [HttpGet("porsubcategoria/{categoriaId}/{subcategoriaId}")]
    public async Task<IActionResult> GetProductosBySubCategoria(int categoriaId, int subcategoriaId)
    {
        var productos = await _db.Productos
            .Include(s => s.Categoria)
            .ThenInclude(o => o.SubCategoria)
            .Where(s => s.CategoriaId == categoriaId && 
                       s.SubCategoriaId == subcategoriaId && 
                       !string.IsNullOrEmpty(s.ImageUrl))
            .ToListAsync();
    
        if (!productos.Any())
        {
            _logger.LogInformation($"No se encontraron Productos con imágenes para la subcategoría con Id {subcategoriaId}");
            return NotFound($"No se encontraron Productos con imágenes para la subcategoría con Id {subcategoriaId}.");
        }
    
        return Ok(productos);
    }
    
    //Productos por Marca 
    [HttpGet("pormarca")]
    public async Task<IActionResult> GetProductosByMarca(string marca)
    {
        var productos = await _db.Productos
            .Include(s => s.Categoria)
            .Include(s => s.SubCategoria)
            .Where(s => s.Marca == marca.ToLower() && !string.IsNullOrEmpty(s.ImageUrl))
            .ToListAsync();
    
        if (!productos.Any())
        {
            _logger.LogInformation($"No se encontraron Productos con imágenes para la marca {marca}");
            return NotFound($"No se encontraron Productos con imágenes para la marca {marca}.");
        }
    
        return Ok(productos);
    }
    // Endpoint para obtener productos destacados excluyendo cerámicas
    [HttpGet("destacados/excluyendoCeramicas")]
    public async Task<IActionResult> GetProductosDestacadosExcluyendoCeramicas()
    {
        var productos = await _db.Productos
            .Include(p => p.Categoria)
            .ThenInclude(c => c.SubCategoria)
            .Where(p => p.EsDestacado == true && 
                       !p.Categoria.Nombre.Contains("Ceramica") && 
                       !string.IsNullOrEmpty(p.ImageUrl))
            .ToListAsync(); 

        if (!productos.Any())
        {
            _logger.LogInformation("No se encontraron productos destacados con imágenes excluyendo cerámicas");
            return NotFound("No se encontraron productos destacados con imágenes en otras categorías");
        }

        return Ok(productos);
    }

    // Endpoint para obtener productos de la categoría cerámicas que están destacados
    [HttpGet("destacados/ceramicas")]
    public async Task<IActionResult> GetCeramicasDestacadas()
    {
        
    
        var productos = await _db.Productos
            .Include(p => p.Categoria)
            .ThenInclude(c => c.SubCategoria)
            .Where(p => p.EsDestacado == true && 
                         p.Categoria.Nombre.Contains("Ceramica") && 
                        !string.IsNullOrEmpty(p.ImageUrl))
            .ToListAsync();

        if (!productos.Any())
        {
            _logger.LogInformation("No se encontraron productos destacados con imágenes en cerámicas");
            return NotFound("No se encontraron productos destacados con imágenes en cerámicas");
        }

        return Ok(productos);
    }
    [HttpGet("marcas/notceramicas")]
    public async Task<IActionResult> GetMarcasExcluyendoCategoria1006()
    {
        const int categoriaCeramicaId = 5; // ID de la categoría a excluir
    
        var marcas = await _db.Productos
            .Where(p => p.CategoriaId != categoriaCeramicaId || !p.Nombre.Contains("Ceramica")) // Filtramos por ID
            .Select(p => p.Marca)
            .Distinct()
            .ToListAsync();

        if (!marcas.Any())
        {
            _logger.LogInformation("No se encontraron marcas excluyendo categoría 1006");
            return NotFound("No hay marcas disponibles para otras categorías");
        }

        return Ok(marcas);
    }
    
    
    [HttpGet("marcas/yesceramicas")]
    public async Task<IActionResult> GetMarcasDeCeramicas()
    {
        const int categoriaCeramicaId = 5;
    
        var marcas = await _db.Productos
            .Where(p => p.CategoriaId == categoriaCeramicaId || p.Nombre.Contains("Ceramica"))
            .Select(p => p.Marca)
            .Distinct()
            .ToListAsync();

        if (!marcas.Any())
        {
            _logger.LogInformation("No se encontraron marcas en la categoría 1006");
            return NotFound("No hay marcas registradas para cerámicas");
        }

        return Ok(marcas);
    }
    
    
    
    [HttpGet("marcas/{categoriaId}")]
    public async Task<IActionResult> GetMarcasPorCategoria(int categoriaId)
    {
        

        // Obtener marcas únicas de la categoría
        var marcas = await _db.Productos
            .Where(p => p.CategoriaId == categoriaId && p.Marca != null)
            .Select(p => p.Marca)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync();

        if (!marcas.Any())
        {
            _logger.LogInformation($"No hay marcas disponibles para la categoría ID: {categoriaId}");
            return NotFound($"No se encontraron marcas en la categoría seleccionada");
        }

        return Ok(marcas);
    }

    
    
    
    
}