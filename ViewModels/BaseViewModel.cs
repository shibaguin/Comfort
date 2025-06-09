using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Comfort.ViewModels
{
    // Базовый класс для всех ViewModel, реализующий интерфейс INotifyPropertyChanged для поддержки привязки данных
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }
    }
} 