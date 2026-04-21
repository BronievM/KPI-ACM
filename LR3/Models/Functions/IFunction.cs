namespace LR3.Models.Functions
{
    internal interface IFunction
    {
        string Title { get; }
        double A { get; }
        double B { get; }
        int M { get; } 
        double Evaluate(double x);
    }
}
