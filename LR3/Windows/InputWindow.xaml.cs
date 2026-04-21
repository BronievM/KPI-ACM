using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LR3.Windows
{
    public partial class InputWindow : Window
    {
        public string InputText { get; private set; }

        public InputWindow(string promptMessage, string defaultInput)
        {
            InitializeComponent();
            txtPrompt.Text = promptMessage;
            txtInput.Text = defaultInput;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            InputText = txtInput.Text;
            DialogResult = true;
        }
    }
}
