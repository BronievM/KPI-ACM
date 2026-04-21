namespace LR3.Models.Functions
{
    internal class TestFunction: IFunction
    {
        public TestFunction(int m) { M = m; }

        public string Title => "Функція sin(x)";
        public double A => 0;
        public double B => Math.PI / 2;
        public int M { get; private set; }
        public double Evaluate(double x) => Math.Sin(x);
    }
}
