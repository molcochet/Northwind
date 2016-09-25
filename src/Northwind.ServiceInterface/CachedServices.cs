using Northwind.ServiceModel;
using ServiceStack;

namespace Northwind.ServiceInterface
{
    [CacheResponse(Duration = 60 * 60, MaxAge = 30 * 60)]
    public class CachedServices : Service
    {
        public object Get(CachedCustomers request) => 
            Gateway.Send(new Customers());

        public object Get(CachedCustomerDetails request) => 
            Gateway.Send(new CustomerDetails { Id = request.Id });

        public object Get(CachedOrders request) => 
            Gateway.Send(new Orders { CustomerId = request.CustomerId, Page = request.Page });
    }
}
