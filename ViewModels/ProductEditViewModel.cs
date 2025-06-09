using Comfort.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace Comfort.ViewModels
{
    public class ProductEditViewModel : BaseViewModel
    {
        private readonly Product _product;
        private readonly ProductViewModel _parentViewModel;
        private readonly MainWindowViewModel _mainViewModel;

        public string Title => _product.ProductID == 0 ? "Новый продукт" : "Редактирование продукта";
        public Product Product => _product;

        public ObservableCollection<ProductType> ProductTypes { get; }
        public ObservableCollection<Material> Materials { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ProductEditViewModel(Product product, ProductViewModel parentViewModel)
        {
            _product = product;
            _parentViewModel = parentViewModel;
            _mainViewModel = parentViewModel.MainViewModel;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

            using (var db = new ApplicationDbContext())
            {
                ProductTypes = new ObservableCollection<ProductType>(db.ProductTypes.ToList());
                Materials = new ObservableCollection<Material>(db.Materials.ToList());
            }
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(_product.Article))
            {
                MessageBox.Show("Артикул не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(_product.ProductName))
            {
                MessageBox.Show("Наименование не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_product.MinPartnerCost < 0)
            {
                MessageBox.Show("Стоимость не может быть отрицательной", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    if (_product.ProductID == 0)
                    {
                        db.Products.Add(_product);
                    }
                    else
                    {
                        db.Products.Update(_product);
                    }
                    db.SaveChanges();
                }

                _parentViewModel.LoadProducts();
                _mainViewModel.HideEditView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при сохранении продукта: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void Cancel()
        {
            _mainViewModel.HideEditView();
        }
    }
} 