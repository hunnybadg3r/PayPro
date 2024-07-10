using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PayPro.Main.Local.Converters
{
    public class DateTimeShortYearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("yy-MM-dd HH:mm:ss");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && DateTime.TryParseExact(str, "yy-MM-dd HH:mm:ss", culture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
