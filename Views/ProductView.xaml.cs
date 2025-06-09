using System.Windows.Controls;
using Comfort.ViewModels;

namespace Comfort.Views
{
    public partial class ProductView : UserControl
    {
        public ProductView(MainWindowViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = new ProductViewModel(mainViewModel);
        }
    }
} 