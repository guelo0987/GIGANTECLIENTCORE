using System;
using System.Collections.Generic;

namespace GIGANTECLIENTCORE.Models;

public partial class RolePermiso
{
    public int IdPermiso { get; set; }

    public int RoleId { get; set; }

    public string TableName { get; set; } = null!;

    public bool CanCreate { get; set; }

    public bool CanRead { get; set; }

    public bool CanUpdate { get; set; }

    public bool CanDelete { get; set; }

    public virtual Role Role { get; set; } = null!;
}
