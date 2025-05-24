using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SubscriptionManager.Helpers
{
    public static class Converters
    {
        public static readonly IValueConverter BoolToVisibilityConverter = new BoolToVisibilityValueConverter();
        public static readonly IValueConverter InverseBoolToVisibilityConverter = new InverseBoolToVisibilityValueConverter();
        public static readonly IValueConverter IsNotNullConverter = new IsNotNullValueConverter();
        public static readonly IValueConverter DecimalConverter = new DecimalValueConverter();
        public static readonly IValueConverter InverseBoolConverter = new InverseBoolValueConverter();
        public static readonly IValueConverter MonthNumberToNameConverter = new MonthNumberToNameValueConverter();
        public static readonly IValueConverter ProfitToColorConverter = new ProfitToColorValueConverter();
    }

    public class BoolToVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }

    public class InverseBoolToVisibilityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Collapsed;
        }
    }

    public class IsNotNullValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToStringConverter : IValueConverter
    {
        public string TrueValue { get; set; } = "True";
        public string FalseValue { get; set; } = "False";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? TrueValue : FalseValue;
            return FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == TrueValue;
        }
    }

    public class DecimalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
                return decimalValue.ToString("F2", culture);
            return "0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return 0m;

            if (decimal.TryParse(value.ToString(), NumberStyles.Any, culture, out decimal result))
                return result;

            return 0m;
        }
    }

    public class InverseBoolValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }
    }

    public class MonthNumberToNameValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int monthNumber && monthNumber >= 1 && monthNumber <= 12)
                return new DateTime(2000, monthNumber, 1).ToString("MMMM", culture);
            return "Invalid Month";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string monthName)
            {
                for (int i = 1; i <= 12; i++)
                {
                    if (new DateTime(2000, i, 1).ToString("MMMM", culture).Equals(monthName, StringComparison.OrdinalIgnoreCase))
                        return i;
                }
            }
            return 1;
        }
    }

    public class ProfitToColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal profit)
            {
                if (profit > 0)
                    return new SolidColorBrush(Colors.LightGreen);
                else if (profit < 0)
                    return new SolidColorBrush(Colors.LightCoral);
                else
                    return new SolidColorBrush(Colors.LightGray);
            }
            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}