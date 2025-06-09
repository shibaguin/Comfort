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

        private UserControl? _editView;
        public UserControl? EditView
        {
            get => _editView;
            set { _editView = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            // По умолчанию открываем ProductView
            CurrentView = new ProductView(this);
        }

        // Метод для управления областью редактирования
        public void ShowEditView(UserControl view)
        {
            EditView = view;
        }

        public void HideEditView()
        {
            EditView = null;
        }
    }
} 