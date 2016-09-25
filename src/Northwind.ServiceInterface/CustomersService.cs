using Northwind.ServiceModel;
using Northwind.ServiceModel.Types;
using ServiceStack;
using ServiceStack.OrmLite;

namespace Northwind.ServiceInterface
{
    public class CustomersService : Service
    {
        public object Get(Customers request) => 
            new CustomersResponse { Customers = Db.Select<Customer>() };
    }
}
