using System.Windows;
using System.Windows.Controls;

namespace PayPro.Main.Controls.Units
{
    public class PaymentRecordDataGridRow : DataGridRow
    {
        static PaymentRecordDataGridRow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PaymentRecordDataGridRow),
                new FrameworkPropertyMetadata(typeof(PaymentRecordDataGridRow)));
        }
    }
}
