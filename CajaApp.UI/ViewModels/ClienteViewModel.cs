using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CajaApp.Application.DTOs.Cliente;
using CajaApp.Application.Interfaces;
using System.Collections.ObjectModel;

namespace CajaApp.UI.ViewModels;

public partial class ClienteViewModel : BaseViewModel
{
    private readonly IClienteService _clienteService;

    [ObservableProperty]
    private ObservableCollection<ClienteDto> _clientes = new();

    [ObservableProperty]
    private ClienteDto? _selectedCliente;

    [ObservableProperty]
    private string _nombre = string.Empty;

    [ObservableProperty]
    private string _apellidos = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _telefono = string.Empty;

    [ObservableProperty]
    private string _direccion = string.Empty;

    [ObservableProperty]
    private string _busquedaNombre = string.Empty;

    [ObservableProperty]
    private string _busquedaEmail = string.Empty;

    [ObservableProperty]
    private bool _dialogoCrearVisible;

    [ObservableProperty]
    private bool _dialogoEditarVisible;

    [ObservableProperty]
    private bool _dialogoConfirmarVisible;

    private int _editingId;

    public ClienteViewModel(IClienteService clienteService) => _clienteService = clienteService;

    public async Task LoadAsync()
    {
        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _clienteService.GetAllAsync();
            if (result.IsSuccess)
            {
                Clientes = new ObservableCollection<ClienteDto>(result.Value);
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al cargar clientes");
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
    private async Task BuscarAsync()
    {
        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _clienteService.SearchAsync(
                string.IsNullOrWhiteSpace(BusquedaNombre) ? null : BusquedaNombre,
                string.IsNullOrWhiteSpace(BusquedaEmail) ? null : BusquedaEmail);

            if (result.IsSuccess)
            {
                Clientes = new ObservableCollection<ClienteDto>(result.Value);
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al buscar clientes");
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
        Apellidos = string.Empty;
        Email = string.Empty;
        Telefono = string.Empty;
        Direccion = string.Empty;
    }

    [RelayCommand]
    private void ShowEditDialog()
    {
        if (SelectedCliente is null) return;

        DialogoEditarVisible = true;
        _editingId = SelectedCliente.Id;
        Nombre = SelectedCliente.Nombre;
        Apellidos = SelectedCliente.Apellidos;
        Email = SelectedCliente.Email ?? string.Empty;
        Telefono = SelectedCliente.Telefono ?? string.Empty;
        Direccion = SelectedCliente.Direccion ?? string.Empty;
    }

    [RelayCommand]
    private void ShowDeleteDialog()
    {
        if (SelectedCliente is null) return;
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
        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellidos))
        {
            ShowError("Nombre y apellidos son obligatorios");
            return;
        }

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _clienteService.CreateAsync(
                new CreateClienteDto(
                    Nombre,
                    Apellidos,
                    string.IsNullOrWhiteSpace(Email) ? null : Email,
                    string.IsNullOrWhiteSpace(Telefono) ? null : Telefono,
                    string.IsNullOrWhiteSpace(Direccion) ? null : Direccion));

            if (result.IsSuccess)
            {
                Clientes.Add(result.Value);
                CloseDialogs();
                ShowSuccess("Cliente creado correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al crear cliente");
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
        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellidos))
        {
            ShowError("Nombre y apellidos son obligatorios");
            return;
        }

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _clienteService.UpdateAsync(
                new UpdateClienteDto(
                    _editingId,
                    Nombre,
                    Apellidos,
                    string.IsNullOrWhiteSpace(Email) ? null : Email,
                    string.IsNullOrWhiteSpace(Telefono) ? null : Telefono,
                    string.IsNullOrWhiteSpace(Direccion) ? null : Direccion));

            if (result.IsSuccess)
            {
                var index = Clientes.IndexOf(Clientes.FirstOrDefault(c => c.Id == _editingId)!);
                if (index >= 0)
                {
                    Clientes[index] = result.Value;
                    SelectedCliente = result.Value;
                }
                CloseDialogs();
                ShowSuccess("Cliente actualizado correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al actualizar cliente");
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
        if (SelectedCliente is null) return;

        IsBusy = true;
        ClearMessages();

        try
        {
            var result = await _clienteService.DeleteAsync(SelectedCliente.Id);

            if (result.IsSuccess)
            {
                Clientes.Remove(SelectedCliente);
                SelectedCliente = null;
                CloseDialogs();
                ShowSuccess("Cliente eliminado correctamente");
            }
            else
            {
                ShowError(result.Error?.Message ?? "Error al eliminar cliente");
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
