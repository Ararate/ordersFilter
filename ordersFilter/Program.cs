using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using ordersFilter.Repository;
using ordersFilter.Model;
using ordersFilter.Files_IO;
using ordersFilter.Files_IO.Import;
using ordersFilter.Attributes;
using NLog;

namespace ordersFilter
{
    internal partial class Program
    {

        private static readonly Dictionary<string, Action<string[]>> commands = new();
        private static ServiceProvider serviceProvider;
        private static Logger log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в генератор заказов. \nВведите help для получения списка доступных команд.");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();

            MapCommands();
            while (true)
            {
                var input = Console.ReadLine();
                input ??= "";
                var parts = SplitCommand()
                    .Matches(input)
                    .Select(x => x.Groups.Cast<Group>().Last(y=>y.Value != "").Value)
                    .ToList();
                //parts.ForEach(x => Console.WriteLine(x));


                if (parts.Count > 0 && commands.ContainsKey(parts[0]))
                {
                    var command = parts[0];
                    parts.RemoveAt(0);
                    commands[command].Invoke([.. parts]);
                }
                else
                {
                    Console.WriteLine("Неизвестная команда, чтобы увидеть список команд введите help");
                }
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Order>, FileOrderRepository>();
            services.AddScoped<IExporter, JsonExporter>();
            services.AddScoped<IImporter, JsonImporter>();
            services.AddScoped<OrderGenerator>();
            services.AddScoped<MainController>();
        }

        private static void MapCommands()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(ControllerAttribute), true).Length <= 0)
                    continue;
                MethodInfo[] methods = type
                .GetMethods()
                .Where(x => x.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
                .ToArray();
                foreach (MethodInfo method in methods)
                {
                    commands.Add(((CommandAttribute)method
                        .GetCustomAttribute(typeof(CommandAttribute))).Command, (parameters) =>
                        {
                            try
                            {
                                var controller = serviceProvider.GetService(type);
                                log.Info ($"{type.Name}.{method.Name}({string.Join(" ", parameters)})");
                                method.Invoke(controller, [parameters]);
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                            }
                        });
                }
            }
        }

        [GeneratedRegex(@"([^\s\""']+)|\""([^\""]*)\""|'([^']*)'")]
        private static partial Regex SplitCommand();
    }
}
