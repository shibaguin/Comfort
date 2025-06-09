using System.Windows.Controls;
using Comfort.ViewModels;

namespace Comfort.Views
{
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
            DataContext = new ProductViewModel((MainWindowViewModel)App.Current.MainWindow.DataContext);
        }
    }
} 