using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Comfort.Services;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Comfort;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    public App()
    {
        ConfigureLogging();
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Console()
            .WriteTo.File("logs/app-.log", 
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Debug()
            .CreateLogger();

        Log.Information("Приложение запущено");
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Регистрация сервисов
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddLogging(builder =>
        {
            builder.AddSerilog(dispose: true);
        });
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Приложение завершает работу");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}

