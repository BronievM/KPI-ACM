namespace LR5.Models
{
    public interface ILinearSystem
    {
        string Title { get; }
        double[,] A { get; }
        double[] B { get; }
    }
}
