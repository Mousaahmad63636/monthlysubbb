// MonthlySubscriptionManager/Converters/DecimalConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace MonthlySubscriptionManager.WPF.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("N2", culture);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (decimal.TryParse(stringValue, NumberStyles.Any, culture, out decimal result))
                {
                    return result;
                }
            }
            return 0m;
        }
    }
}