using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CajaApp.Application.DTOs.Caja;
using CajaApp.Application.Interfaces;
using CajaApp.Domain.Enums;
using System.Collections.ObjectModel;

namespace CajaApp.UI.ViewModels;

public partial class CajaViewModel : BaseViewModel
{
    private readonly ICajaService _cajaService;

    [ObservableProperty]
    private ObservableCollection<CajaDto> _cajas = new();

    [ObservableProperty]
    private CajaDto? _selectedCaja;

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private string _descripcion = string.Empty;

    [ObservableProperty]
    private decimal _saldoInicial;

    [ObservableProperty]
    private bool _dialogoCrearVisible;

    [ObservableProperty]
    private bool _dialogoEditarVisible;

    [ObservableProperty]
    private bool _dialogoConfirmarVisible;

    [ObservableProperty]
    private bool _dialogoAbrirVisible;

    [ObservableProperty]
    private decimal _saldoApertura;

    private int _editingId;

    public CajaViewModel(ICajaService cajaService) => _cajaService = cajaService;

    public async Task LoadAsync()
    {
        IsBusy = true;
        ClearMessages();
        
        try
        {
            var result = await _cajaService.GetAllAsync();
            if (result.IsSuccess)
            {
                Cajas = new ObservableCollection<CajaDto>(result.Value);
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al cargar cajas");
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
    private void ShowCreateDialog()
    {
        DialogoCrearVisible = true;
        Nombre = string.Empty;
        Descripcion = string.Empty;
        SaldoInicial = 0;
    }

    [RelayCommand]
    private void ShowEditDialog()
    {
        if (SelectedCaja is null) return;

        DialogoEditarVisible = true;
        _editingId = SelectedCaja.Id;
        Nombre = SelectedCaja.Nombre;
        Descripcion = SelectedCaja.Descripcion;
    }

    [RelayCommand]
    private void ShowDeleteDialog()
    {
        if (SelectedCaja is null) return;
        DialogoConfirmarVisible = true;
    }

    [RelayCommand]
    private void ShowAbrirDialog()
    {
        if (SelectedCaja is null) return;
        DialogoAbrirVisible = true;
        SaldoApertura = 0;
    }

    [RelayCommand]
    private void CloseDialogs()
    {
        DialogoCrearVisible = false;
        DialogoEditarVisible = false;
        DialogoConfirmarVisible = false;
        DialogoAbrirVisible = false;
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            ShowError("El nombre es obligatorio");
            return;
        }

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _cajaService.CreateAsync(
                new CreateCajaDto(Nombre, Descripcion, SaldoInicial));

            if (result.IsSuccess)
            {
                Cajas.Add(result.Value);
                CloseDialogs();
                ShowSuccess("Caja creada correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al crear caja");
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
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            ShowError("El nombre es obligatorio");
            return;
        }

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _cajaService.UpdateAsync(
                new UpdateCajaDto(_editingId, Nombre, Descripcion));

            if (result.IsSuccess)
            {
                var index = Cajas.IndexOf(Cajas.FirstOrDefault(c => c.Id == _editingId)!);
                if (index >= 0)
                {
                    Cajas[index] = result.Value;
                    SelectedCaja = result.Value;
                }
                CloseDialogs();
                ShowSuccess("Caja actualizada correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al actualizar caja");
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
        if (SelectedCaja is null) return;

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _cajaService.DeleteAsync(SelectedCaja.Id);

            if (result.IsSuccess)
            {
                Cajas.Remove(SelectedCaja);
                SelectedCaja = null;
                CloseDialogs();
                ShowSuccess("Caja eliminada correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al eliminar caja");
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
    private async Task AbrirAsync()
    {
        if (SelectedCaja is null) return;

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _cajaService.AbrirCajaAsync(SelectedCaja.Id, SaldoApertura);

            if (result.IsSuccess)
            {
                var index = Cajas.IndexOf(Cajas.FirstOrDefault(c => c.Id == SelectedCaja.Id)!);
                if (index >= 0)
                {
                    Cajas[index] = result.Value;
                    SelectedCaja = result.Value;
                }
                CloseDialogs();
                ShowSuccess("Caja abierta correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al abrir caja");
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
    private async Task CerrarAsync()
    {
        if (SelectedCaja is null) return;

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _cajaService.CerrarCajaAsync(SelectedCaja.Id);

            if (result.IsSuccess)
            {
                var index = Cajas.IndexOf(Cajas.FirstOrDefault(c => c.Id == SelectedCaja.Id)!);
                if (index >= 0)
                {
                    Cajas[index] = result.Value;
                    SelectedCaja = result.Value;
                }
                ShowSuccess("Caja cerrada correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al cerrar caja");
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

    public string GetEstadoText(EstadoCaja estado) => estado switch
    {
        EstadoCaja.Abierta => "Abierta",
        EstadoCaja.Cerrada => "Cerrada",
        _ => "Desconocido"
    };
}
