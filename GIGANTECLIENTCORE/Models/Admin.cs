using System;
using System.Collections.Generic;

namespace GIGANTECLIENTCORE.Models;

public partial class Admin
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? FechaIngreso { get; set; }

    public string? Telefono { get; set; }

    public bool? SoloLectura { get; set; }

    public int? RolId { get; set; }

    public virtual Role? Rol { get; set; }
}
