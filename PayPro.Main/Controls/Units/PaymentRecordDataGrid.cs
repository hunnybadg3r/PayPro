using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PayPro.Main.Controls.Units
{
    public class PaymentRecordDataGrid : DataGrid
    {
        static PaymentRecordDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PaymentRecordDataGrid),
                new FrameworkPropertyMetadata(typeof(PaymentRecordDataGrid)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PaymentRecordDataGridRow();
        }

        public PaymentRecordDataGrid()
        {
            AutoGenerateColumns = false;
            CanUserSortColumns = true;

            double totalAvailableWidth = ActualWidth;
            double[] columnWidths = [1.5, 2, 1, 1, 1, 1, 1];
            double totalRatio = columnWidths.Sum();

            Columns.Add(new DataGridTextColumn { Header = "거래ID", Binding = new Binding("TransactionId") { NotifyOnSourceUpdated = true }, Width = new DataGridLength(columnWidths[0] / totalRatio, DataGridLengthUnitType.Star) });
            Columns.Add(new DataGridTextColumn { Header = "거래 시간", Binding = new Binding("Timestamp") { NotifyOnSourceUpdated = true }, Width = new DataGridLength(columnWidths[1] / totalRatio, DataGridLengthUnitType.Star) });
            Columns.Add(new DataGridTextColumn { Header = "유종", Binding = new Binding("FuelType"), Width = new DataGridLength(columnWidths[2] / totalRatio, DataGridLengthUnitType.Star) });
            Columns.Add(new DataGridTextColumn { Header = "액수", Binding = new Binding("Amount"), Width = new DataGridLength(columnWidths[3] / totalRatio, DataGridLengthUnitType.Star) });
            Columns.Add(new DataGridTextColumn { Header = "리터", Binding = new Binding("Liters"), Width = new DataGridLength(columnWidths[4] / totalRatio, DataGridLengthUnitType.Star) });
            Columns.Add(new DataGridTextColumn { Header = "결제 방식", Binding = new Binding("PaymentMethod"), Width = new DataGridLength(columnWidths[5] / totalRatio, DataGridLengthUnitType.Star) });
            Columns.Add(new DataGridTextColumn { Header = "상태", Binding = new Binding("Status") { NotifyOnSourceUpdated = true }, Width = new DataGridLength(columnWidths[6] / totalRatio, DataGridLengthUnitType.Star) });
        }
    }
}
