using System.ComponentModel;
using System.Windows.Controls;
using Comfort.Views;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Comfort.ViewModels
{
    // ViewModel главного окна приложения, управляющая навигацией и отображением представлений
    public class MainWindowViewModel : BaseViewModel
    {
        // Текущее активное представление в главной области
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

        // Заголовок окна приложения
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

        // Заголовок в верхней части интерфейса
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

        // Представление для редактирования (отображается в правой части)
        private UserControl? _editView;
        public UserControl? EditView
        {
            get => _editView;
            set { _editView = value; OnPropertyChanged(); }
        }

        // Стек для хранения истории навигации
        private readonly Stack<UserControl> _navigationStack;
        public IServiceProvider ServiceProvider { get; }

        // Команда для возврата на предыдущую страницу
        public ICommand NavigateBackCommand { get; }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _navigationStack = new Stack<UserControl>();
            
            var productView = new ProductView(this);
            CurrentView = productView;
            _navigationStack.Push(productView);

            NavigateBackCommand = new RelayCommand(NavigateBack, CanNavigateBack);
        }

        // Обновляет заголовки окна и шапки в зависимости от текущего представления
        private void UpdateTitles()
        {
            string pageTitle = GetPageTitle(_currentView);
            WindowTitle = $"Комфорт - {pageTitle}";
            HeaderTitle = pageTitle;
        }

        // Возвращает заголовок страницы в зависимости от типа представления
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

        // Переход к новому представлению с сохранением в истории навигации
        public void NavigateTo(UserControl view)
        {
            _navigationStack.Push(view);
            CurrentView = view;
        }

        // Возврат к предыдущему представлению из истории навигации
        public void NavigateBack()
        {
            if (_navigationStack.Count > 1)
            {
                _navigationStack.Pop();
                CurrentView = _navigationStack.Peek();
            }
        }

        // Проверка возможности возврата назад
        public bool CanNavigateBack()
        {
            return _navigationStack.Count > 1;
        }

        // Показывает представление редактирования в правой части окна
        public void ShowEditView(UserControl view)
        {
            EditView = view;
        }

        // Скрывает представление редактирования
        public void HideEditView()
        {
            EditView = null;
        }
    }
} 