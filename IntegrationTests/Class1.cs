using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IntegrationTests
{
    public class TestSetup
    {
        public TestSetup()
        {
            var serviceCollection = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  // Corrected line
                .AddJsonFile(
                    path: "appsettings.Development.json",
                    optional: false,
                    reloadOnChange: true)
                .AddUserSecrets<TestSetup>()
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }
}
