using LR4.Models;
using LR4.Models.Equations;
using System.Windows;

namespace LR4
{
    public partial class MainWindow : Window
    {
        private readonly TangentSolver _solver;

        public MainWindow()
        {
            InitializeComponent();
            _solver = new TangentSolver();
        }

        private void RunVariantFunction(object sender, RoutedEventArgs e)
        {
            var paramWin = new ParametersWindow("1", "2", "0,0001") { Owner = this };

            if (paramWin.ShowDialog() == true)
            {
                var eq = new CustomEquation(paramWin.A, paramWin.B);
                ExecuteAnalysis(eq, paramWin.Epsilon);
            }
        }

        private void ExecuteAnalysis(IEquation eq, double epsilon)
        {
            txtLog.Clear();

            AnalysisResult result = _solver.Solve(eq, epsilon);

            plotView.Model = result.GraphModel;
            plotView.InvalidatePlot(true);

            foreach (var line in result.LogLines)
            {
                txtLog.AppendText(line + "\n");
            }
            txtLog.ScrollToEnd();
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Text Files (*.txt)|*.txt", FileName = "Report_LR4.txt" };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    DataHelper.SaveLog(dialog.FileName, txtLog.Text);
                    MessageBox.Show("Звіт успішно збережено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExitApp(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }
    }
}