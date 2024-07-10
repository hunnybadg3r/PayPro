using PayPro.Main.Local.Models;
using PayPro.Main.Local.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace PayPro.Main.Themes.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            var collectionView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            collectionView.SortDescriptions.Clear();
            collectionView.SortDescriptions.Add(new SortDescription("Timestamp", ListSortDirection.Descending));
        }
    }
}
