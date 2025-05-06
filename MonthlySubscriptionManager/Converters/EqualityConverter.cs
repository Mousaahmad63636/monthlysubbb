// MonthlySubscriptionManager/Converters/EqualityConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace MonthlySubscriptionManager.WPF.Converters
{
    public class EqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && parameter != null && value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return parameter;
            }
            return Binding.DoNothing;
        }
    }
}