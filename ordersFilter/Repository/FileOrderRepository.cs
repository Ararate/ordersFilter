using System.Configuration;
using ordersFilter.Files_IO.Import;
using ordersFilter.Model;

namespace ordersFilter.Repository
{
    internal class FileOrderRepository : IRepository<Order>
    {
        private readonly IImporter _importer;
        public FileOrderRepository(IImporter importer)
        {
            _importer = importer;
        }
        public IEnumerable<Order> GetAll(Func<Order, bool>? filter = null)
        {
            List<Order>? orders;
            while (true)
            {
                try
                {
                    var path = ConfigurationManager.AppSettings["sourcePath"];
                    orders = _importer.Import<List<Order>>(path);
                    break;
                }
                catch
                {
                    Console.WriteLine("Не удалось получить данные из файла. Укажите правильный путь к файлу: ");
                    ConfigurationManager.AppSettings["sourcePath"] = Console.ReadLine();
                }
            }
            
            if (filter != null && orders != null)
            {
                orders = orders.Where(filter).ToList();
            }
            return orders ?? [];
        }
    }
}
