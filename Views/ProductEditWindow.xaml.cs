using System.Windows;
using Comfort.Models;
using Comfort.ViewModels;

namespace Comfort.Views
{
    public partial class ProductEditWindow : Window
    {
        public ProductEditWindow(Product product, ProductViewModel parentViewModel)
        {
            InitializeComponent();
            // Устанавливаем DataContext для всего окна, ProductEditView унаследует его
            DataContext = new ProductEditViewModel(product, parentViewModel, this);
            // Устанавливаем заголовок окна в зависимости от режима (добавление/редактирование)
            Title = product.ProductID == 0 ? "Добавить продукт" : "Редактировать продукт";
        }
    }
} 