using System.Collections.Generic;
using SuperRocket.Core.Model;

namespace SuperRocket.Core.Services
{
    public interface ICustomerService
    {
        List<Customer> GetAllCustomers();
    }
}
