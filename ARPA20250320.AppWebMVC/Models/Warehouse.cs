using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ARPA20250320.AppWebMVC.Models;

public partial class Warehouse
{
    [Display(Name = "IdBodega")]
    public int WarehouseId { get; set; }
    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string WarehouseName { get; set; } = null!;
    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
