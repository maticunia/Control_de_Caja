using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CajaApp.Application.Interfaces;
using CajaApp.UI.Views;

namespace CajaApp.UI.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ICajaService _cajaService;
    private readonly IClienteService _clienteService;
    private readonly IMovimientoService _movimientoService;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private string _currentViewName = "Inicio";

    [ObservableProperty]
    private bool _isCajaAbierta;

    [ObservableProperty]
    private string _cajaActualNombre = "Ninguna";

    public MainViewModel(
        ICajaService cajaService,
        IClienteService clienteService,
        IMovimientoService movimientoService)
    {
        _cajaService = cajaService;
        _clienteService = clienteService;
        _movimientoService = movimientoService;
    }

    [RelayCommand]
    private async Task NavigateToCajaAsync()
    {
        await CheckCajaStatusAsync();
        CurrentView = new CajaView();
        CurrentViewName = "Cajas";
    }

    [RelayCommand]
    private async Task NavigateToClientesAsync()
    {
        await CheckCajaStatusAsync();
        CurrentView = new ClienteView();
        CurrentViewName = "Clientes";
    }

    [RelayCommand]
    private async Task NavigateToMovimientosAsync()
    {
        await CheckCajaStatusAsync();
        CurrentView = new MovimientoView();
        CurrentViewName = "Movimientos";
    }

    [RelayCommand]
    private async Task NavigateToAbrirCajaAsync()
    {
        CurrentView = new AbrirCajaView();
        CurrentViewName = "Abrir Caja";
    }

    [RelayCommand]
    private async Task NavigateToHomeAsync()
    {
        await CheckCajaStatusAsync();
        CurrentView = null;
        CurrentViewName = "Inicio";
    }

    private async Task CheckCajaStatusAsync()
    {
        var result = await _cajaService.GetCajaAbiertaAsync();
        if (result.IsSuccess && result.Value is not null)
        {
            IsCajaAbierta = true;
            CajaActualNombre = result.Value.Nombre;
        }
        else
        {
            IsCajaAbierta = false;
            CajaActualNombre = "Ninguna";
        }
    }

    public async Task InitializeAsync()
    {
        await CheckCajaStatusAsync();
    }
}
