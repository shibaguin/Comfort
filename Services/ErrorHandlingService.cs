using System.Windows;
using Serilog;

namespace Comfort.Services;

// Интерфейс сервиса для обработки ошибок и логирования
public interface IErrorHandlingService
{
    // Отображение сообщения об ошибке пользователю
    void ShowError(string message, string title = "Ошибка");
    // Отображение предупреждения пользователю
    void ShowWarning(string message, string title = "Предупреждение");
    // Отображение информационного сообщения пользователю
    void ShowInfo(string message, string title = "Информация");
    // Логирование ошибки и отображение сообщения пользователю
    void LogError(Exception ex, string message);
    // Логирование предупреждения и отображение сообщения пользователю
    void LogWarning(Exception ex, string message);
    // Логирование информационного сообщения
    void LogInfo(string message);
}

// Реализация сервиса для обработки ошибок и логирования
public class ErrorHandlingService : IErrorHandlingService
{
    private readonly ILogger _logger;

    public ErrorHandlingService(ILogger logger)
    {
        _logger = logger;
    }

    // Отображение диалогового окна с сообщением об ошибке
    public void ShowError(string message, string title = "Ошибка")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    // Отображение диалогового окна с предупреждением
    public void ShowWarning(string message, string title = "Предупреждение")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    // Отображение диалогового окна с информационным сообщением
    public void ShowInfo(string message, string title = "Информация")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // Логирование ошибки и отображение сообщения пользователю
    public void LogError(Exception ex, string message)
    {
        _logger.Error(ex, message);
        ShowError($"{message}\n\n{ex.Message}");
    }

    // Логирование предупреждения и отображение сообщения пользователю
    public void LogWarning(Exception ex, string message)
    {
        _logger.Warning(ex, message);
        ShowWarning($"{message}\n\n{ex.Message}");
    }

    // Логирование информационного сообщения без отображения пользователю
    public void LogInfo(string message)
    {
        _logger.Information(message);
    }
} 