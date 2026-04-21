using LR4.Models.Equations;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LR4.Models
{
    public class TangentSolver
    {
        public AnalysisResult Solve(IEquation eq, double epsilon)
        {
            var result = new AnalysisResult();
            result.LogLines.Add($"--- {eq.Title} ---");
            result.LogLines.Add($"Метод: Дотичних (Ньютона)");
            result.LogLines.Add($"Відрізок пошуку: [{eq.A}, {eq.B}], Точність ε = {epsilon}\n");

            var calculator = new TangentMath();

            result.LogLines.Add("--- Етап 1: Програмне відокремлення коренів ---");
            double step = 0.1; 
            var isolatedIntervals = calculator.SeparateRoots(eq, eq.A, eq.B, step);

            if (isolatedIntervals.Count == 0)
            {
                result.LogLines.Add($"[!] На відрізку [{eq.A}; {eq.B}] коренів не виявлено.");
                return result; 
            }

            result.LogLines.Add($"Крок табуляції: h = {step}");
            foreach (var interval in isolatedIntervals)
            {
                result.LogLines.Add($"Корінь локалізовано на вузькому проміжку: [{interval.Item1:F2}; {interval.Item2:F2}]");
            }
            result.LogLines.Add("");

            result.LogLines.Add("--- Етап 2: Уточнення кореня (Метод Ньютона) ---");
            var steps = calculator.FindRoot(eq, epsilon, out double finalRoot);

            if (steps.Count > 0) result.LogLines.Add($"Початкове наближення (x0): {steps[0].Xn}\n");

            result.LogLines.Add($"{"n",-2} | {"x_n",-8} | {"f(x_n)",-10} | {"f'(x)",-8} | {"x_new",-8} | {"|Δ|",-9}");
            result.LogLines.Add(new string('-', 55));

            foreach (var _step in steps)
            {
                result.LogLines.Add($"{_step.N,-2} | {_step.Xn,8:F4} | {_step.F_Xn,10:E3} | {_step.Df_Xn,8:F3} | {_step.X_Next,8:F4} | {_step.Delta,9:E3}");
            }

            result.LogLines.Add($"\n>>> Знайдений корінь: x = {finalRoot:F8}");
            result.LogLines.Add($">>> Значення функції: f(x) = {eq.Evaluate(finalRoot):E8}");
            result.LogLines.Add($">>> Кількість ітерацій: {steps.Count}");

            var plotModel = new PlotModel { Title = eq.Title, Background = OxyColors.White };
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "x" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "f(x)" });

            plotModel.Legends.Add(new OxyPlot.Legends.Legend { LegendPosition = OxyPlot.Legends.LegendPosition.BottomRight,  LegendBackground = OxyColor.FromAColor(220, OxyColors.White),  LegendBorder = OxyColors.Black  });

            plotModel.Annotations.Add(new OxyPlot.Annotations.LineAnnotation { Type = OxyPlot.Annotations.LineAnnotationType.Horizontal, Y = 0, Color = OxyColors.DarkSlateGray, LineStyle = LineStyle.Dash, StrokeThickness = 1.5   });
            plotModel.Annotations.Add(new OxyPlot.Annotations.LineAnnotation { Type = OxyPlot.Annotations.LineAnnotationType.Vertical, X = 0, Color = OxyColors.DarkSlateGray, LineStyle = LineStyle.Dash, StrokeThickness = 1.5 });
            var funcSeries = new LineSeries { Title = "f(x)", Color = OxyColors.Blue, StrokeThickness = 2, TrackerFormatString = "{0}\nx: {2:F4}\ny: {4:F4}" };
            for (double x = eq.A - 0.5; x <= eq.B + 0.5; x += 0.05)
            {
                funcSeries.Points.Add(new DataPoint(x, eq.Evaluate(x)));
            }
            plotModel.Series.Add(funcSeries);

            var rootSeries = new ScatterSeries { Title = $"Знайдений корінь (x ≈ {finalRoot:F4})", MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red, MarkerSize = 4, TrackerFormatString = "{0}\nx: {2:F4}\ny: {4:F4}" };
            rootSeries.Points.Add(new ScatterPoint(finalRoot, 0));
            plotModel.Series.Add(rootSeries);
            result.GraphModel = plotModel;
            return result;
        }
    }
}
