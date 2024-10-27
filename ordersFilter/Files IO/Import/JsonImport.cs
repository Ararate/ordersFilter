using Newtonsoft.Json;

namespace ordersFilter.Files_IO.Import
{
    internal class JsonImporter : IImporter
    {
        public T? Import<T>(string? path)
        {
             var json = File.ReadAllText(path);
             return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
