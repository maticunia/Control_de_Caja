using CajaApp.UI.ViewModels;
using System.Windows.Controls;

namespace CajaApp.UI.Views.Cliente;

public partial class ClienteView : UserControl
{
    public ClienteView()
    {
        InitializeComponent();
    }

    public ClienteView(ClienteViewModel viewModel) : this()
    {
        DataContext = viewModel;
        Loaded += async (s, e) =>
        {
            await viewModel.LoadAsync();
        };
    }
}
