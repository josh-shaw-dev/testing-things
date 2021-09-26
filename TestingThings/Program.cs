using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace TestingThings
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            IHostEnvironment? hostEnvironment = null;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting {Product}", AssemblyInformation.Current.Product);
                IHost host = CreateHostBuilder(args)
                    .Build();
                hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
                hostEnvironment.ApplicationName = AssemblyInformation.Current.Product;

                await host.RunAsync()
                    .ConfigureAwait(false);

                return 0;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Log.Fatal(exception, $"{AssemblyInformation.Current.Product} terminated unexpectedly in {hostEnvironment?.EnvironmentName} mode.");

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        // Use local secrets manager
                        builder.AddUserSecrets<Startup>();
                    }
                    else
                    {
                        // TODO add some azure secrets here
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.AddServerHeader = false;
                    });

                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithThreadId()
                    .Enrich.WithSpan()
                    .Enrich.WithMachineName());
        }
    }
}
