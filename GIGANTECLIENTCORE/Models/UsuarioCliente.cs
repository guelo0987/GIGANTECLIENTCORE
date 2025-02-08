using System;
using System.Collections.Generic;

namespace GIGANTECLIENTCORE.Models;

public partial class UsuarioCliente
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public string? Password { get; set; }

    public string? Direccion { get; set; }

    public string? Ciudad { get; set; }

    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }

    public string? Rnc { get; set; }

    public DateOnly? Dob { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public int RolId { get; set; }
    

    public virtual ICollection<HistorialCorreo> HistorialCorreos { get; set; } = new List<HistorialCorreo>();

    public virtual Compañium? RncNavigation { get; set; }
    
    public virtual ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();

    public virtual Role Rol { get; set; } = null!;

    public virtual ICollection<Solicitud> Solicituds { get; set; } = new List<Solicitud>();
}
