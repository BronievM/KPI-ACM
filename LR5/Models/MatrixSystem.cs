namespace LR5.Models
{
    public class MatrixSystem
    {
        public double[,] A { get; set; }
        public double[] B { get; set; }
        public double Epsilon { get; set; }

        public MatrixSystem()
        {
            A = new double[,] {
                { 10.0,  2.0, -1.0 },
                {  1.0,  8.0,  2.0 },
                {  2.0, -1.0, 10.0 }
            };
            B = new double[] { 11.0, 11.0, 11.0 };
            Epsilon = 0.0001;
        }
    }
}
