
namespace ordersFilter.Repository
{
    internal interface IRepository<T> where T : class
    {
        public IEnumerable<T> GetAll(Func<T, bool>? filter = null);

    }
}
