using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comfort.Models;

// Модель продукта производства
public class Product
{
    // Уникальный идентификатор продукта
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductID { get; set; }

    // Артикул продукта
    [Required]
    [StringLength(50)]
    public string Article { get; set; } = null!;

    // Идентификатор типа продукта
    [Required]
    public int ProductTypeID { get; set; }

    // Наименование продукта
    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    // Минимальная стоимость для партнера
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinPartnerCost { get; set; }

    // Идентификатор основного материала
    [Required]
    public int MainMaterialID { get; set; }

    // Связь с типом продукта
    [ForeignKey("ProductTypeID")]
    public virtual ProductType ProductType { get; set; } = null!;

    // Связь с основным материалом
    [ForeignKey("MainMaterialID")]
    public virtual Material MainMaterial { get; set; } = null!;

    // Связь с цехами производства
    public virtual ICollection<ProductWorkshop> ProductWorkshops { get; set; } = new List<ProductWorkshop>();

    // Общее время изготовления продукта (сумма времени по всем цехам)
    [NotMapped]
    public int ManufacturingTime => (int)Math.Ceiling(ProductWorkshops.Sum(pw => pw.ManufacturingTime));
} 