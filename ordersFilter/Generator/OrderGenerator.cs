using Bogus;
using ordersFilter.Model;

namespace ordersFilter
{
    /// <summary>
    /// Генератор случайных заказов
    /// </summary>
    internal class OrderGenerator
    {
        private readonly Faker<Order> _orderFake;

        public OrderGenerator()
        {
            string[] districts = { "Арбат", "Басманный район", "Замоскворечье", "Красносельский район", "Мещанский район", "Пресненский район", "Таганский район", "Тверской район" };
            _orderFake = new Faker<Order>()
                .RuleFor(o => o.Id, f => f.Random.Number(1, 1000))
                .RuleFor(o => o.Weight, f => f.Random.Number(10, 90))
                .RuleFor(o => o.District,f => districts[f.Random.Number(0, districts.Length-1)] )
                .RuleFor(o => o.DeliveryDateTime,f => RandomDay());
        }

        public Order Generate()
        {
            return _orderFake.Generate();
        }

        private DateTime RandomDay()
        {
            Random gen = new Random();
            DateTime start = new DateTime(2023, 1, 1);
            int range = (int)Math.Ceiling((DateTime.Today - start).TotalSeconds);
            return start.AddSeconds(gen.Next(range));
        }
    }
}

