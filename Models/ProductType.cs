using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comfort.Models;

// Модель типа продукта производства
public class ProductType
{
    // Уникальный идентификатор типа продукта
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductTypeID { get; set; }

    // Наименование типа продукта
    [Required]
    [StringLength(100)]
    public string ProductTypeName { get; set; } = null!;

    // Коэффициент для расчета сырья
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Coefficient { get; set; }

    // Связь с продуктами данного типа
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
} 