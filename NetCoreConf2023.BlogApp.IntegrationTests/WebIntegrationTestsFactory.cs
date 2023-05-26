using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace NetCoreConf2023.BlogApp.IntegrationTests;

public class WebIntegrationTestsFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        builder
            .UseEnvironment(Environments.Development)
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                var configurationValues = configuration.AsEnumerable().ToList();
                // This overrides configuration settings that were added as part
                // of building the Host (e.g. calling WebApplication.CreateBuilder(args))
                configurationBuilder.AddInMemoryCollection(configurationValues);
            });
    }
}
