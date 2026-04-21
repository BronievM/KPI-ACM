using LR3.Models;
using LR3.Models.Functions;
using LR3.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;

namespace LR3
{
    public partial class MainWindow : Window
    {
        private readonly InterpolationAnalyzer _analyzer;
        private IFunction _currentFunction;
        public MainWindow()
        {
            InitializeComponent();
            _analyzer = new InterpolationAnalyzer();
        }
        private void RunVariantFunction(object sender, RoutedEventArgs e)
        {
            var inputWin = new InputWindow("Введіть кількість інтервалів (m):", "10") { Owner = this };
            if (inputWin.ShowDialog() == true && int.TryParse(inputWin.InputText, out int m) && m > 3)
            {
                _currentFunction = new CustomFunction(m);
                ExecuteAnalysis(updateLog: true);
            }
            else MessageBox.Show("Некоректне значення m. Потрібно ввести ціле число більше 3.", "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
           
        }

        private void RunTestFunction(object sender, RoutedEventArgs e)
        {
            var inputWin = new InputWindow("Введіть кількість інтервалів (m):", "20") { Owner = this };
            if (inputWin.ShowDialog() == true && int.TryParse(inputWin.InputText, out int m) && m > 3)
            {
                _currentFunction = new TestFunction(m);
                ExecuteAnalysis(updateLog: true);
            }
            else MessageBox.Show("Некоректне значення m. Потрібно ввести ціле число більше 3.", "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void EvaluatePoint(object sender, RoutedEventArgs e)
        {
            var inputWin = new InputWindow("Введіть значення x:", "3,5") { Owner = this };
            if (inputWin.ShowDialog() == true && double.TryParse(inputWin.InputText, out double x))
            {
                string result = _analyzer.EvaluateAtPoint(x);
                txtLog.AppendText(result);
                txtLog.ScrollToEnd();
            }
        }

        private void OnGraphModeMenuClick(object sender, RoutedEventArgs e)
        {
            var clickedItem = sender as System.Windows.Controls.MenuItem;

            if (clickedItem == mnuModeInterpolation)
            {
                mnuModeInterpolation.IsChecked = true;
                mnuModeError.IsChecked = false;
            }
            else if (clickedItem == mnuModeError)
            {
                mnuModeInterpolation.IsChecked = false;
                mnuModeError.IsChecked = true;
            }

            if (_currentFunction != null && plotView != null)
            {
                ExecuteAnalysis(updateLog: false);
            }
        }

        private void ExecuteAnalysis(bool updateLog)
        {
            if (_currentFunction == null) return;

            bool isErrorMode = mnuModeError.IsChecked == true;
            AnalysisResult result = _analyzer.RunExperiment(_currentFunction, isErrorMode);

            plotView.Model = result.GraphModel;
            plotView.InvalidatePlot(true);

            if (updateLog)
            {
                txtLog.Clear();
                foreach (var line in result.LogLines)
                {
                    txtLog.AppendText(line + "\n");
                }
                txtLog.ScrollToEnd();
            }
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Text Files (*.txt)|*.txt" };
            if (dialog.ShowDialog() == true)
            {
                try { DataHelper.SaveLog(dialog.FileName, txtLog.Text); MessageBox.Show("Збережено успішно!"); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void ExitApp(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }
    }
}
