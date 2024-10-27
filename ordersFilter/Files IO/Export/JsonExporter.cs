using Newtonsoft.Json;

namespace ordersFilter.Files_IO
{
    internal class JsonExporter : IExporter
    {
        public void Export(string? path, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
        }
    }
}
