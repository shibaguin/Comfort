using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Comfort.Converters
{
    // Конвертер для преобразования числового значения в видимость элемента
    public class NullOrNegativeToVisibilityConverter : IValueConverter
    {
        // Преобразует значение в Visibility: Visible если значение >= 0, иначе Collapsed
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue && intValue >= 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        // Обратное преобразование не реализовано
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 