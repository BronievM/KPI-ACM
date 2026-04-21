namespace LR4.Models.Equations
{
    public interface IEquation
    {
        string Title { get; }
        double A { get; }
        double B { get; }
        double Evaluate(double x);
        double EvaluateFirstDerivative(double x);
        double EvaluateSecondDerivative(double x);
    }
}
