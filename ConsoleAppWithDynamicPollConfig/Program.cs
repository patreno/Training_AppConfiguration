using ConsoleAppWithDynamicPollConfig;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


var startup = new Startup();

var configuration = startup.ServiceProvider.GetService<IConfiguration>();

Console.WriteLine(configuration["TestApp:Settings:Message"] ?? "Hello world!");

// Wait for the user to press Enter
Console.ReadLine();

var refresher = startup.ServiceProvider.GetService<SampleConfigRefresher>();
if (refresher != null)
{
    await refresher.RefreshConfiguration();
    Console.WriteLine(configuration["TestApp:Settings:Message"] ?? "Hello world!");

}