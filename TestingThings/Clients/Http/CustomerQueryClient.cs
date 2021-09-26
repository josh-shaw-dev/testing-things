using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TestingThings.Models;

namespace TestingThings.Clients.Http
{
    public class CustomerQueryClient : ICustomerQueryClient
    {
        private readonly ILogger<CustomerQueryClient> _logger;
        private readonly HttpClient _client;

        private static readonly string[] Customers = new[]
{
            "Fun Corp", "Dog Corp", "Cat Corp", "Llama Corp", "Moose Corp", "Donkey Corp", "Fish Corp", "Pool Corp", "Bike Corp", "Beer Corp"
        };

        public CustomerQueryClient(ILogger<CustomerQueryClient> logger, HttpClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<Customer>> GetCustomers(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting customers");

            Random rng = new();
            return Enumerable.Range(1, 5).Select(index => new Customer {
                Name = Customers[rng.Next(Customers.Length)]
            })
            .ToArray();
        }
    }
}
