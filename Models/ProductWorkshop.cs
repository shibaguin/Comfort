using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comfort.Models;

// Модель связи продукта с цехом производства
public class ProductWorkshop
{
    // Уникальный идентификатор связи
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductWorkshopID { get; set; }

    // Идентификатор продукта
    [Required]
    public int ProductID { get; set; }

    // Идентификатор цеха
    [Required]
    public int WorkshopID { get; set; }

    // Время производства в цехе (в часах)
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal ManufacturingTime { get; set; }

    // Связь с продуктом
    [ForeignKey("ProductID")]
    public virtual Product Product { get; set; } = null!;

    // Связь с цехом
    [ForeignKey("WorkshopID")]
    public virtual Workshop Workshop { get; set; } = null!;
} 