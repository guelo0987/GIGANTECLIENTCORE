using GIGANTECLIENTCORE.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECLIENTCORE.Controllers;




[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ILogger<CategoriaController> _logger;
    private readonly MyDbContext _db;
    
    public CategoriaController(MyDbContext db, ILogger<CategoriaController> logger)
    {
        _logger = logger;
        _db = db;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCategorias()
    {
        _logger.LogInformation("Obteniendo Categorias...");
        var categorias = await _db.Categoria
            .Include(o => o.SubCategoria)
            .ToListAsync();
        return Ok(categorias);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoriaId(int id)
    {
        var categorium = await _db.Categoria
            .Include(o => o.SubCategoria)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (categorium == null)
        {
            _logger.LogError("Categoria no Encontrada.");
            return NotFound("categoria no encontrada.");
        }

        return Ok(categorium);
    }
}