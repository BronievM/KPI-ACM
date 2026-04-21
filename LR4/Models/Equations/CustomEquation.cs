namespace LR4.Models.Equations
{
    internal class CustomEquation: IEquation
    {
        public string Title => "Варіант 9: x^3 - x - 3 = 0";
        public double A { get; }
        public double B { get; }

        public CustomEquation(double a, double b)
        {
            A = a;
            B = b;
        }

        public double Evaluate(double x) => Math.Pow(x, 3) - x - 3.0;

        public double EvaluateFirstDerivative(double x) => 3.0 * Math.Pow(x, 2) - 1.0;

        public double EvaluateSecondDerivative(double x) => 6.0 * x;
    }
}
