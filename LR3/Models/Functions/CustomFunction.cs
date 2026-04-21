namespace LR3.Models.Functions
{
    internal class CustomFunction: IFunction
    {
        public CustomFunction(int m) { M = m; }

        public string Title => "Функція e^{-(x+sin(x))}";
        public double A => 2.0;
        public double B => 5.0;
        public int M { get; private set; }
        public double Evaluate(double x) => Math.Exp(-(x + Math.Sin(x)));
    }
}
