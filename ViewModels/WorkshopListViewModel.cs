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

        public ObservableCollection<WorkshopInfo> Workshops { get; }
        public ObservableCollection<Product> Products { get; }

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

        public ICommand LoadWorkshopsCommand { get; }

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