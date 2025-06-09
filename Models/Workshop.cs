using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comfort.Models;

// Модель цеха
public class Workshop
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int WorkshopID { get; set; }

    [Required]
    [StringLength(100)]
    public string WorkshopName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string WorkshopType { get; set; } = null!;

    [Required]
    public int StaffCount { get; set; }

    // Навигационное свойство
    public virtual ICollection<ProductWorkshop> ProductWorkshops { get; set; } = new List<ProductWorkshop>();
} 