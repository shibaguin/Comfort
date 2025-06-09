using System;
using System.Globalization;
using System.Windows.Data;

namespace Comfort.Converters
{
    // Конвертер для инвертирования булевого значения
    public class InverseBooleanConverter : IValueConverter
    {
        // Инвертирует булево значение: true -> false, false -> true
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }

        // Обратное преобразование также инвертирует значение
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }
} 