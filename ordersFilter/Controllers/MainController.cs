using Newtonsoft.Json;
using ordersFilter.Attributes;
using ordersFilter.Files_IO;
using ordersFilter.Model;
using ordersFilter.Repository;
using System.Configuration;

namespace ordersFilter
{
    /// <summary>
    /// Основной контроллер
    /// </summary>
    [Controller]
    internal class MainController
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IExporter _exporter;
        private readonly OrderGenerator _orderGenerator;

        public MainController(IRepository<Order> orderRepo, IExporter exporter, OrderGenerator orderGenerator)
        {
            _orderRepo = orderRepo;
            _exporter = exporter;
            _orderGenerator = orderGenerator;
        }

        /// <summary>
        /// Фильтр источника, указанного в параметре sourcePath в app.config и экспорт результата.
        /// Параметры(3): district, dateFrom, pathExport
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Command("filter")]
        public void Filter(string[] parameters)
        {
            try
            {
            if (parameters.Length != 3)
                {
                    Console.WriteLine("Ошибка. Неверно заданы параметры. \nНеобходимые параметры(3): район, время первой доставки(ДД.ММ.ГГГГ чч:мм:сс), путь к файлу для экспорта");
                    return;
                }
            string district = parameters[0].ToLower();
            DateTime dateFrom;
            string pathExport = parameters[2];
            dateFrom = DateTime.ParseExact(parameters[1], "dd.MM.yyyy HH:mm:ss", null);

            var result = _orderRepo.GetAll(
                x => x.DeliveryDateTime >= dateFrom 
                && x.DeliveryDateTime <= dateFrom.AddMinutes(30) 
                && x.District.ToLower() == district);

            _exporter.Export(pathExport,result);
                Console.WriteLine($"Данные успешно выгружены в файл {pathExport}");
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(FormatException))
                    Console.WriteLine("Ошибка. Дата должна быть в формате ДД.ММ.ГГГГ чч:мм:сс");
                else
                    Console.WriteLine(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Генерация заказов
        /// </summary>
        /// <param name="parameters"></param>

        [Command("generateOrders")]
        public void GenerateOrders(string[] parameters)
        {
            try
            {
                if (parameters.Length != 2 || !int.TryParse(parameters[1], out _) || int.Parse(parameters[1]) < 1 || int.Parse(parameters[1]) > 1000)
                {
                    Console.WriteLine("Ошибка. Неверно заданы параметры. \nНеобходимые параметры(2): путь к файлу, количество записей(1-1000)");
                    return;
                }
                int quantity = int.Parse(parameters[1]);
                string pathExport = parameters[0];

                var orders = new Order[quantity];
                for (int i = 0; i < orders.Length; i++)
                {
                    Order or;
                    do
                    {
                        or = _orderGenerator.Generate();
                        orders[i] = or;
                    }
                    while (!orders.Any(o => o.Id == or.Id));
                }
                var json = JsonConvert.SerializeObject(orders);
                File.WriteAllText(pathExport, json);
                Console.WriteLine("Готово");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        /// <summary>
        /// Вывод справки
        /// </summary>
        /// <param name="parameters"></param>

        [Command("help")]
        public void Help(string[] parameters)
        {
            Console.WriteLine(
                "filter [название района] [время первого заказа(ДД.ММ.ГГГГ чч:мм:сс)] [путь к файлу для экспорта]\n" +
                "   фильтрация заказов для доставки в конкретный район города в ближайшие полчаса после времени первого заказа.\n" +
                "generateOrders [путь к файлу для экспорта] [количество записей(число 1-1000)]\n" +
                "   генерация случайных записей заказов по указанному пути\n" +
                "ChangeOrderSourceFile [путь к файлу]\n" +
                "   Задать источник списка заказов");
        }

        /// <summary>
        /// Изменение пути к источнику списка заказов
        /// </summary>
        /// <param name="parameters"></param>
        [Command("ChangeOrderSourceFile")]
        public void ChangeOrderSourceFile(string[] parameters)
        {
            if(parameters.Length != 1)
            {
                Console.WriteLine("Ошибка. Неверно заданы параметры. \nНеобходимые параметры(1): путь к файлу");
                return;
            }

            string path = parameters[0];
            ConfigurationManager.AppSettings["sourcePath"] = path;
        }
    }
}
