using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECLIENTCORE.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SolicitudesController:ControllerBase
{


    private readonly MyDbContext _db;
    private readonly ILogger<SolicitudesController> _logger;
    
    public SolicitudesController(MyDbContext db, ILogger<SolicitudesController> logger)
    {

        _db = db;
        _logger = logger;
    }
    
    
    
    
    
    [HttpPost("confirmar")]
    public async Task<IActionResult> ConfirmarSolicitud([FromBody] ConfirmarCompraRequest request)
    {
        if (request.UsuarioId <= 0)
        {
            return BadRequest("UsuarioId no válido.");
        }

        var carrito = await _db.Carrito.Include(c => c.DetallesCarrito)
            .FirstOrDefaultAsync(c => c.UsuarioId == request.UsuarioId);

        if (carrito == null || carrito.DetallesCarrito.Count == 0)
        {
            return BadRequest("El carrito está vacío.");
        }

        var solicitud = new Solicitud { UsuarioId = request.UsuarioId, FechaSolicitud = DateTime.UtcNow };
        _db.Solicituds.Add(solicitud);
        await _db.SaveChangesAsync();

        foreach (var item in carrito.DetallesCarrito)
        {
            _db.DetalleSolicituds.Add(new DetalleSolicitud
            {
                IdSolicitud = solicitud.IdSolicitud,
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad
            });
        }

        _db.Carrito.Remove(carrito);
        await _db.SaveChangesAsync();

        return Ok("Solicitud confirmada.");
    }


    [HttpGet("SolicitudesUsuario/{id}")]
    public async Task<IActionResult> GetSolicitudesUsuario(int id)
    {
        var solicitudesUsuario = await _db.Solicituds
            .Include(o => o.Usuario)
            .Include(o => o.DetalleSolicituds)
            .ThenInclude(l => l.Producto)
            .ThenInclude(l => l.Categoria)
            .ThenInclude(l => l.SubCategoria)
            .Where(o => o.UsuarioId == id)
            .ToListAsync();

        if (!solicitudesUsuario.Any())
        {
            _logger.LogInformation("No se encontraron Solitudes en el Usuario");
            return NotFound("No se encontraron Solicitudes en el usuario");
        }

        return Ok(solicitudesUsuario);
    }
    
}

public class ConfirmarCompraRequest
{
    public int UsuarioId { get; set; }
}