using CajaApp.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace CajaApp.UI.Views.Caja;

public partial class CajaView : UserControl
{
    public CajaView()
    {
        InitializeComponent();
    }

    public CajaView(CajaViewModel viewModel) : this()
    {
        DataContext = viewModel;
        Loaded += async (s, e) =>
        {
            await viewModel.LoadAsync();
        };
    }
}
