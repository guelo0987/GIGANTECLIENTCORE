using System;
using System.Collections.Generic;

namespace GIGANTECLIENTCORE.Models;

public partial class SubCategorium
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int CategoriaId { get; set; }

    public virtual Categorium Categoria { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
