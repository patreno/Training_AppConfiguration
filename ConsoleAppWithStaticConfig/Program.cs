using Microsoft.Extensions.Configuration;
using Azure.Identity;

var builder = new ConfigurationBuilder();
builder.AddAzureAppConfiguration(options =>
{    
    string endpoint = Environment.GetEnvironmentVariable("Training_AppConfig_Endpoint");
    Console.WriteLine($"Endpoint: {endpoint}");
    options.Connect(new Uri(endpoint), new DefaultAzureCredential());
});

var config = builder.Build();
Console.WriteLine(config["TestApp:Settings:Message"] ?? "Hello world!");