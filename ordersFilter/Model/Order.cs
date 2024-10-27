
namespace ordersFilter.Model
{
    internal class Order
    {
        public int Id { get; set; }
        public int Weight { get; set; }
        public required string District { get; set; }
        public DateTime DeliveryDateTime { get; set; }
    }
}
