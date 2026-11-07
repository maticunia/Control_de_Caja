using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CajaApp.Application.DTOs.Caja;
using CajaApp.Application.DTOs.Cliente;
using CajaApp.Application.DTOs.Movimiento;
using CajaApp.Application.Interfaces;
using CajaApp.Domain.Enums;
using System.Collections.ObjectModel;

namespace CajaApp.UI.ViewModels;

public partial class MovimientoViewModel : BaseViewModel
{
    private readonly IMovimientoService _movimientoService;
    private readonly ICajaService _cajaService;
    private readonly IClienteService _clienteService;

    [ObservableProperty]
    private ObservableCollection<MovimientoCajaDto> _movimientos = new();

    [ObservableProperty]
    private MovimientoCajaDto? _selectedMovimiento;

    [ObservableProperty]
    private ObservableCollection<CajaDto> _cajas = new();

    [ObservableProperty]
    private CajaDto? _selectedCaja;

    [ObservableProperty]
    private ObservableCollection<ClienteDto> _clientes = new();

    [ObservableProperty]
    private ClienteDto? _selectedCliente;

    [ObservableProperty]
    private TipoMovimiento _tipoMovimiento = TipoMovimiento.Ingreso;

    [ObservableProperty]
    private decimal _monto;

    [ObservableProperty]
    private string _descripcion = string.Empty;

    [ObservableProperty]
    private string _categoria = string.Empty;

    [ObservableProperty]
    private string _referencia = string.Empty;

    [ObservableProperty]
    private DateTime _fechaInicio = DateTime.Today.AddDays(-7);

    [ObservableProperty]
    private DateTime _fechaFin = DateTime.Today;

    [ObservableProperty]
    private decimal _saldoActual;

    [ObservableProperty]
    private bool _dialogoCrearVisible;

    [ObservableProperty]
    private bool _dialogoEditarVisible;

    [ObservableProperty]
    private bool _dialogoConfirmarVisible;

    private int _editingId;

    public MovimientoViewModel(
        IMovimientoService movimientoService,
        ICajaService cajaService,
        IClienteService clienteService)
    {
        _movimientoService = movimientoService;
        _cajaService = cajaService;
        _clienteService = clienteService;
    }

    public async Task LoadAsync()
    {
        IsBusy = true;
        ClearMessages();

        try
        {
            // Cargar cajas
            var cajasResult = await _cajaService.GetAllAsync();
            if (cajasResult.IsSuccess)
            {
                Cajas = new ObservableCollection<CajaDto>(cajasResult.Value);
                SelectedCaja = Cajas.FirstOrDefault(c => c.Estado == EstadoCaja.Abierta);
            }

            // Cargar clientes
            var clientesResult = await _clienteService.GetAllAsync();
            if (clientesResult.IsSuccess)
            {
                Clientes = new ObservableCollection<ClienteDto>(clientesResult.Value);
            }

            // Cargar movimientos
            await LoadMovimientosAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMovimientosAsync()
    {
        try
        {
            Result<IReadOnlyList<MovimientoCajaDto>> result;
            
            if (SelectedCaja is not null)
            {
                result = await _movimientoService.GetByCajaAsync(SelectedCaja.Id);
            }
            else
            {
                result = await _movimientoService.GetByFechaAsync(FechaInicio, FechaFin);
            }

            if (result.IsSuccess)
            {
                Movimientos = new ObservableCollection<MovimientoCajaDto>(result.Value);
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al cargar movimientos");
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    [RelayCommand]
    private async Task CajaSelectionChangedAsync()
    {
        if (SelectedCaja is not null)
        {
            await LoadMovimientosAsync();
            var saldoResult = await _cajaService.GetByIdAsync(SelectedCaja.Id);
            if (saldoResult.IsSuccess)
            {
                SaldoActual = saldoResult.Value.SaldoActual;
            }
        }
    }

    [RelayCommand]
    private async Task BuscarPorFechaAsync()
    {
        SelectedCaja = null;
        await LoadMovimientosAsync();
    }

    [RelayCommand]
    private void ShowCreateDialog()
    {
        var cajaAbierta = Cajas.FirstOrDefault(c => c.Estado == EstadoCaja.Abierta);
        if (cajaAbierta is null)
        {
            ShowError("No hay ninguna caja abierta. Abra una caja primero.");
            return;
        }

        DialogoCrearVisible = true;
        SelectedCaja = cajaAbierta;
        TipoMovimiento = TipoMovimiento.Ingreso;
        Monto = 0;
        Descripcion = string.Empty;
        Categoria = string.Empty;
        Referencia = string.Empty;
        SelectedCliente = null;
    }

    [RelayCommand]
    private void ShowEditDialog()
    {
        if (SelectedMovimiento is null) return;

        DialogoEditarVisible = true;
        _editingId = SelectedMovimiento.Id;
        Descripcion = SelectedMovimiento.Descripcion;
        Categoria = SelectedMovimiento.Categoria ?? string.Empty;
        Referencia = SelectedMovimiento.Referencia ?? string.Empty;
    }

    [RelayCommand]
    private void ShowDeleteDialog()
    {
        if (SelectedMovimiento is null) return;
        DialogoConfirmarVisible = true;
    }

    [RelayCommand]
    private void CloseDialogs()
    {
        DialogoCrearVisible = false;
        DialogoEditarVisible = false;
        DialogoConfirmarVisible = false;
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        if (SelectedCaja is null)
        {
            ShowError("Seleccione una caja");
            return;
        }

        if (Monto <= 0)
        {
            ShowError("El monto debe ser mayor a cero");
            return;
        }

        if (string.IsNullOrWhiteSpace(Descripcion))
        {
            ShowError("La descripción es obligatoria");
            return;
        }

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _movimientoService.CreateAsync(
                new CreateMovimientoDto(
                    SelectedCaja.Id,
                    TipoMovimiento,
                    Monto,
                    Descripcion,
                    string.IsNullOrWhiteSpace(Categoria) ? null : Categoria,
                    SelectedCliente?.Id,
                    string.IsNullOrWhiteSpace(Referencia) ? null : Referencia));

            if (result.IsSuccess)
            {
                Movimientos.Insert(0, result.Value);
                SaldoActual = result.Value.Tipo == TipoMovimiento.Ingreso 
                    ? SaldoActual + result.Value.Monto 
                    : SaldoActual - result.Value.Monto;
                CloseDialogs();
                ShowSuccess("Movimiento registrado correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al registrar movimiento");
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UpdateAsync()
    {
        if (string.IsNullOrWhiteSpace(Descripcion))
        {
            ShowError("La descripción es obligatoria");
            return;
        }

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _movimientoService.UpdateAsync(
                new UpdateMovimientoDto(
                    _editingId,
                    Descripcion,
                    string.IsNullOrWhiteSpace(Categoria) ? null : Categoria,
                    string.IsNullOrWhiteSpace(Referencia) ? null : Referencia));

            if (result.IsSuccess)
            {
                var index = Movimientos.IndexOf(Movimientos.FirstOrDefault(m => m.Id == _editingId)!);
                if (index >= 0)
                {
                    Movimientos[index] = result.Value;
                    SelectedMovimiento = result.Value;
                }
                CloseDialogs();
                ShowSuccess("Movimiento actualizado correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al actualizar movimiento");
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedMovimiento is null) return;

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _movimientoService.DeleteAsync(SelectedMovimiento.Id);

            if (result.IsSuccess)
            {
                Movimientos.Remove(SelectedMovimiento);
                SelectedMovimiento = null;
                CloseDialogs();
                ShowSuccess("Movimiento eliminado correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al eliminar movimiento");
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public string GetTipoText(TipoMovimiento tipo) => tipo switch
    {
        TipoMovimiento.Ingreso => "Ingreso",
        TipoMovimiento.Egreso => "Egreso",
        _ => "Desconocido"
    };
}
