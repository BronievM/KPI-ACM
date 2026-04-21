using System.IO;
namespace LR3.Models
{
    public static class DataHelper
    {
        public static void SaveLog(string path, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Немає даних для збереження.");

            File.WriteAllText(path, content);
        }
    }
}
