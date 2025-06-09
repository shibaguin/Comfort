using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comfort.Models;

// Модель материала для производства
public class Material
{
    // Уникальный идентификатор материала
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaterialID { get; set; }

    // Наименование материала
    [Required]
    [StringLength(100)]
    public string MaterialName { get; set; } = null!;

    // Процент потерь при производстве
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal LossPercentage { get; set; }

    // Связь с продуктами, использующими данный материал
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
} 