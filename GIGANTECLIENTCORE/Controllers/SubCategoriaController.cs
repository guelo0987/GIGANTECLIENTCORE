using GIGANTECLIENTCORE.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECLIENTCORE.Controllers;




[ApiController]
[Route("api/[controller]")]
public class SubCategoriaController : ControllerBase
{
    private readonly ILogger<SubCategoriaController> _logger;
    private readonly MyDbContext _db;
    
    public SubCategoriaController(MyDbContext db, ILogger<SubCategoriaController> logger)
    {
        _logger = logger;
        _db = db;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSubCategorias()
    {
        _logger.LogInformation("Obteniendo SubCategorias...");
        var subCategorias = await _db.SubCategoria
            .Include(o => o.Categoria)
            .ToListAsync();
        return Ok(subCategorias);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> SubGetCategoriaId(int id)
    {
        var subcategorium = await _db.SubCategoria
            .Include(o => o.Categoria)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (subcategorium == null)
        {
            _logger.LogError("SubCategoria no Encontrada.");
            return NotFound("Subcategoria no encontrada.");
        }

        return Ok(subcategorium);
    }
    
    //Subcategorias por categoria
    [HttpGet("porcategoria/{categoriaId}")]
    public async Task<IActionResult> GetSubCategoriasByCategoria(int categoriaId)
    {
        var subcategorias = await _db.SubCategoria
            .Include(s => s.Categoria)
            .Where(s => s.CategoriaId == categoriaId)
            .ToListAsync();
    
        if (!subcategorias.Any())
        {
            _logger.LogInformation("No se encontraron subcategorías para la categoría con Id: {CategoriaId}", categoriaId);
            return NotFound($"No se encontraron subcategorías para la categoría con Id {categoriaId}.");
        }
    
        return Ok(subcategorias);
    }

}