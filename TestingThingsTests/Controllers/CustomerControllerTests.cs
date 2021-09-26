using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestingThings;
using TestingThings.Clients.Http;
using TestingThings.Controllers;
using TestingThings.Models;
using TestingThingsTests.Common;
using Xunit;

namespace TestingThingsTests.Controllers
{
    public class CustomerControllerTests
    {
        [Theory, AutoMoqData]
        public async Task Get_ValidResponse_CallsQueryService(
            [Frozen] ICustomerQueryClient customerQueryClient,
            IEnumerable<Customer> customers,
            [Greedy] CustomerController sut)
        {
            customerQueryClient.AsMock()
                .Setup(c => c.GetCustomers(It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            IEnumerable<Customer> customerResponse = await sut.Get(CancellationToken.None);

            customerResponse.Should()
                .BeEquivalentTo(customers, "it should return equivilent customers");
        }

        [Theory, AutoMoqData]
        public async Task Get_ErrorResponse_ThrowsErrorWhenQueryThrows(
            [Frozen] ICustomerQueryClient customerQueryClient,
            NullReferenceException exception,
            [Greedy] CustomerController sut)
        {
            customerQueryClient.AsMock()
                .Setup(c => c.GetCustomers(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            Func<Task> act = async () => await sut.Get(CancellationToken.None);
            
            await act.Should()
                .ThrowAsync<NullReferenceException>("it should throw the same exception type")
                .WithMessage(exception.Message, "it should have the same message");
        }
    }
}
