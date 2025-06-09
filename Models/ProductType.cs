using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comfort.Models;

// Модель типа продукта
public class ProductType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductTypeID { get; set; }

    [Required]
    [StringLength(100)]
    public string ProductTypeName { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal Coefficient { get; set; }

    // Навигационное свойство
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
} 