using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestingThings.Models;

namespace TestingThings.Clients.Http
{
    public interface ICustomerQueryClient
    {
        Task<IEnumerable<Customer>> GetCustomers(CancellationToken cancellationToken = default);
    }
}