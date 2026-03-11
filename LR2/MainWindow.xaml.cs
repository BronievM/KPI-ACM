using LR2.SortingLogic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;
using System.Windows.Input;

namespace LR2
{
    public partial class MainWindow : Window
    {
        private readonly SortingManager _appManager;
        public MainWindow() 
        { 
            InitializeComponent();
            _appManager = new SortingManager(new ShakerSorter());

            txtLog.AppendText($"Програма готова до роботи!\n");
            txtLog.AppendText($"Згенеруйте або завантажте масив!\n");
            txtLog.AppendText($">> Очікую вказівок \n");
            txtLog.AppendText("------------------------\n");
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Text Files (*.txt)|*.txt" };
            if (dialog.ShowDialog() == true) {
                try {
                    _appManager.SaveCurrent(dialog.FileName);
                    MessageBox.Show("Збережено успішно!");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void LoadFile(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "Text Files|*.txt" };
            if (dialog.ShowDialog() == true) {
                txtLog.AppendText($"> Відкриваю файл за шляхом: {System.IO.Path.GetFileName(dialog.FileName)}...\n");
                Mouse.OverrideCursor = Cursors.Wait;
                Application.Current.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

                try
                {
                    SortResult res = _appManager.LoadAndSort(dialog.FileName);
                    ProcessSingleArray(res, $"Дані з файлу (N={res.Data!.Length})");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"[!] ПОМИЛКА: {ex.Message}\n");
                    txtLog.ScrollToEnd();
                }
                finally { Mouse.OverrideCursor = null; }
            }
        }

        private void ProcessSingleArray(SortResult result, string title)
        {
            txtLog.AppendText($">> Відсортовано! Час: {result.TimeMs:F3} мс\n");
            txtLog.AppendText($">> Операцій: {result.Operations}\n");
            txtLog.AppendText(">> ------------------------\n");

            var model = new PlotModel { Title = title };
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Індекс елемента", IsPanEnabled = false, IsZoomEnabled = false });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Значення", IsPanEnabled = false, IsZoomEnabled = false });

            var dataSeries = new LineSeries
            {
                Title = "Відсортовані дані",
                Color = OxyColors.DodgerBlue,
                MarkerType = MarkerType.None,
                CanTrackerInterpolatePoints = false,
                TrackerFormatString = "Індекс: {2}\nЗначення: {4}"
            };

            for (int i = 0; i < result.Data!.Length; i++) { dataSeries.Points.Add(new DataPoint(i, result.Data[i])); }

            model.Series.Add(dataSeries);
            plotView.Model = model;
            plotView.InvalidatePlot();
            txtLog.ScrollToEnd();
        }

        private void RunAnalysis(object sender, RoutedEventArgs e)
        {
            InputWindow inputWin = new InputWindow("Введіть кількість тестових масивів:", "10") { Owner = this, Title = "Кількість масивів" };

            if (inputWin.ShowDialog() == true)
            {
                int arrayCount = inputWin.Count;

                txtLog.Clear();
                txtLog.AppendText($"> Запускаю {arrayCount} тестів...\n");
                txtLog.AppendText(">> Зачекайте, виконуються обчислення...\n");
                Mouse.OverrideCursor = Cursors.Wait;
                Application.Current.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

                try
                {
                    bool showOperations = mnuShowOperations.IsChecked == true;
                    var model = new PlotModel { Title = showOperations ? "Аналіз кількості операцій" : "Аналіз часу виконання" };
                    model.Legends.Add(new OxyPlot.Legends.Legend
                    {
                        LegendPosition = OxyPlot.Legends.LegendPosition.TopLeft, 
                        LegendPlacement = OxyPlot.Legends.LegendPlacement.Inside, 
                        LegendBackground = OxyColor.FromAColor(220, OxyColors.White), 
                        LegendBorder = OxyColors.Black 
                    });
                    model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Розмір масиву (N)", IsPanEnabled = false, IsZoomEnabled = false });
                    model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = showOperations ? "Кількість операцій" : "Час виконання (мс)", IsPanEnabled = false, IsZoomEnabled = false });
                    var dataSeries = new LineSeries
                    {
                        Title = showOperations ? "Кількість операцій" : "Практичний час (мс)",
                        CanTrackerInterpolatePoints = false,
                        Color = showOperations ? OxyColors.DodgerBlue : OxyColors.Lime,
                        MarkerType = showOperations ? MarkerType.Triangle : MarkerType.Circle
                    };
                    LineSeries theorySeries = null;
                    if (showOperations)
                    {
                        theorySeries = new LineSeries { Title = "Теоретична складність O(n^2)", CanTrackerInterpolatePoints = false, Color = OxyColors.Gray, LineStyle = LineStyle.Dash };
                    }

                    List<SortResult> results = _appManager.RunFullAnalysis(arrayCount);
                    double totalTime = 0;

                    for (int i = 0; i < results.Count; i++)
                    {
                        int n = results[i].Data!.Length;
                        double time = results[i].TimeMs;
                        long ops = results[i].Operations;
                        totalTime += time;

                        txtLog.AppendText($"+[Масив {i + 1}] Розмір: N = {n}, Час: {time:F3} мс, Операцій: {ops}\n");
                        if (showOperations)
                        {
                            dataSeries.Points.Add(new DataPoint(n, ops));
                            theorySeries.Points.Add(new DataPoint(n, Math.Pow(n, 2) / 3.0));
                        }
                        else dataSeries.Points.Add(new DataPoint(n, time));
                    }

                    txtLog.AppendText(">> ------------------------\n");
                    txtLog.AppendText($">> Загальний час сортування: {totalTime / 1000.0:F2} с ({totalTime:F0} мс)\n");
                    txtLog.AppendText($">> Середній час: {(totalTime / results.Count):F3} мс\n");

                    model.Series.Add(dataSeries);
                    if (showOperations) model.Series.Add(theorySeries);
                    plotView.Model = model;
                    plotView.InvalidatePlot();
                    txtLog.ScrollToEnd();
                }
                finally {
                    Mouse.OverrideCursor = null;
                    txtLog.ScrollToEnd();
                }
            }
        }

        private void OpenInputWindow(object sender, RoutedEventArgs e)
        {
            InputWindow inputWin = new InputWindow("Введіть кількість елементів у масиві:", "1000") { Owner = this, Title = "Генерація масиву" };
            if (inputWin.ShowDialog() == true)
            {
                txtLog.AppendText($"> Генерую {inputWin.Count} елементів...\n");
                Mouse.OverrideCursor = Cursors.Wait;
                Application.Current.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);

                try
                {
                    SortResult result = _appManager.GenerateAndSort(inputWin.Count);
                    ProcessSingleArray(result, $"Візуалізація масиву (N={inputWin.Count})");
                }
                finally { Mouse.OverrideCursor = null; }
            }
        }

        private void ExitApp(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }
    }
}