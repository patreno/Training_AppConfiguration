using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppWithDynamicPollConfig
{
    class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; } = null!;

        public IConfigurationRefresher Refresher { get; private set; } = null!;

        public Startup()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(options =>
            {
                string? endpoint = Environment.GetEnvironmentVariable("Training_AppConfig_Endpoint");
                Console.WriteLine($"Endpoint: {endpoint}");
                options.Connect(new Uri(endpoint ?? ""), new DefaultAzureCredential())
                        .ConfigureRefresh(refresh =>
                        {
                            refresh.Register("TestApp:Settings:Message")
                                   .SetRefreshInterval(TimeSpan.FromSeconds(30));
                        });
                Refresher = options.GetRefresher();
            });

            Configuration = builder.Build();

            var services = new ServiceCollection();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
