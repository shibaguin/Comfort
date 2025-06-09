using System.ComponentModel;
using System.Windows.Controls;
using Comfort.Views;

namespace Comfort.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _windowTitle = "Продукция";
        public string WindowTitle
        {
            get => _windowTitle;
            set { _windowTitle = value; OnPropertyChanged(); }
        }

        private UserControl _currentView = null!;
        public UserControl CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            // По умолчанию открываем ProductView
            CurrentView = new ProductView();
        }
    }
} 