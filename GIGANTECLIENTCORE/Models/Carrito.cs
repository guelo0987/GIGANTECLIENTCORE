using System;
using System.Collections.Generic;

namespace GIGANTECLIENTCORE.Models
{
    public partial class Carrito
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; } // Solo para usuarios registrados
        
        // Relación con UsuarioCliente
        public virtual UsuarioCliente Usuario { get; set; } = null!;

        // Relación con DetalleCarrito
        public virtual ICollection<DetalleCarrito> DetallesCarrito { get; set; } = new List<DetalleCarrito>();
    }
}