﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Comfort.ViewModels;

namespace Comfort.Views;

// Главное окно приложения, инициализирует основной ViewModel и устанавливает ProductView как начальное представление
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var vm = new MainWindowViewModel(App.Services);
        DataContext = vm;
        var productView = new ProductView(vm);
        vm.CurrentView = productView;
    }
}