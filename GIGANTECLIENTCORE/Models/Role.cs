using System;
using System.Collections.Generic;

namespace GIGANTECLIENTCORE.Models;

public partial class Role
{
    public int IdRol { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<RolePermiso> RolePermisos { get; set; } = new List<RolePermiso>();

    public virtual ICollection<UsuarioCliente> UsuarioClientes { get; set; } = new List<UsuarioCliente>();
}
