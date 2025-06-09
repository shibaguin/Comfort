using Comfort.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using Comfort.Views;

namespace Comfort.ViewModels
{
    /// <summary>
    /// ViewModel для формы редактирования продукта
    /// </summary>
    public class ProductEditViewModel : BaseViewModel
    {
        private readonly Product _originalProduct; // Ссылка на исходный объект продукта
        private Product _editableProduct; // Копия продукта для редактирования

        // Заголовок формы зависит от режима (добавление/редактирование)
        public string Title => _originalProduct.ProductID == 0 ? "Новый продукт" : "Редактирование продукта";
        public Product Product // Свойство, к которому привязывается UI
        {
            get => _editableProduct;
            set
            {
                _editableProduct = value;
                OnPropertyChanged();
            }
        }

        private readonly ProductViewModel _parentViewModel;
        private readonly MainWindowViewModel _mainViewModel;

        // Коллекции для выпадающих списков
        public ObservableCollection<ProductType> ProductTypes { get; }
        public ObservableCollection<Material> Materials { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ProductEditViewModel(Product product, ProductViewModel parentViewModel, MainWindowViewModel mainViewModel)
        {
            _originalProduct = product;
            _parentViewModel = parentViewModel;
            _mainViewModel = mainViewModel;

            // Создаем копию продукта для редактирования. Если это новый продукт, используем его напрямую.
            if (_originalProduct.ProductID == 0)
            {
                _editableProduct = _originalProduct; // Для нового продукта _originalProduct уже является новой инстанцией
            }
            else
            {
                // Копируем свойства существующего продукта в новую инстанцию для редактирования
                _editableProduct = new Product
                {
                    ProductID = _originalProduct.ProductID,
                    Article = _originalProduct.Article,
                    ProductName = _originalProduct.ProductName,
                    MinPartnerCost = _originalProduct.MinPartnerCost,
                    ProductTypeID = _originalProduct.ProductTypeID,
                    MainMaterialID = _originalProduct.MainMaterialID
                    // Навигационные свойства не копируем, так как они будут подгружены при сохранении или refresh'е
                };
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

            // Загружаем данные для выпадающих списков
            using (var db = new ApplicationDbContext())
            {
                ProductTypes = new ObservableCollection<ProductType>(db.ProductTypes.ToList());
                Materials = new ObservableCollection<Material>(db.Materials.ToList());
            }
        }

        /// <summary>
        /// Сохранение продукта в базу данных
        /// </summary>
        private void Save()
        {
            // Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(Product.Article))
            {
                MessageBox.Show("Артикул не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Product.ProductName))
            {
                MessageBox.Show("Наименование не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка корректности стоимости
            if (Product.MinPartnerCost < 0)
            {
                MessageBox.Show("Стоимость не может быть отрицательной", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    if (_originalProduct.ProductID == 0) // Новый продукт
                    {
                        db.Products.Add(Product); // Добавляем отредактированную копию
                    }
                    else // Существующий продукт
                    {
                        // Копируем изменения из редактируемой копии в исходный объект
                        _originalProduct.Article = Product.Article;
                        _originalProduct.ProductName = Product.ProductName;
                        _originalProduct.MinPartnerCost = Product.MinPartnerCost;
                        _originalProduct.ProductTypeID = Product.ProductTypeID;
                        _originalProduct.MainMaterialID = Product.MainMaterialID;

                        db.Products.Update(_originalProduct); // Обновляем исходный объект
                    }
                    db.SaveChanges();
                }

                // Обновляем список продуктов в родительском окне
                _parentViewModel.LoadProducts();
                _mainViewModel.NavigateBack();
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

        /// <summary>
        /// Возврат к предыдущей странице без сохранения изменений
        /// </summary>
        private void Cancel()
        {
            _mainViewModel.NavigateBack();
        }
    }
} 