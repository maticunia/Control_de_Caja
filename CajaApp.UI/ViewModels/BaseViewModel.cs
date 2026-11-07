using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CajaApp.UI.ViewModels;

public abstract class BaseViewModel : ObservableObject
{
    private bool _isBusy;
    private string? _errorMessage;
    private string? _successMessage;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public string? SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    protected void ClearMessages()
    {
        ErrorMessage = null;
        SuccessMessage = null;
    }

    protected void ShowError(string message)
    {
        ErrorMessage = message;
        SuccessMessage = null;
    }

    protected void ShowSuccess(string message)
    {
        SuccessMessage = message;
        ErrorMessage = null;
    }
}
