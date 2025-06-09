using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comfort.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductID { get; set; }

    [Required]
    [StringLength(50)]
    public string Article { get; set; } = null!;

    [Required]
    public int ProductTypeID { get; set; }

    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinPartnerCost { get; set; }

    [Required]
    public int MainMaterialID { get; set; }

    // Навигационные свойства
    [ForeignKey("ProductTypeID")]
    public virtual ProductType ProductType { get; set; } = null!;

    [ForeignKey("MainMaterialID")]
    public virtual Material MainMaterial { get; set; } = null!;

    public virtual ICollection<ProductWorkshop> ProductWorkshops { get; set; } = new List<ProductWorkshop>();

    // Вычисляемое свойство для времени изготовления
    [NotMapped]
    public int ManufacturingTime => (int)Math.Ceiling(ProductWorkshops.Sum(pw => pw.ManufacturingTime));
} 