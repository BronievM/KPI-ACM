namespace LR5.Models
{
    public class JacobiMath
    {
        public bool CheckConvergence(double[,] A, out double maxNorm)
        {
            int n = A.GetLength(0);
            maxNorm = 0;

            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i != j) sum += Math.Abs(A[i, j] / A[i, i]);
                }
                if (sum > maxNorm) maxNorm = sum;
            }

            return maxNorm < 1.0;
        }

        public List<IterationStep> Solve(double[,] A, double[] B, double epsilon, out double[] finalX)
        {
            int n = A.GetLength(0);

            double[,] alpha = new double[n, n];
            double[] beta = new double[n];

            for (int i = 0; i < n; i++)
            {
                beta[i] = B[i] / A[i, i];
                for (int j = 0; j < n; j++)
                {
                    alpha[i, j] = (i != j) ? -A[i, j] / A[i, i] : 0.0;
                }
            }

            double[] currentX = new double[n];
            Array.Copy(beta, currentX, n);

            List<IterationStep> steps = new List<IterationStep>();
            double delta;
            int iteration = 0;

            do
            {
                double[] nextX = new double[n];
                for (int i = 0; i < n; i++)
                {
                    nextX[i] = beta[i];
                    for (int j = 0; j < n; j++)
                    {
                        nextX[i] += alpha[i, j] * currentX[j];
                    }
                }

                delta = 0;
                for (int i = 0; i < n; i++)
                {
                    double d = Math.Abs(nextX[i] - currentX[i]);
                    if (d > delta) delta = d;
                }

                steps.Add(new IterationStep {Iteration = iteration + 1,X = (double[])nextX.Clone(), Delta = delta });

                currentX = nextX;
                iteration++;

            } while (delta > epsilon && iteration < 1000);

            finalX = currentX;
            return steps;
        }
    }
}
