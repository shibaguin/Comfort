using System.Windows.Controls;
using Comfort.ViewModels;

namespace Comfort.Views
{
    // Представление для отображения и управления списком продуктов
    public partial class ProductView : UserControl
    {
        public ProductView(MainWindowViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = new ProductViewModel(mainViewModel);
        }
    }
} 