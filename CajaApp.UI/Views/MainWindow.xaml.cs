using CajaApp.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CajaApp.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        Loaded += async (s, e) =>
        {
            await viewModel.InitializeAsync();
        };
    }
}
