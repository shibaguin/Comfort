using System.Windows;
using Serilog;

namespace Comfort.Services;

public interface IErrorHandlingService
{
    void ShowError(string message, string title = "Ошибка");
    void ShowWarning(string message, string title = "Предупреждение");
    void ShowInfo(string message, string title = "Информация");
    void LogError(Exception ex, string message);
    void LogWarning(Exception ex, string message);
    void LogInfo(string message);
}

public class ErrorHandlingService : IErrorHandlingService
{
    private readonly ILogger _logger;

    public ErrorHandlingService(ILogger logger)
    {
        _logger = logger;
    }

    public void ShowError(string message, string title = "Ошибка")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowWarning(string message, string title = "Предупреждение")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public void ShowInfo(string message, string title = "Информация")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void LogError(Exception ex, string message)
    {
        _logger.Error(ex, message);
        ShowError($"{message}\n\n{ex.Message}");
    }

    public void LogWarning(Exception ex, string message)
    {
        _logger.Warning(ex, message);
        ShowWarning($"{message}\n\n{ex.Message}");
    }

    public void LogInfo(string message)
    {
        _logger.Information(message);
    }
} 