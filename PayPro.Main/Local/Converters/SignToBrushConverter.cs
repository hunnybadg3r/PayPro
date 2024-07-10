using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PayPro.Main.Local.Converters
{
    public class SignToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sign)
            {
                if (sign.StartsWith("+"))
                {
                    return Brushes.Red; // Red for positive growth
                }
                else if (sign.StartsWith("-"))
                {
                    return Brushes.Blue; // Blue for negative growth
                }
            }

            return Brushes.Black; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
