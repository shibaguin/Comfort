using System.Windows.Controls;

namespace Comfort.Views
{
    public partial class ProductEditView : UserControl
    {
        public ProductEditView(Models.Product product, ViewModels.ProductViewModel viewModel)
        {
            InitializeComponent();
            DataContext = new ViewModels.ProductEditViewModel(product, viewModel);
        }
    }
} 