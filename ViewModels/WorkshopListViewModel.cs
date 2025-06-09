using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Comfort.Services;
using Serilog;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Comfort.Models;

namespace Comfort.ViewModels
{
    // ViewModel для управления списком цехов и их связями с продуктами
    public class WorkshopListViewModel : BaseViewModel
    {
        private readonly IWorkshopService _workshopService;
        private readonly IErrorHandlingService _errorHandling;
        private readonly ILogger _logger;

        private int _selectedProductId;
        private bool _isLoading;
        private string _errorMessage = string.Empty;

        public WorkshopListViewModel(
            IWorkshopService workshopService,
            IErrorHandlingService errorHandling,
            ILogger logger)
        {
            _workshopService = workshopService;
            _errorHandling = errorHandling;
            _logger = logger;

            Workshops = new ObservableCollection<WorkshopInfo>();
            Products = new ObservableCollection<Product>();
            LoadWorkshopsCommand = new RelayCommand(async () => await LoadWorkshops(), () => !IsLoading);

            LoadProducts();
        }

        // Коллекция цехов для выбранного продукта
        public ObservableCollection<WorkshopInfo> Workshops { get; }
        // Коллекция всех продуктов для выбора
        public ObservableCollection<Product> Products { get; }

        // ID выбранного продукта, при изменении автоматически загружаются связанные цеха
        public int SelectedProductId
        {
            get => _selectedProductId;
            set
            {
                if (_selectedProductId != value)
                {
                    _selectedProductId = value;
                    OnPropertyChanged();
                    _ = LoadWorkshops();
                }
            }
        }

        // Флаг загрузки данных
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        // Сообщение об ошибке для отображения пользователю
        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        // Команда для обновления списка цехов
        public ICommand LoadWorkshopsCommand { get; }

        // Загрузка списка всех продуктов из базы данных
        private void LoadProducts()
        {
            try
            {
                using var db = new ApplicationDbContext();
                var products = db.Products
                    .Include(p => p.ProductType)
                    .OrderBy(p => p.ProductName)
                    .ToList();

                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading products");
                _errorHandling.LogError(ex, "Ошибка при загрузке списка продуктов");
            }
        }

        // Асинхронная загрузка цехов для выбранного продукта
        private async Task LoadWorkshops()
        {
            if (SelectedProductId <= 0)
            {
                ErrorMessage = "Выберите продукт";
                return;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                _logger.Information("Loading workshops for product {ProductId}", SelectedProductId);
                var workshops = await _workshopService.GetWorkshopsForProduct(SelectedProductId);

                Workshops.Clear();
                foreach (var workshop in workshops)
                {
                    Workshops.Add(workshop);
                }

                _logger.Information("Loaded {Count} workshops", workshops.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading workshops");
                _errorHandling.LogError(ex, "Ошибка при загрузке списка цехов");
                ErrorMessage = "Не удалось загрузить список цехов";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
} 