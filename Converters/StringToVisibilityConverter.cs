using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Comfort.Converters
{
    // Конвертер для преобразования строки в видимость элемента
    public class StringToVisibilityConverter : IValueConverter
    {
        // Преобразует строку в Visibility: Visible если строка не пустая, иначе Collapsed
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        // Обратное преобразование не реализовано
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 