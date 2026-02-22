using System.IO;
using System.Text.Json;

namespace GestoDocente.Services
{
    public static class DataService
    {
        private static string BasePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "GestoDocente");

        public static T? LoadJson<T>(string filename)
        {
            try
            {
                string fullPath = Path.Combine(BasePath, filename);
                if (!File.Exists(fullPath)) return default;
                string json = File.ReadAllText(fullPath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch { return default; }
        }

        public static void SaveJson<T>(string filename, T data)
        {
            try
            {
                Directory.CreateDirectory(BasePath);
                string fullPath = Path.Combine(BasePath, filename);
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fullPath, json);
            }
            catch { }
        }
    }
}
