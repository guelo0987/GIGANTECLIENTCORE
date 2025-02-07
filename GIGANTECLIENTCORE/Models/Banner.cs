using System;
using System.Collections.Generic;

namespace GIGANTECLIENTCORE.Models;

public partial class Banner
{
    public int Id  { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool Active { get; set; }

    public int OrderIndex { get; set; }

    public DateTime CreatedAt { get; set; }
}
