namespace LR4.Models
{
    public class IterationStep
    {
        public int N { get; set; }
        public double Xn { get; set; }
        public double F_Xn { get; set; }
        public double Df_Xn { get; set; }
        public double X_Next { get; set; }
        public double Delta { get; set; }
    }
}
