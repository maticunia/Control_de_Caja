using CajaApp.Application;
using CajaApp.Infrastructure;
using CajaApp.UI.ViewModels;
using CajaApp.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;

namespace CajaApp.UI;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database path
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CajaApp",
            "cajaapp.db");
        
        var dbDirectory = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dbDirectory))
            Directory.CreateDirectory(dbDirectory);

        var connectionString = $"Data Source={dbPath}";

        // Infrastructure
        services.AddInfrastructure(connectionString);
        
        // Application
        services.AddApplication();

        // UI Services
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<CajaViewModel>();
        services.AddTransient<ClienteViewModel>();
        services.AddTransient<MovimientoViewModel>();
        services.AddTransient<AbrirCajaViewModel>();

        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<CajaView>();
        services.AddTransient<ClienteView>();
        services.AddTransient<MovimientoView>();
        services.AddTransient<AbrirCajaView>();

        // Register IServiceProvider for ViewModel navigation
        services.AddSingleton<IServiceProvider>(sp => sp);
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize database
        await DatabaseConfig.InitializeDatabaseAsync(_serviceProvider);

        // Create and show main window
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}

public interface INavigationService
{
    void NavigateTo(string viewName);
    void NavigateTo(string viewName, object parameter);
    object? CurrentView { get; set; }
}

public class NavigationService : INavigationService
{
    public object? CurrentView { get; set; }

    public void NavigateTo(string viewName) => NavigateTo(viewName, null);

    public void NavigateTo(string viewName, object parameter)
    {
        // Implementation will be handled by MainViewModel
    }
}
