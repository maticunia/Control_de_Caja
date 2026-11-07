using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CajaApp.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CajaApp.UI.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ICajaService _cajaService;
    private readonly IClienteService _clienteService;
    private readonly IMovimientoService _movimientoService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private object? _currentViewModel;

    [ObservableProperty]
    private string _currentViewName = "Inicio";

    [ObservableProperty]
    private bool _isCajaAbierta;

    [ObservableProperty]
    private string _cajaActualNombre = "Ninguna";

    public MainViewModel(
        ICajaService cajaService,
        IClienteService clienteService,
        IMovimientoService movimientoService,
        IServiceProvider serviceProvider)
    {
        _cajaService = cajaService;
        _clienteService = clienteService;
        _movimientoService = movimientoService;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private async Task NavigateToCajaAsync()
    {
        await CheckCajaStatusAsync();
        var vm = _serviceProvider.GetRequiredService<CajaViewModel>();
        await vm.LoadAsync();
        CurrentViewModel = vm;
        CurrentViewName = "Cajas";
    }

    [RelayCommand]
    private async Task NavigateToClientesAsync()
    {
        await CheckCajaStatusAsync();
        var vm = _serviceProvider.GetRequiredService<ClienteViewModel>();
        await vm.LoadAsync();
        CurrentViewModel = vm;
        CurrentViewName = "Clientes";
    }

    [RelayCommand]
    private async Task NavigateToMovimientosAsync()
    {
        await CheckCajaStatusAsync();
        var vm = _serviceProvider.GetRequiredService<MovimientoViewModel>();
        await vm.LoadAsync();
        CurrentViewModel = vm;
        CurrentViewName = "Movimientos";
    }

    [RelayCommand]
    private async Task NavigateToAbrirCajaAsync()
    {
        var vm = _serviceProvider.GetRequiredService<AbrirCajaViewModel>();
        await vm.LoadAsync();
        CurrentViewModel = vm;
        CurrentViewName = "Abrir Caja";
    }

    [RelayCommand]
    private async Task NavigateToHomeAsync()
    {
        await CheckCajaStatusAsync();
        CurrentViewModel = null;
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
