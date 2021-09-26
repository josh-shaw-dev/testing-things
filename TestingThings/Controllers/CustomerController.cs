using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestingThings.Clients.Http;
using TestingThings.Models;

namespace TestingThings.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerQueryClient _exampleHttpClient;

        public CustomerController(ICustomerQueryClient exampleHttpClient)
        {
            _exampleHttpClient = exampleHttpClient ?? throw new ArgumentNullException(nameof(exampleHttpClient));
        }

        [HttpGet]
        public async Task<IEnumerable<Customer>> Get(CancellationToken cancellationToken)
        {
            return await _exampleHttpClient.GetCustomers(cancellationToken);
        }
    }
}
