using LR5.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Windows;

namespace LR5
{
    public partial class MainWindow : Window
    {
        private readonly JacobiSolver _solver;
        private readonly MatrixSystem _config;
        private AnalysisResult _lastResult;

        public MainWindow()
        {
            InitializeComponent();
            QuestPDF.Settings.License = LicenseType.Community;
            _solver = new JacobiSolver();
            _config = new MatrixSystem();
        }

        private void RunCustomFunction(object sender, RoutedEventArgs e)
        {
            var inputWin = new InputWindow(_config) { Owner = this };

            if (inputWin.ShowDialog() == true)
            {
                txtLog.Clear();
                var math = new JacobiMath();

                if (!math.CheckConvergence(_config.A, out double norm))
                {
                    var result = MessageBox.Show(
                        $"Матриця не задовольняє вимоги збіжності (норма = {norm:F4} >= 1).\nІтераційний процес розбіжиться.\n\nВпевнені, що хочете продовжити?",
                        "Попередження про збіжність",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        txtLog.AppendText("[СКАСОВАНО] Обчислення зупинено користувачем через порушення умови збіжності.\n");
                        return;
                    }
                }

                _lastResult = _solver.Execute(_config.A, _config.B, _config.Epsilon, "Метод Якобі");
                txtLog.Text = _lastResult.LogText;
                txtLog.ScrollToEnd();
                DrawConvergenceGraph(_lastResult.Steps);
            }
        }

        private void DrawConvergenceGraph(List<IterationStep> steps)
        {
            var model = new PlotModel { Title = "Графік збіжності методу Якобі" };

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Номер ітерації",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsZoomEnabled = false,
                IsPanEnabled = false
            });

            model.Axes.Add(new LogarithmicAxis
            {
                Position = AxisPosition.Left,
                Title = "Похибка (Delta)",
                MajorGridlineStyle = LineStyle.Solid,
                IsZoomEnabled = false,
                IsPanEnabled = false
            });

            var series = new LineSeries
            {
                Title = "Зміна похибки",
                Color = OxyColors.Blue,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.Red
            };

            foreach (var step in steps)
            {
                if (step.Delta > 0)
                {
                    series.Points.Add(new DataPoint(step.Iteration, step.Delta));
                }
            }

            model.Series.Add(series);
            plotConvergence.Model = model;
        }

        private byte[] GetPlotImageBytes()
        {
            if (plotConvergence.Model == null) return null;

            using (var stream = new System.IO.MemoryStream())
            {
                var pngExporter = new OxyPlot.Wpf.PngExporter { Width = 800, Height = 500 };
                pngExporter.Export(plotConvergence.Model, stream);
                return stream.ToArray();
            }
        }
        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            if (_lastResult == null) return;

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF файли (*.pdf)|*.pdf",
                FileName = "Звіт_СЛАР"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    byte[] chartImage = GetPlotImageBytes();

                    QuestPDF.Fluent.Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(QuestPDF.Helpers.PageSizes.A4);
                            page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                            page.PageColor(QuestPDF.Helpers.Colors.White);
                            page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                            page.Header().PaddingBottom(10).Text("Результати обчислень: Метод Якобі")
                                .SemiBold().FontSize(18).FontColor(QuestPDF.Helpers.Colors.Black);

                            page.Content().Column(column =>
                            {
                                column.Item().PaddingBottom(5).Text("Початкова система рівнянь:").SemiBold();
                                column.Item().Border(1).Padding(5).Column(c =>
                                {
                                    int n = _config.A.GetLength(0);
                                    for (int i = 0; i < n; i++)
                                    {
                                        string row = "";
                                        for (int j = 0; j < n; j++)
                                        {
                                            double val = _config.A[i, j];
                                            row += j == 0 ? $"{val}*x{j + 1}" : $" {(val < 0 ? "-" : "+")} {Math.Abs(val)}*x{j + 1}";
                                        }
                                        c.Item().Text($"{row} = {_config.B[i]}");
                                    }
                                });

                                if (chartImage != null)
                                {
                                    column.Item().PaddingTop(15).Text("Графік збіжності (Delta):").SemiBold();
                                    column.Item().Image(chartImage, ImageScaling.FitWidth);
                                }

                                column.Item().PaddingTop(15).Text("Остаточний результат:").SemiBold();
                                var finalX = _lastResult.Steps.Last().X;
                                column.Item().BorderBottom(1).PaddingVertical(5).Column(c =>
                                {
                                    for (int i = 0; i < finalX.Length; i++)
                                    {
                                        c.Item().Text($"x{i + 1} = {finalX[i]:F8}");
                                    }
                                    c.Item().Text($"Кількість ітерацій: {_lastResult.Steps.Count}").Italic();
                                });

                                column.Item().PaddingTop(20).Text("Протокол ітераційного процесу:").SemiBold();
                                column.Item().Table(table =>
                                {
                                    int n = _config.A.GetLength(0);
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(35);
                                        for (int i = 0; i < n; i++) columns.RelativeColumn();
                                        columns.RelativeColumn(); 
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("№");
                                        for (int i = 0; i < n; i++) header.Cell().Element(CellStyle).Text($"x{i + 1}");
                                        header.Cell().Element(CellStyle).Text("Delta");

                                        static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
                                            => container.DefaultTextStyle(x => x.SemiBold()).BorderBottom(1).PaddingVertical(5).AlignCenter();
                                    });

                                    foreach (var step in _lastResult.Steps)
                                    {
                                        table.Cell().Element(ContentStyle).Text(step.Iteration.ToString());
                                        foreach (var x in step.X) table.Cell().Element(ContentStyle).Text(x.ToString("F5"));
                                        table.Cell().Element(ContentStyle).Text(step.Delta.ToString("E2"));

                                        static QuestPDF.Infrastructure.IContainer ContentStyle(QuestPDF.Infrastructure.IContainer container)
                                            => container.BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(2).AlignCenter();
                                    }
                                });
                            });

                        });
                    })
             .GeneratePdf(saveFileDialog.FileName);

                    MessageBox.Show("Звіт успішно згенеровано!");
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Не вдалося зберегти файл. Схоже, він зараз відкритий у якійсь програмі (наприклад, у браузері або PDF-рідері).\n\nБудь ласка, закрийте файл і спробуйте ще раз.",
                        "Файл зайнято", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Виникла непередбачена помилка під час збереження звіту:\n{ex.Message}",
                        "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}