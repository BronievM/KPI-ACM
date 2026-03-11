using System.IO;

namespace LR2.SortingLogic
{
    public static class DataHelper
    {
        private static Random rnd = new Random();

        public static int[] Generate(int size)
        {
            if (size <= 0) throw new ArgumentException("Розмір має бути більшим за 0.");
            return Enumerable.Range(0, size).Select(_ => rnd.Next(-10000, 10000)).ToArray();
        }

        public static void Save(string path, int[] data)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Немає даних.");
            File.WriteAllText(path, string.Join(" ", data));
        }

        public static int[] Load(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Файл не знайдено.");

            string content = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Файл порожній.");

            var parts = content.Split(new[] { ' ', '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] result = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out result[i])) throw new FormatException($"Елемент '{parts[i]}' не є цілим числом. Перевірте файл.");
            }
            return result;
        }
    }
}