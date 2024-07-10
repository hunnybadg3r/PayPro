using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace MockPOS.Main.Local.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && DateTime.TryParseExact(str, "yyyy-MM-dd HH:mm:ss", culture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
