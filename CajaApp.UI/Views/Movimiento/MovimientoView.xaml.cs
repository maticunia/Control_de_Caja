using CajaApp.UI.ViewModels;
using System.Windows.Controls;

namespace CajaApp.UI.Views.Movimiento;

public partial class MovimientoView : UserControl
{
    public MovimientoView()
    {
        InitializeComponent();
    }

    public MovimientoView(MovimientoViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
