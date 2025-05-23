using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SubscriptionManager.Helpers  // Changed from SimpleSubscriptionManager.Helpers
{
    public static class Converters
    {
        public static readonly IValueConverter BoolToVisibilityConverter = new BoolToVisibilityValueConverter();
        public static readonly IValueConverter IsNotNullConverter = new IsNotNullValueConverter();
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
}