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
    
    
}