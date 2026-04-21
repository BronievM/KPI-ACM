namespace LR5.Models
{
    public class CustomSystem : ILinearSystem
    {
        public string Title { get; set; } = "Власна система рівнянь";
        public double[,] A { get; set; }
        public double[] B { get; set; }
    }
}
