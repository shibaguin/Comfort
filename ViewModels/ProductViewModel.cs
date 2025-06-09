using Comfort.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Comfort.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public ObservableCollection<Product> Products { get; set; }

        public ProductViewModel()
        {
            using (var db = new ApplicationDbContext())
            {
                Products = new ObservableCollection<Product>(
                    db.Products
                        .Include(p => p.ProductType)
                        .Include(p => p.MainMaterial)
                        .Include(p => p.ProductWorkshops)
                        .ToList()
                );
            }
        }
    }
} 