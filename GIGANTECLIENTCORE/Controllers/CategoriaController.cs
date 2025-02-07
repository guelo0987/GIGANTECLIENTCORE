using GIGANTECLIENTCORE.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECLIENTCORE.Controllers;




[ApiController]
[Route("api/[controller]")]
public class CategoriaController:ControllerBase
{

    private readonly ILogger<CategoriaController> _logger;
    private readonly MyDbContext _db;
    

    public CategoriaController(MyDbContext db, ILogger<CategoriaController> logger)
    {
        _logger = logger;
        _db = db;
    }
    
    
    [HttpGet]
    public IActionResult GetCategorias()
    {
        _logger.LogInformation("Obteniendo Categorias...");
        return Ok(_db.Categoria.Include(o=>o.SubCategoria).ToList());
    }
    
    
    [HttpGet("{id}")]
    public IActionResult GetCategoriaId(int id)
    {
        var categorium = _db.Categoria
            .Include(o=>o.SubCategoria).FirstOrDefault(u => u.Id == id);

        if (categorium == null)
        {
            _logger.LogError("Categoria no Encontrada.");
            return NotFound("categoria no encontrada.");
        }

        return Ok(categorium);
    }

}