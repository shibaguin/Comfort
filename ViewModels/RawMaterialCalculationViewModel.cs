using Comfort.Models;
using Comfort.Services;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace Comfort.ViewModels
{
    // ViewModel для расчета необходимого количества сырья для производства
    public class RawMaterialCalculationViewModel : BaseViewModel
    {
        private readonly IWorkshopService _workshopService;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly ILogger _logger;

        private ObservableCollection<ProductType> _productTypes = new();
        private ObservableCollection<Material> _materials = new();
        private int _selectedProductTypeId;
        private int _selectedMaterialId;
        private int _quantity;
        private double _param1;
        private double _param2;
        private int _result = -1;
        private string _errorMessage = string.Empty;
        private bool _isLoading;
        private Product? _selectedProduct;

        public RawMaterialCalculationViewModel(
            IWorkshopService workshopService,
            IErrorHandlingService errorHandlingService,
            ILogger logger)
        {
            _workshopService = workshopService;
            _errorHandlingService = errorHandlingService;
            _logger = logger;

            CalculateCommand = new RelayCommand(async () => await CalculateRawMaterials(), CanCalculate);
            _ = LoadData();
        }

        // Выбранный продукт, при выборе автоматически заполняются связанные поля
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                if (_selectedProduct != null)
                {
                    SelectedProductTypeId = _selectedProduct.ProductTypeID;
                    SelectedMaterialId = _selectedProduct.MainMaterialID;
                    Quantity = 1;
                    Param1 = 1;
                    Param2 = 1;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsProductSelected));
            }
        }

        // Флаг наличия выбранного продукта
        public bool IsProductSelected => SelectedProduct != null;

        // Коллекция типов продуктов для выбора
        public ObservableCollection<ProductType> ProductTypes
        {
            get => _productTypes;
            set
            {
                _productTypes = value;
                OnPropertyChanged();
            }
        }

        // Коллекция материалов для выбора
        public ObservableCollection<Material> Materials
        {
            get => _materials;
            set
            {
                _materials = value;
                OnPropertyChanged();
            }
        }

        // ID выбранного типа продукта
        public int SelectedProductTypeId
        {
            get => _selectedProductTypeId;
            set
            {
                _selectedProductTypeId = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // ID выбранного материала
        public int SelectedMaterialId
        {
            get => _selectedMaterialId;
            set
            {
                _selectedMaterialId = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Количество единиц для расчета
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Первый параметр для расчета
        public double Param1
        {
            get => _param1;
            set
            {
                _param1 = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Второй параметр для расчета
        public double Param2
        {
            get => _param2;
            set
            {
                _param2 = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Результат расчета количества сырья
        public int Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        // Сообщение об ошибке для отображения пользователю
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        // Флаг загрузки данных
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Команда для запуска расчета
        public ICommand CalculateCommand { get; }

        // Загрузка списков типов продуктов и материалов из базы данных
        private async Task LoadData()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            _logger.Information("Attempting to load ProductTypes and Materials for RawMaterialCalculationViewModel.");
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var productTypes = await db.ProductTypes.ToListAsync();
                    ProductTypes = new ObservableCollection<ProductType>(productTypes);
                    _logger.Information("Loaded {ProductTypeCount} ProductTypes.", productTypes.Count);

                    var materials = await db.Materials.ToListAsync();
                    Materials = new ObservableCollection<Material>(materials);
                    _logger.Information("Loaded {MaterialCount} Materials.", materials.Count);
                }
            }
            catch (Exception ex)
            {
                _errorHandlingService.LogError(ex, "Ошибка загрузки данных для расчета сырья.");
                ErrorMessage = "Не удалось загрузить необходимые данные. Проверьте логи для подробностей.";
                _logger.Error(ex, "Failed to load ProductTypes or Materials for RawMaterialCalculationViewModel.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Проверка возможности выполнения расчета
        private bool CanCalculate()
        {
            return !IsLoading && SelectedProductTypeId > 0 && SelectedMaterialId > 0 && Quantity > 0 && Param1 > 0 && Param2 > 0;
        }

        // Выполнение расчета необходимого количества сырья
        private async Task CalculateRawMaterials()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            Result = -1;
            try
            {
                Result = await _workshopService.CalculateRawMaterials(
                    SelectedProductTypeId,
                    SelectedMaterialId,
                    Quantity,
                    Param1,
                    Param2
                );
                if (Result == -1)
                {
                    ErrorMessage = "Не удалось рассчитать сырье. Проверьте введенные данные.";
                }
            }
            catch (Exception ex)
            {
                _errorHandlingService.LogError(ex, "Ошибка при расчете сырья.");
                ErrorMessage = "Произошла ошибка при расчете сырья.";
                Result = -1;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
} 