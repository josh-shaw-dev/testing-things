using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using TestingThings.Clients.Http;
using TestingThings.Configuration;

namespace TestingThings.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<ICustomerQueryClient, CustomerQueryClient>((serviceProvider, client) =>
                {
                    SomeConfig someConfig = serviceProvider.GetRequiredService<IOptions<SomeConfig>>().Value;
                    client.BaseAddress = someConfig.SomeUri;
                })
                // Retry transient network issues
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.RetryAsync(2));
        }

        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            // Use IOptions for config
            services.AddOptions<SomeConfig>(SomeConfig.SectionName)
                .Bind(configuration.GetSection(SomeConfig.SectionName))
                .ValidateDataAnnotations();
        }
    }
}
