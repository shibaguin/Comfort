using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comfort.Models;

// Модель материала
public class Material
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaterialID { get; set; }

    [Required]
    [StringLength(100)]
    public string MaterialName { get; set; } = null!;

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal LossPercentage { get; set; }

    // Навигационное свойство
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
} 