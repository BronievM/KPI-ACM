using System.Text;

namespace LR5.Models
{
    public class JacobiSolver
    {
        public AnalysisResult Execute(double[,] A, double[] B, double epsilon, string title)
        {
            var sb = new StringBuilder();
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
                sb.AppendLine($"[УВАГА] Умова збіжності не виконується! Норма матриці = {norm:F4} (має бути < 1).");
            }
            else
            {
                sb.AppendLine($"Умова збіжності виконується. Норма матриці = {norm:F4} < 1.\n");
            }

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

            sb.AppendLine("\n>----------------------");

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

            sb.AppendLine("\nРезультат:");
            for (int i = 0; i < finalX.Length; i++)
            {
                sb.AppendLine($"x{i + 1} = {finalX[i]:F7}");
            }
            sb.AppendLine($"\nКількість ітерацій: {steps.Count}");

            return new AnalysisResult { LogText = sb.ToString(), Steps = steps };
        }
    }
}
