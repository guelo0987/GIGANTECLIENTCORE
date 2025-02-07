using GIGANTECLIENTCORE.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECLIENTCORE.Controllers;




[ApiController]
[Route("api/[controller]")]
public class SubCategoriaController:ControllerBase
{

    private readonly ILogger<SubCategoriaController> _logger;
    private readonly MyDbContext _db;
    

    public SubCategoriaController(MyDbContext db, ILogger<SubCategoriaController> logger)
    {
        _logger = logger;
        _db = db;
    }
    
    
    [HttpGet]
    public IActionResult GetSubCategorias()
    {
        _logger.LogInformation("Obteniendo SubCategorias...");
        return Ok(_db.SubCategoria.Include(o=>o.Categoria).ToList());
    }
    
    
    [HttpGet("{id}")]
    public IActionResult SubGetCategoriaId(int id)
    {
        var subcategorium = _db.SubCategoria
            .Include(o=>o.Categoria).FirstOrDefault(u => u.Id == id);

        if (subcategorium == null)
        {
            _logger.LogError("SubCategoria no Encontrada.");
            return NotFound("Subcategoria no encontrada.");
        }

        return Ok(subcategorium);
    }
    
    
    //Subcategorias por categoria
    [HttpGet("porcategoria/{categoriaId}")]
    public IActionResult GetSubCategoriasByCategoria(int categoriaId)
    {
        var subcategorias = _db.SubCategoria
            .Include(s => s.Categoria)
            .Where(s => s.CategoriaId == categoriaId)
            .ToList();
    
        if (!subcategorias.Any())
        {
            _logger.LogInformation("No se encontraron subcategorías para la categoría con Id: {CategoriaId}", categoriaId);
            return NotFound($"No se encontraron subcategorías para la categoría con Id {categoriaId}.");
        }
    
        return Ok(subcategorias);
    }

}