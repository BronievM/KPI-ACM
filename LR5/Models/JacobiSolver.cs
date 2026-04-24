using System.Text;

namespace LR5.Models
{
    public class JacobiSolver
    {
        public AnalysisResult Execute(double[,] A, double[] B, double epsilon, string title)
        {
            var sb = new StringBuilder();
            string convergenceType;
            double qSum = 0;
            int qCount = 0;
            sb.AppendLine($"--- {title} ---");
            sb.AppendLine($"Точність: {epsilon}\n");

            int n = A.GetLength(0);

            sb.AppendLine("Початкова система рівнянь:");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double val = A[i, j];
                    if (j == 0)
                    {
                        sb.Append($"{val}*x{j + 1}");
                    }
                    else
                    {
                        string sign = val < 0 ? "-" : "+";
                        sb.Append($" {sign} {Math.Abs(val)}*x{j + 1}");
                    }
                }
                sb.AppendLine($" = {B[i]}");
            }
            sb.AppendLine();

            var math = new JacobiMath();

            if (!math.CheckConvergence(A, out double norm))
            {
                sb.AppendLine($"[УВАГА] Умова збіжності не виконується! Норма матриці = {norm:F3} (має бути < 1).");
            }
            else
            {
                sb.AppendLine($"Умова збіжності виконується. Норма матриці = {norm:F3} < 1.\n");
            }

            if (norm < 0.5) convergenceType = "швидка";
            else if (norm < 0.9) convergenceType = "середня";
            else convergenceType = "повільна";

            sb.AppendLine($"Тип збіжності: {convergenceType}");

            if (norm > 0.9)
                sb.AppendLine("[УВАГА] Очікується повільна збіжність або нестабільна поведінка.");

            sb.AppendLine();

            var steps = math.Solve(A, B, epsilon, out double[] finalX);

            sb.AppendLine(string.Format("{0,-5} | {1,-45} | {2,-10}", "Ітер", "Значення X", "Delta"));
            sb.AppendLine(new string('-', 70));

            foreach (var step in steps)
            {
                string xStr = "";
                for (int i = 0; i < step.X.Length; i++)
                {
                    xStr += $"x{i + 1}={step.X[i]:F4}  ";
                }
                sb.AppendLine(string.Format("{0,-5} | {1,-45} | {2,-10:E3}", step.Iteration, xStr.Trim(), step.Delta));
            }

            for (int i = 1; i < steps.Count; i++)
            {
                if (steps[i - 1].Delta > 0)
                {
                    qSum += steps[i].Delta / steps[i - 1].Delta;
                    qCount++;
                }
            }

            double avgQ = qCount > 0 ? qSum / qCount : 0;

            sb.AppendLine("\n" + " РЕЗУЛЬТАТ ".PadLeft(40, '=').PadRight(70, '='));

            sb.AppendLine("\nПеревірка (підстановка Ax ≈ b):");

            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                    sum += A[i, j] * finalX[j];

                sb.AppendLine($"Рівняння {i + 1}: {sum:F4} ≈ {B[i]}");
            }

            bool converged = steps[^1].Delta <= epsilon && !double.IsNaN(steps[^1].Delta) && !double.IsInfinity(steps[^1].Delta);

            sb.AppendLine(converged
                ? $"Точність ε = {epsilon} досягнута."
                : "Досягнуто обмеження ітерацій.");

            
            sb.AppendLine("\nРозв’язок:");
            for (int i = 0; i < finalX.Length; i++)
            {
                if (Math.Abs(finalX[i]) > 10000)
                {
                    sb.AppendLine($"  x{i + 1} = {finalX[i]:E6}");
                }
                else
                {
                    sb.AppendLine($"  x{i + 1} = {finalX[i]:F6}");
                }
            }


            sb.AppendLine("\nХарактеристики:");
            sb.AppendLine($"  Ітерації: {steps.Count}");
            sb.AppendLine($"  Тип збіжності: {convergenceType}");
            sb.AppendLine($"  Коеф. збіжності q: {avgQ:F4}");
            sb.AppendLine($"  (Довідка) менше значення q → швидша збіжність.");


            double progress = steps[0].Delta > 0
                ? (1 - steps[^1].Delta / steps[0].Delta) * 100
                : 100;

            sb.AppendLine($"  Досягнута збіжність: {progress:F2}%");

            sb.AppendLine("\nВисновок:");
            sb.AppendLine(
                converged
                    ? $"Збіжність: {convergenceType} \nУзгоджується з нормою матриці."
                    : "Метод не досяг заданої точності."
            );

            return new AnalysisResult { LogText = sb.ToString(), Steps = steps };
        }
    }
}
