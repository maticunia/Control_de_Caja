using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CajaApp.Application.DTOs.Caja;
using CajaApp.Application.Interfaces;
using System.Collections.ObjectModel;

namespace CajaApp.UI.ViewModels;

public partial class AbrirCajaViewModel : BaseViewModel
{
    private readonly ICajaService _cajaService;

    [ObservableProperty]
    private ObservableCollection<CajaDto> _cajas = new();

    [ObservableProperty]
    private CajaDto? _selectedCaja;

    [ObservableProperty]
    private decimal _saldoInicial;

    public AbrirCajaViewModel(ICajaService cajaService) => _cajaService = cajaService;

    public async Task LoadAsync()
    {
        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _cajaService.GetAllAsync();
            if (result.IsSuccess)
            {
                Cajas = new ObservableCollection<CajaDto>(
                    result.Value.Where(c => c.Estado == Domain.Enums.EstadoCaja.Cerrada));
                
                if (Cajas.Count > 0)
                    SelectedCaja = Cajas[0];
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
    private async Task AbrirCajaAsync()
    {
        if (SelectedCaja is null)
        {
            ShowError("Seleccione una caja");
            return;
        }

        if (SaldoInicial < 0)
        {
            ShowError("El saldo inicial no puede ser negativo");
            return;
        }

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _cajaService.AbrirCajaAsync(SelectedCaja.Id, SaldoInicial);

            if (result.IsSuccess)
            {
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
}
