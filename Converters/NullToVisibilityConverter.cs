using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Comfort.Converters
{
    // Конвертер для преобразования null в видимость элемента
    public class NullToVisibilityConverter : IValueConverter
    {
        // Преобразует значение в Visibility: Visible если значение не null, иначе Collapsed
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        // Обратное преобразование не реализовано
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 