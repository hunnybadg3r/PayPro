using System.Globalization;
using System.Windows.Data;

namespace MockPOS.Main.Local.Converters;

public class PaymentMethodsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "CREDIT_CARD" => "카드",
            "DIGITAL_WALLET" => "간편",
            _ => value
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "카드" => "CREDIT_CARD",
            "간편" => "DIGITAL_WALLET",
            _ => value
        };
    }
}