using System;
using System.Collections.Generic;

namespace GIGANTECLIENTCORE.Models;

public partial class Producto
{
    public int Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Marca { get; set; }

    public bool? Stock { get; set; }

    public int SubCategoriaId { get; set; }

    public string? ImageUrl { get; set; }

    public int? CategoriaId { get; set; }
    
    public string? Descripcion { get; set; }
    

    public virtual Categorium? Categoria { get; set; }

    public virtual ICollection<DetalleSolicitud> DetalleSolicituds { get; set; } = new List<DetalleSolicitud>();
    public virtual ICollection<DetalleCarrito> DetallesCarrito { get; set; } = new List<DetalleCarrito>();
    public virtual SubCategorium SubCategoria { get; set; } = null!;
}
