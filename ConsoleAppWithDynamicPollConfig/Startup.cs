using Azure.Core.Diagnostics;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppWithDynamicPollConfig
{
    class Startup
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        //public IConfigurationRefresher Refresher { get; private set; } = null!;

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
                                   .SetRefreshInterval(TimeSpan.FromSeconds(10));
                        });
                //Refresher = options.GetRefresher();
            });

            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddAzureAppConfiguration();

            services.AddTransient<SampleConfigRefresher>();

            using var listener = new AzureEventSourceListener((eventData, text) =>
            {
                if (eventData.EventSource.Name == "Microsoft-Extensions-Configuration-AzureAppConfiguration-Refresh")
                {
                    Console.WriteLine("[{1}] {0}: {2}", eventData.EventSource.Name, eventData.Level, text);
                }
            }, EventLevel.Verbose);

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
