using Comfort.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using System.Windows;
using System;
using Comfort.Views;

namespace Comfort.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly MainWindowViewModel _mainViewModel;
        public MainWindowViewModel MainViewModel => _mainViewModel;
        public ObservableCollection<Product> Products { get; set; }

        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public ProductViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Products = new ObservableCollection<Product>();
            
            AddProductCommand = new RelayCommand(AddProduct);
            EditProductCommand = new RelayCommand<Product>(EditProduct);
            DeleteProductCommand = new RelayCommand<Product>(DeleteProduct);

            LoadProducts();
        }

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

        private void AddProduct()
        {
            var newProduct = new Product();
            _mainViewModel.ShowEditView(new ProductEditView(newProduct, this));
        }

        private void EditProduct(Product product)
        {
            if (product == null) return;
            _mainViewModel.ShowEditView(new ProductEditView(product, this));
        }

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
                    }
                    Products.Remove(product);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при удалении продукта: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }
    }
} 