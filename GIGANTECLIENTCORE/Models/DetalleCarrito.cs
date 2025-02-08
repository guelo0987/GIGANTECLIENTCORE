using System;

namespace GIGANTECLIENTCORE.Models
{
    public partial class DetalleCarrito
    {
        public int Id { get; set; }
        public int CarritoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }

        // Relación con Carrito
        public virtual Carrito Carrito { get; set; } = null!;

        // Relación con Producto
        public virtual Producto Producto { get; set; } = null!;
    }
}