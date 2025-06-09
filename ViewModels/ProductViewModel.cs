using Comfort.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using System.Windows;
using System;
using Comfort.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Comfort.Services;
using Serilog;

namespace Comfort.ViewModels
{
    // ViewModel для управления списком продуктов и операциями с ними
    public class ProductViewModel : BaseViewModel
    {
        private readonly MainWindowViewModel _mainViewModel;
        public MainWindowViewModel MainViewModel => _mainViewModel;
        // Коллекция продуктов для отображения в интерфейсе
        public ObservableCollection<Product> Products { get; set; }

        // Команды для управления продуктами
        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ShowWorkshopsCommand { get; }
        public ICommand ShowRawMaterialCalculationCommand { get; }

        public ProductViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Products = new ObservableCollection<Product>();
            
            AddProductCommand = new RelayCommand(AddProduct);
            EditProductCommand = new RelayCommand<Product>(EditProduct);
            DeleteProductCommand = new RelayCommand<Product>(DeleteProduct);
            ShowWorkshopsCommand = new RelayCommand<Product>(ShowWorkshops);
            ShowRawMaterialCalculationCommand = new RelayCommand<Product>(ShowRawMaterialCalculation);

            LoadProducts();
        }

        // Загрузка списка продуктов из базы данных с включением связанных данных
        public void LoadProducts()
        {
            using (var db = new ApplicationDbContext())
            {
                Products = new ObservableCollection<Product>(
                    db.Products
                        .Include(p => p.ProductType)
                        .Include(p => p.MainMaterial)
                        .Include(p => p.ProductWorkshops)
                        .ToList()
                );
            }
            OnPropertyChanged(nameof(Products));
        }

        // Открытие окна создания нового продукта
        private void AddProduct()
        {
            var newProduct = new Product();
            var editView = new ProductEditView
            {
                DataContext = new ProductEditViewModel(newProduct, this, _mainViewModel)
            };
            _mainViewModel.NavigateTo(editView);
        }

        // Открытие окна редактирования существующего продукта
        private void EditProduct(Product product)
        {
            if (product == null) return;
            var editView = new ProductEditView
            {
                DataContext = new ProductEditViewModel(product, this, _mainViewModel)
            };
            _mainViewModel.NavigateTo(editView);
        }

        // Удаление продукта с подтверждением
        private void DeleteProduct(Product product)
        {
            if (product == null) return;

            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить этот продукт?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new ApplicationDbContext())
                    {
                        db.Products.Remove(product);
                        db.SaveChanges();
                        Products.Remove(product);
                    }
                }
                catch (Exception ex)
                {
                    _mainViewModel.ServiceProvider.GetRequiredService<IErrorHandlingService>().LogError(ex, "Ошибка при удалении продукта");
                    MessageBox.Show($"Ошибка при удалении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Открытие окна управления цехами для выбранного продукта
        private void ShowWorkshops(Product product)
        {
            if (product == null) return;
            var workshopViewModel = _mainViewModel.ServiceProvider.GetRequiredService<WorkshopListViewModel>();
            workshopViewModel.SelectedProductId = product.ProductID;
            var workshopView = new WorkshopView
            {
                DataContext = workshopViewModel
            };
            _mainViewModel.NavigateTo(workshopView);
        }

        // Открытие окна расчета сырья для выбранного продукта
        private void ShowRawMaterialCalculation(Product product)
        {
            var rawMaterialCalculationViewModel = _mainViewModel.ServiceProvider.GetRequiredService<RawMaterialCalculationViewModel>();
            rawMaterialCalculationViewModel.SelectedProduct = product;
            var rawMaterialCalculationView = new RawMaterialCalculationView
            {
                DataContext = rawMaterialCalculationViewModel
            };
            _mainViewModel.NavigateTo(rawMaterialCalculationView);
        }
    }
} 