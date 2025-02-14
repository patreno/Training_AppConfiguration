using Azure.Messaging.EventGrid;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration.Extensions;
using System;
using System.Threading.Tasks;
using ConsoleAppWithDynamicPollConfig;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;

namespace TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {            
            var startup = new Startup();
            var configuration = startup.ServiceProvider.GetRequiredService<IConfiguration>();
            var refresher = startup.Refresher;
            var serviceProvider = startup.ServiceProvider;
          
            await RegisterRefreshEventHandler(refresher, serviceProvider);
            var message = configuration["TestApp:Settings:Message"];
            Console.WriteLine($"Initial value: {configuration["TestApp:Settings:Message"]}");

            while (true)
            {
                await refresher.TryRefreshAsync();

                if (configuration["TestApp:Settings:Message"] != message)
                {
                    Console.WriteLine($"New value: {configuration["TestApp:Settings:Message"]}");
                    message = configuration["TestApp:Settings:Message"];
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private static async Task RegisterRefreshEventHandler(IConfigurationRefresher refresher, IServiceProvider serviceProvider)
        {
            
            ServiceBusProcessor serviceBusProcessor = serviceProvider.GetService<IAzureClientFactory<ServiceBusProcessor>>()
            .CreateClient("ServiceBusProcessor");

            serviceBusProcessor.ProcessMessageAsync += (processMessageEventArgs) =>
            {
                // Build EventGridEvent from notification message
                EventGridEvent eventGridEvent = EventGridEvent.Parse(BinaryData.FromBytes(processMessageEventArgs.Message.Body));

                // Create PushNotification from eventGridEvent
                eventGridEvent.TryCreatePushNotification(out PushNotification pushNotification);

                // Prompt Configuration Refresh based on the PushNotification
                refresher.ProcessPushNotification(pushNotification);

                return Task.CompletedTask;
            };

            serviceBusProcessor.ProcessErrorAsync += (exceptionargs) =>
            {
                Console.WriteLine($"{exceptionargs.Exception}");
                return Task.CompletedTask;
            };

            await serviceBusProcessor.StartProcessingAsync();
        }
    }
}