using ConsoleAppWithDynamicPollConfig;


var startup = new Startup();

var _configuration = startup.Configuration;
var _refresher = startup.Refresher;

Console.WriteLine(_configuration["TestApp:Settings:Message"] ?? "Hello world!");

// Wait for the user to press Enter
Console.ReadLine();

if (_refresher != null)
{
    await _refresher.TryRefreshAsync();
    Console.WriteLine(_configuration["TestApp:Settings:Message"] ?? "Hello world!");

}