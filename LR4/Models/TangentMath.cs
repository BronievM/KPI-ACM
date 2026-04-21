using LR4.Models.Equations;

namespace LR4.Models
{
    internal class TangentMath
    {
        public List<(double, double)> SeparateRoots(IEquation eq, double a, double b, double h)
        {
            var intervals = new List<(double, double)>();
            double x = a;

            while (x < b)
            {
                double nextX = Math.Min(x + h, b); 
                double fx1 = eq.Evaluate(x);
                double fx2 = eq.Evaluate(nextX);

                if (fx1 * fx2 <= 0) intervals.Add((x, nextX));
                x = nextX;
            }
            return intervals;
        }

        public List<IterationStep> FindRoot(IEquation eq, double epsilon, out double root)
        {
            var steps = new List<IterationStep>();
            double x_n = eq.Evaluate(eq.A) * eq.EvaluateSecondDerivative(eq.A) > 0 ? eq.A : eq.B;

            int iteration = 0;
            double delta = double.MaxValue;

            while (delta > epsilon && iteration < 1000)
            {
                double f_xn = eq.Evaluate(x_n);
                double df_xn = eq.EvaluateFirstDerivative(x_n);
                if (Math.Abs(df_xn) < 1e-15) break;
                double x_next = x_n - f_xn / df_xn;
                delta = Math.Abs(x_next - x_n);
                steps.Add(new IterationStep { N = iteration, Xn = x_n, F_Xn = f_xn, Df_Xn = df_xn, X_Next = x_next, Delta = delta });
                x_n = x_next;
                iteration++;
            }

            root = x_n;
            return steps;
        }
    }
}
