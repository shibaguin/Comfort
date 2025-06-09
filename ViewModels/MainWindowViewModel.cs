using System.ComponentModel;
using System.Windows.Controls;
using Comfort.Views;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Comfort.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private UserControl _currentView = null!;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
                UpdateTitles();
            }
        }

        private string _windowTitle = "Комфорт";
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged();
            }
        }

        private string _headerTitle = "Комфорт";
        public string HeaderTitle
        {
            get => _headerTitle;
            set
            {
                _headerTitle = value;
                OnPropertyChanged();
            }
        }

        private UserControl? _editView;
        public UserControl? EditView
        {
            get => _editView;
            set { _editView = value; OnPropertyChanged(); }
        }

        private readonly Stack<UserControl> _navigationStack;
        public IServiceProvider ServiceProvider { get; }

        public ICommand NavigateBackCommand { get; }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _navigationStack = new Stack<UserControl>();
            
            // Изначально открываем ProductView
            var productView = new ProductView(this);
            CurrentView = productView;
            _navigationStack.Push(productView);

            NavigateBackCommand = new RelayCommand(NavigateBack, CanNavigateBack);
        }

        private void UpdateTitles()
        {
            string pageTitle = GetPageTitle(_currentView);
            WindowTitle = $"Комфорт - {pageTitle}";
            HeaderTitle = pageTitle;
        }

        private string GetPageTitle(UserControl view)
        {
            return view switch
            {
                ProductView => "Продукты",
                WorkshopView => "Цеха производства",
                ProductEditView => "Редактирование продукта",
                RawMaterialCalculationView => "Расчет сырья",
                _ => "Комфорт"
            };
        }

        public void NavigateTo(UserControl view)
        {
            _navigationStack.Push(view);
            CurrentView = view;
        }

        public void NavigateBack()
        {
            if (_navigationStack.Count > 1)
            {
                _navigationStack.Pop(); // Удаляем текущую страницу
                CurrentView = _navigationStack.Peek(); // Переходим к предыдущей
            }
        }

        public bool CanNavigateBack()
        {
            return _navigationStack.Count > 1; // Можно вернуться, если в стеке больше одной страницы
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