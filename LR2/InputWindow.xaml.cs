using System.Windows;

namespace LR2
{
    public partial class InputWindow : Window
    {
        public InputWindow(string promptMessage, string defaultInput)
        {
            InitializeComponent();
            txtPrompt.Text = promptMessage;
            txtInput.Text = defaultInput;
        }

        public int Count { get; private set; }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtInput.Text, out int result) && result > 0)
            {
                Count = result;
                DialogResult = true;
            }
            else MessageBox.Show("Введіть ціле додатне число!"); 
        }
    }
}
