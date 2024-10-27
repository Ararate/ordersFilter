
namespace ordersFilter.Files_IO.Import
{
    internal interface IImporter
    {
        T? Import<T>(string path);
    }
}
