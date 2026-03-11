namespace LR2.SortingLogic
{
    public class SortingManager
    {
        private readonly ISorter _sorter;
        private int[]? _currentArray;

        public SortingManager(ISorter sorter) { _sorter = sorter; }
        public int[]? CurrentArray => _currentArray;

        public SortResult GenerateAndSort(int size)
        {
            int[] rawData = DataHelper.Generate(size);
            SortResult result = _sorter.Sort(rawData);
            _currentArray = result.Data;
            return result;
        }

        public SortResult LoadAndSort(string path)
        {
            _currentArray = DataHelper.Load(path);
            SortResult result = _sorter.Sort(_currentArray);
            _currentArray = result.Data;
            return result;
        }

        public void SaveCurrent(string path)
        {
            if (_currentArray == null) throw new InvalidOperationException("Немає даних для збереження.");
            DataHelper.Save(path, _currentArray);
        }

        public List<SortResult> RunFullAnalysis(int arrayCount)
        {
            var results = new List<SortResult>();
            for (int i = 1; i <= arrayCount; i++)
            {
                int n = i * 1000;
                int[] data = DataHelper.Generate(n);
                results.Add(_sorter.Sort(data));
            }
            return results;
        }
    }
}