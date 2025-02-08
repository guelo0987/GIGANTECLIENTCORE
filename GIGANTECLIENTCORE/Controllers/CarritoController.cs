using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.Models;

namespace GIGANTECLIENTCORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly MyDbContext _db;

        public CarritoController(MyDbContext context)
        {
            _db = context;
        }






        [HttpGet]
        public IActionResult ObtenerCarritoUsario(int id)
        {
            
            
            var cart = _db.Carrito
                .Include(o=>o.Usuario)
                .Include(o => o.DetallesCarrito)
                .ThenInclude(l => l.Producto)
                .ThenInclude(l => l.Categoria)
                .ThenInclude(o=>o.SubCategoria).Where(o=>o.UsuarioId==id);

            return Ok(cart);

        }

        // 📌 Agregar producto al carrito con usuario enviado en el request
        [HttpPost("agregar")]
        public async Task<IActionResult> AgregaroProducto([FromBody] AgregarCarritoRequest request)
        {
            if (request.UsuarioId <= 0)
            {
                return BadRequest("UsuarioId no válido.");
            }

            var carrito = await _db.Carrito.FirstOrDefaultAsync(c => c.UsuarioId == request.UsuarioId);
            if (carrito == null)
            {
                carrito = new Carrito { UsuarioId = request.UsuarioId };
                _db.Carrito.Add(carrito);
                await _db.SaveChangesAsync();
            }

            var detalle = new DetalleCarrito
            {
                CarritoId = carrito.Id,
                ProductoId = request.ProductoId,
                Cantidad = request.Cantidad
            };

            _db.DetalleCarrito.Add(detalle);
            await _db.SaveChangesAsync();

            return Ok("Producto agregado al carrito.");
        }

        // 📌 Eliminar un producto del carrito
        [HttpDelete("eliminar/{productoId}")]
        public async Task<IActionResult> EliminarProducto(int usuarioId, int productoId)
        {
            var carrito = await _db.Carrito.Include(c => c.DetallesCarrito)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                return NotFound("No tienes un carrito activo.");
            }

            var detalle = carrito.DetallesCarrito.FirstOrDefault(d => d.ProductoId == productoId);
            if (detalle == null)
            {
                return NotFound("El producto no está en el carrito.");
            }

            _db.DetalleCarrito.Remove(detalle);
            await _db.SaveChangesAsync();

            return Ok("Producto eliminado del carrito.");
        }
        
        
        
        
        [HttpPut("editar")]
        public async Task<IActionResult> EditarProducto([FromBody] EditarCarritoRequest request)
        {
            if (request.UsuarioId <= 0)
            {
                return BadRequest("UsuarioId no válido.");
            }

            var carrito = await _db.Carrito.Include(c => c.DetallesCarrito)
                .FirstOrDefaultAsync(c => c.UsuarioId == request.UsuarioId);

            if (carrito == null)
            {
                return NotFound("No tienes un carrito activo.");
            }

            var detalle = carrito.DetallesCarrito.FirstOrDefault(d => d.ProductoId == request.ProductoId);
            if (detalle == null)
            {
                return NotFound("El producto no está en el carrito.");
            }

            detalle.Cantidad = request.Cantidad;
            await _db.SaveChangesAsync();

            return Ok("Producto actualizado en el carrito.");
        }

        
    }
    
    public class AgregarCarritoRequest
    {
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
    
    public class EditarCarritoRequest
    {
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
