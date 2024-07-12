using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MockPOS.Main.Local.ViewModels;

namespace MockPOS.Main.Themes.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void CardNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void CardNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox currentTextBox = sender as TextBox;
            if (currentTextBox.Text.Length == currentTextBox.MaxLength)
            {
                FocusNextTextBox(currentTextBox);
            }
        }

        private void FocusNextTextBox(TextBox currentTextBox)
        {
            if (currentTextBox == CardNumber1)
                CardNumber2.Focus();
            else if (currentTextBox == CardNumber2)
                CardNumber3.Focus();
            else if (currentTextBox == CardNumber3)
                CardNumber4.Focus();
        }

        private void CardNumber4_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                (sender as UIElement)?.MoveFocus(request);

                e.Handled = true;
            }
        }

        private void IdentificationTokenTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 알파벳과 숫자만 허용
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"^[a-zA-Z0-9]");
        }

        private void IdentificationTokenTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                // 16자를 초과하는 입력을 방지
                if (textBox.Text.Length > 16)
                {
                    textBox.Text = textBox.Text.Substring(0, 16);
                    textBox.SelectionStart = 16;
                }
            }
        }
    }
}