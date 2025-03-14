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
            .ToListAsync();
        _logger.LogInformation("Obteniendo Productos");
        return Ok(products);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> ProductoId(int id)
    {
        var products = await _db.Productos
            .Include(o => o.Categoria)
            .ThenInclude(o => o.SubCategoria)
            .FirstOrDefaultAsync(u => u.Codigo == id);

        if (products == null)
        {
            _logger.LogError("Producto no Encontrado.");
            return NotFound("Producto no encontrado.");
        }

        return Ok(products);
    }
    
    //Productos por categoria 
    [HttpGet("porcategoria/{categoriaId}")]
    public async Task<IActionResult> GetProductosByCategoria(int categoriaId)
    {
        var productos = await _db.Productos
            .Include(s => s.Categoria)
            .Where(s => s.CategoriaId == categoriaId)
            .ToListAsync();
    
        if (!productos.Any())
        {
            _logger.LogInformation($"No se encontraron Categorias en el Producto con el Codigo proporcionado", categoriaId);
            return NotFound($"No se encontraron Productos para la categoría con Id {categoriaId}.");
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
            .Where(s => s.CategoriaId == categoriaId && s.SubCategoriaId == subcategoriaId)
            .ToListAsync();
    
        if (!productos.Any())
        {
            _logger.LogInformation($"No se encontraron Productos con la Subcategoria", subcategoriaId);
            return NotFound($"No se encontraron Productos para la subcategoría con Id {subcategoriaId}.");
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
            .Where(s => s.Marca == marca.ToLower())
            .ToListAsync();
    
        if (!productos.Any())
        {
            _logger.LogInformation($"No se encontraron Marcas en el Producto con la marca proporcionada", marca);
            return NotFound($"No se encontraron Productos para la marca  {marca}.");
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
            .Where(p => p.EsDestacado == true 
                        && !p.Nombre.Contains("Ceramica"))
            .ToListAsync(); 

        if (!productos.Any())
        {
            _logger.LogInformation("No se encontraron productos destacados excluyendo categoría 1006");
            return NotFound("No se encontraron productos destacados en otras categorías");
        }

        return Ok(productos);
    }

    // Endpoint para obtener productos de la categoría cerámicas que están destacados
    [HttpGet("destacados/ceramicas")]
    public async Task<IActionResult> GetCeramicasDestacadas()
    {
        const int categoriaCeramicaId = 5;
    
        var productos = await _db.Productos
            .Include(p => p.Categoria)
            .ThenInclude(c => c.SubCategoria)
            .Where(p => p.EsDestacado == true 
                        && p.CategoriaId == categoriaCeramicaId || p.Nombre.Contains("Ceramica"))
            .ToListAsync();

        if (!productos.Any())
        {
            _logger.LogInformation("No se encontraron productos destacados de la categoría 1006");
            return NotFound("No se encontraron productos destacados en cerámicas");
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

    
    
    
    
}