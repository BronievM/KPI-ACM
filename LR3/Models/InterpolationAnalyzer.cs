using LR3.Models.Functions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LR3.Models
{
    internal class InterpolationAnalyzer
    {
        private NewtonInterpolator _lastInterpolator;
        private IFunction _lastFunc;

        public AnalysisResult RunExperiment(IFunction funcDef, bool isErrorMode)
        {
            _lastFunc = funcDef;
            var result = new AnalysisResult();

            result.LogLines.Add($"--- {funcDef.Title} ---");
            result.LogLines.Add($"Інтервал: [{funcDef.A}, {funcDef.B}], Кількість інтервалів m = {funcDef.M}");

            double h = (funcDef.B - funcDef.A) / funcDef.M;
            double[] X = new double[funcDef.M + 1];
            double[] Y = new double[funcDef.M + 1];

            for (int i = 0; i <= funcDef.M; i++)
            {
                X[i] = funcDef.A + i * h;
                Y[i] = funcDef.Evaluate(X[i]);
            }

            _lastInterpolator = new NewtonInterpolator(X, Y);

            var plotModel = new PlotModel
            {
                Title = isErrorMode ? "Графік похибки" : "Графік інтерполяції",
                Background = OxyColors.White
            };

            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "x", IsPanEnabled = false, IsZoomEnabled = false });

            int plotPoints = 200;
            double step = (funcDef.B - funcDef.A) / plotPoints;

            if (isErrorMode)
            {
                plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Абс. похибка |Δ|", IsPanEnabled = false, IsZoomEnabled = false });

                var errorSeries = new LineSeries { Title = "Похибка", Color = OxyColors.Purple, StrokeThickness = 1.5 };

                for (double x = funcDef.A; x <= funcDef.B + 1e-9; x += step)
                {
                    double exact = funcDef.Evaluate(x);
                    double interp = _lastInterpolator.Evaluate(x, funcDef.M);
                    errorSeries.Points.Add(new DataPoint(x, Math.Abs(exact - interp)));
                }

                plotModel.Series.Add(errorSeries);
            }
            else
            {
                plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "f(x)", IsPanEnabled = false, IsZoomEnabled = false });

                var funcSeries = new LineSeries { Title = "Оригінальна функція", Color = OxyColors.Blue, StrokeThickness = 2 };
                var interpSeries = new LineSeries { Title = $"Інтерполяція (n={funcDef.M})", Color = OxyColors.Red, LineStyle = LineStyle.Dash, StrokeThickness = 2 };
                var nodesSeries = new ScatterSeries { Title = "Вузли", MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Black, MarkerSize = 4 };

                for (int i = 0; i <= funcDef.M; i++)
                {
                    nodesSeries.Points.Add(new ScatterPoint(X[i], Y[i]));
                }

                for (double x = funcDef.A; x <= funcDef.B + 1e-9; x += step)
                {
                    double exact = funcDef.Evaluate(x);
                    double interp = _lastInterpolator.Evaluate(x, funcDef.M);
                    funcSeries.Points.Add(new DataPoint(x, exact));
                    interpSeries.Points.Add(new DataPoint(x, interp));
                }

                plotModel.Series.Add(funcSeries);
                plotModel.Series.Add(interpSeries);
                plotModel.Series.Add(nodesSeries);

                plotModel.Legends.Add(new OxyPlot.Legends.Legend { LegendPosition = OxyPlot.Legends.LegendPosition.TopRight, LegendBackground = OxyColor.FromAColor(220, OxyColors.White), LegendBorder = OxyColors.Black });
            }

            result.GraphModel = plotModel;

            int maxDegree = funcDef.M - 3;
            if (maxDegree > 0)
            {
                int j_idx = funcDef.M / 2 - 1;
                double x_mid = (X[j_idx] + X[j_idx + 1]) / 2.0;

                result.LogLines.Add($"\nОцінка похибки інтерполяції для x = {x_mid:F5}");
                result.LogLines.Add($"{"n",-3} | {"Δn (оцінка)",-15} | {"Δn exact",-15} | {"k_Δ",-10} | {"δn",-10}");
                result.LogLines.Add(new string('-', 65));

                for (int n = 1; n <= maxDegree; n++)
                {
                    double P_n = _lastInterpolator.Evaluate(x_mid, n);
                    double P_n1 = _lastInterpolator.Evaluate(x_mid, n + 1);
                    double P_n2 = _lastInterpolator.Evaluate(x_mid, n + 2);

                    double exact = funcDef.Evaluate(x_mid);
                    double delta_n = P_n - P_n1;
                    double delta_exact = P_n - exact;
                    double delta_delta_n = P_n1 - P_n2;

                    double k_delta = delta_n != 0 ? 1.0 - delta_exact / delta_n : 0;
                    double rel_blur = delta_n != 0 ? Math.Abs(delta_delta_n / delta_n) : 0;

                    result.LogLines.Add($"{n,-3} | {delta_n,15:E5} | {delta_exact,15:E5} | {k_delta,10:F4} | {rel_blur,10:F4}");
                }
            }

            return result;
        }

        public string EvaluateAtPoint(double x)
        {
            if (_lastInterpolator == null || _lastFunc == null)
                return "\n[!] Помилка: Спочатку виконайте аналіз функції.";

            if (x < _lastFunc.A - 1 || x > _lastFunc.B + 1)
                return $"\n[!] Попередження: Точка x={x} знаходиться за межами відрізка [{_lastFunc.A}, {_lastFunc.B}]. Обчислення може бути некоректним.";

            double exact = _lastFunc.Evaluate(x);
            double interp = _lastInterpolator.Evaluate(x, _lastFunc.M);
            double error = Math.Abs(exact - interp);

            return $"\n>>> Обчислення в точці x = {x}\n" + $"Оригінальна функція: {exact:F8}\n" +
                   $"Поліном Ньютона:     {interp:F8}\n" + $"Абсолютна похибка:   {error:E5}\n";
        }
    }

}
