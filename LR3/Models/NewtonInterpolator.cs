namespace LR3.Models
{
    internal class NewtonInterpolator
    {
        private readonly double[] _x;
        private readonly double[] _y;
        private readonly double[,] _d;
        private readonly int _maxDegree;

        public NewtonInterpolator(double[] x, double[] y)
        {
            _x = x;
            _y = y;
            _maxDegree = x.Length - 1;
            _d = new double[_maxDegree + 1, _maxDegree + 1];

            for (int i = 0; i <= _maxDegree; i++) _d[i, 0] = _y[i];
            for (int j = 1; j <= _maxDegree; j++)
            {
                for (int i = 0; i <= _maxDegree - j; i++)
                {
                    _d[i, j] = (_d[i + 1, j - 1] - _d[i, j - 1]) / (_x[i + j] - _x[i]);
                }
            }
        }

        public double Evaluate(double xTarget, int degree)
        {
            if (degree > _maxDegree) degree = _maxDegree;
            double result = _d[0, 0];
            double term = 1.0;
            for (int i = 1; i <= degree; i++)
            {
                term *= (xTarget - _x[i - 1]);
                result += term * _d[0, i];
            }
            return result;
        }
    }
}
