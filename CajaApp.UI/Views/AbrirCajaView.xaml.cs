using CajaApp.UI.ViewModels;
using System.Windows.Controls;

namespace CajaApp.UI.Views;

public partial class AbrirCajaView : UserControl
{
    public AbrirCajaView()
    {
        InitializeComponent();
    }

    public AbrirCajaView(AbrirCajaViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
