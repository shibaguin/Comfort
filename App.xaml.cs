using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Comfort.Services;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System.IO;

namespace Comfort;

// Основной класс приложения WPF
public partial class App : Application
{
    // Провайдер сервисов для внедрения зависимостей
    private ServiceProvider _serviceProvider = null!;
    private IErrorHandlingService _errorHandling = null!;

    // Инициализация приложения
    public App()
    {
        try
        {
            // Настройка логирования для логирования ошибок
            ConfigureLogging();
            
            // Инициализация контейнера зависимостей
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            _errorHandling = _serviceProvider.GetRequiredService<IErrorHandlingService>();
        }
        catch (Exception ex)
        {
            // Логируем критическую ошибку и показываем сообщение пользователю
            Log.Fatal(ex, "Критическая ошибка при инициализации приложения");
            MessageBox.Show(
                $"Произошла критическая ошибка при запуске приложения:\n{ex.Message}",
                "Ошибка инициализации",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    // Настройка логирования Serilog
    private void ConfigureLogging()
    {
        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logPath);

            Log.Logger = new LoggerConfiguration()
                // Устанавливаем минимальный уровень логирования
                .MinimumLevel.Debug()
                // Добавляем вывод в консоль
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                // Добавляем запись в файл с ежедневной ротацией
                .WriteTo.File(
                    Path.Combine(logPath, "app-.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 31)
                .CreateLogger();

            Log.Information("Приложение запущено");
        }
        catch (Exception ex)
        {
            // При ошибке настройки логирования показываем сообщение и пробрасываем исключение дальше
            MessageBox.Show(
                $"Ошибка при настройке логирования:\n{ex.Message}",
                "Ошибка логирования",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            throw;
        }
    }

    // Регистрация сервисов
    private void ConfigureServices(IServiceCollection services)
    {
        try
        {
            // Регистрация сервисов приложения
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton(Log.Logger); // Регистрируем глобальный логгер
            services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
            
            // Настройка системы логирования
            services.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true);
            });
        }
        catch (Exception ex)
        {
            // Логируем ошибку и показываем сообщение пользователю
            Log.Error(ex, "Ошибка при настройке сервисов");
            MessageBox.Show(
                $"Ошибка при настройке сервисов:\n{ex.Message}",
                "Ошибка конфигурации",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            throw;
        }
    }

    // Обработчик запуска приложения
    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);
            
            // Регистрируем глобальные обработчики необработанных исключений
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }
        catch (Exception ex)
        {
            // При ошибке запуска логируем и показываем сообщение
            Log.Fatal(ex, "Критическая ошибка при запуске приложения");
            MessageBox.Show(
                $"Произошла критическая ошибка при запуске приложения:\n{ex.Message}",
                "Ошибка запуска",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    // Обработчик необработанных исключений в домене
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        _errorHandling.LogError(exception ?? new Exception("Неизвестная ошибка"), "Необработанное исключение в домене приложения");
    }

    // Обработчик необработанных исключений в UI потоке
    private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        _errorHandling.LogError(e.Exception, "Необработанное исключение в UI потоке");
        e.Handled = true;
    }

    // Обработчик необработанных исключений в асинхронных задачах
    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _errorHandling.LogError(e.Exception, "Необработанное исключение в задаче");
        e.SetObserved();
    }

    // Обработчик события завершения работы приложения
    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            // Логируем завершение работы и закрываем логгер
            Log.Information("Приложение завершает работу");
            Log.CloseAndFlush();
        }
        catch (Exception ex)
        {
            // При ошибке завершения показываем предупреждение
            MessageBox.Show(
                $"Ошибка при завершении работы приложения:\n{ex.Message}",
                "Ошибка завершения",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        finally
        {
            // Всегда вызываем базовый метод завершения
            base.OnExit(e);
        }
    }
}

