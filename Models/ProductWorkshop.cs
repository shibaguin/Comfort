using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comfort.Models;

public class ProductWorkshop
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductWorkshopID { get; set; }

    [Required]
    public int ProductID { get; set; }

    [Required]
    public int WorkshopID { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal ManufacturingTime { get; set; }

    // Навигационные свойства
    [ForeignKey("ProductID")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("WorkshopID")]
    public virtual Workshop Workshop { get; set; } = null!;
} 