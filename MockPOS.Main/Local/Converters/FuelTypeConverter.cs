using System.Globalization;
using System.Windows.Data;

namespace MockPOS.Main.Local.Converters;

public class FuelTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "GASOLINE" => "휘발유",
            "DIESEL" => "경유",
            _ => value
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "휘발유" => "GASOLINE",
            "경유" => "DIESEL",
            _ => value
        };
    }
}