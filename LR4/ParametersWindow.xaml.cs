using System.Windows;

namespace LR4
{
    public partial class ParametersWindow : Window
    {
        public double A { get; private set; }
        public double B { get; private set; }
        public double Epsilon { get; private set; }

        public ParametersWindow(string defaultA, string defaultB, string defaultEps)
        {
            InitializeComponent();
            txtA.Text = defaultA;
            txtB.Text = defaultB;
            txtEpsilon.Text = defaultEps;
        }

        private void BtnCompute_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtA.Text.Replace('.', ','), out double a) &&
                double.TryParse(txtB.Text.Replace('.', ','), out double b) &&
                double.TryParse(txtEpsilon.Text.Replace('.', ','), out double eps))
            {
                if (a >= b)
                {
                    MessageBox.Show("Межа A повинна бути меншою за B!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (eps <= 0)
                {
                    MessageBox.Show("Точність повинна бути додатним числом!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                A = a;
                B = b;
                Epsilon = eps;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Некоректний формат чисел!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
