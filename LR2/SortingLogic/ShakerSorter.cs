using System.Diagnostics;

namespace LR2.SortingLogic
{
    public class ShakerSorter : ISorter {
        public SortResult Sort(int[] input) {
            if (input == null || input.Length == 0)
                throw new ArgumentException("Помилка: Масив порожній.");

            int[] m = (int[])input.Clone();
            long ops = 0;
            int n = m.Length;
            int left = 1, right = n - 1, j, t, k = n - 1;

            Stopwatch sw = Stopwatch.StartNew();
            do {
                for (j = right; j >= left; j--) {
                    ops++;
                    if (m[j - 1] > m[j]) { t = m[j - 1]; m[j - 1] = m[j]; m[j] = t; k = j; }
                }
                left = k + 1;
                for (j = left; j <= right; j++) {
                    ops++;
                    if (m[j - 1] > m[j]) { t = m[j - 1]; m[j - 1] = m[j]; m[j] = t; k = j; }
                }
                right = k - 1;
            } 
            while (left <= right);
            sw.Stop();

            return new SortResult { TimeMs = sw.Elapsed.TotalMilliseconds, Operations = ops, Data = m };
        }
    }
}
