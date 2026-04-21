using LR5.Models;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace LR5
{
    public partial class InputWindow : Window
    {
        private readonly MatrixSystem _config;
        private int _currentN = 0;
        private TextBox[,] _textMatrixA;
        private TextBox[] _textVectorB;

        public InputWindow(MatrixSystem config)
        {
            InitializeComponent();
            _config = config;

            int n = _config.B.Length;
            GenerateMatrixUI(n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    _textMatrixA[i, j].Text = _config.A[i, j].ToString();
                }
                _textVectorB[i].Text = _config.B[i].ToString();
            }
            txtEpsilon.Text = _config.Epsilon.ToString();
        }

        private void BtnGenerateMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtSize.Text, out int n) && n >= 2 && n <= 20) GenerateMatrixUI(n);
            else MessageBox.Show("Введіть коректне ціле число N (від 2 до 20).", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void GenerateMatrixUI(int n)
        {
            _currentN = n;
            txtSize.Text = n.ToString();
            MatrixGrid.Children.Clear();
            MatrixGrid.RowDefinitions.Clear();
            MatrixGrid.ColumnDefinitions.Clear();

            _textMatrixA = new TextBox[n, n];
            _textVectorB = new TextBox[n];

            for (int j = 0; j < n; j++) MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });
            MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });

            for (int i = 0; i < n; i++)
            {
                MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });

                for (int j = 0; j < n; j++)
                {
                    var txtA = new TextBox { Margin = new Thickness(2), VerticalContentAlignment = VerticalAlignment.Center, Text = (i == j) ? "10" : "1" };
                    Grid.SetRow(txtA, i); Grid.SetColumn(txtA, j);
                    MatrixGrid.Children.Add(txtA);
                    _textMatrixA[i, j] = txtA;
                }

                var separator = new TextBlock { Text = "|", FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetRow(separator, i); Grid.SetColumn(separator, n);
                MatrixGrid.Children.Add(separator);

                var txtB = new TextBox { Margin = new Thickness(2), VerticalContentAlignment = VerticalAlignment.Center, Text = (i + 1).ToString() };
                Grid.SetRow(txtB, i); Grid.SetColumn(txtB, n + 1);
                MatrixGrid.Children.Add(txtB);
                _textVectorB[i] = txtB;
            }
        }

        private void BtnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Текстові файли (*.txt;*.csv)|*.txt;*.csv|Всі файли (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string fileContent = File.ReadAllText(openFileDialog.FileName);
                    var values = Regex.Split(fileContent.Trim(), @"\s+").Where(v => !string.IsNullOrWhiteSpace(v)).ToArray();
                    int totalElements = values.Length;
                    int calculatedN = (int)(-1 + Math.Sqrt(1 + 4 * totalElements)) / 2;

                    if (calculatedN * (calculatedN + 1) != totalElements)
                    {
                        MessageBox.Show("Кількість елементів у файлі не утворює коректну матрицю N x N+1.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (_currentN != calculatedN) GenerateMatrixUI(calculatedN);

                    int idx = 0;
                    for (int i = 0; i < calculatedN; i++)
                    {
                        for (int j = 0; j < calculatedN; j++) _textMatrixA[i, j].Text = values[idx++];
                        _textVectorB[i].Text = values[idx++];
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка читання файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (_currentN == 0) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Текстові файли (*.txt)|*.txt", DefaultExt = ".txt" };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                    {
                        for (int i = 0; i < _currentN; i++)
                        {
                            string line = "";
                            for (int j = 0; j < _currentN; j++)
                            {
                                line += _textMatrixA[i, j].Text + "\t";
                            }
                            line += _textVectorB[i].Text;
                            sw.WriteLine(line.Trim());
                        }
                    }
                    MessageBox.Show("Матрицю успішно збережено у файл.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка збереження файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentN == 0) return;
                double ParseVal(string input) => double.Parse(input.Replace('.', ','));

                _config.A = new double[_currentN, _currentN];
                _config.B = new double[_currentN];

                for (int i = 0; i < _currentN; i++)
                {
                    for (int j = 0; j < _currentN; j++) _config.A[i, j] = ParseVal(_textMatrixA[i, j].Text);
                    _config.B[i] = ParseVal(_textVectorB[i].Text);
                }

                _config.Epsilon = ParseVal(txtEpsilon.Text);
                DialogResult = true;
                Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Помилка вводу: перевірте правильність числових значень у матриці.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}