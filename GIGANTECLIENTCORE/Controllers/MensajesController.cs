using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GIGANTECLIENTCORE.Controllers;



[ApiController]
[Route("api/[controller]")]
public class MensajesController:ControllerBase
{

    private readonly ILogger<MensajesController> _logger;
    private readonly MyDbContext _db;
    

    public MensajesController(MyDbContext db, ILogger<MensajesController> logger)
    {
        _db = db;
        _logger = logger;
    }





    [HttpPost]
    public async Task<IActionResult> MandarMensaje([FromBody] mensajes msj)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Agregar mensaje a la base de datos
        _db.mensajes.Add(msj);
        await _db.SaveChangesAsync();

        return Created("Creado", msj);

    }
}