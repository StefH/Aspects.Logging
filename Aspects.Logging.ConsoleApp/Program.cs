using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Aspects.Logging.ConsoleApp;

internal static class Program
{
    static async Task Main()
    {
        var serviceProvider = RegisterServices();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .CreateLogger();

        var testLogic = serviceProvider.GetRequiredService<TestLogic>();
        await testLogic.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    private static ServiceProvider RegisterServices()
    {
        var configuration = SetupConfiguration();
        var services = new ServiceCollection();

        services.AddLogging(configure => configure.AddSerilog());

        services.AddSingleton(configuration);
        services.AddSingleton<TestLogic>();

        services.RegisterAspectLogging(configuration.GetRequiredSection("AspectLoggingOptions"));

        return services.BuildServiceProvider();
    }

    private static IConfiguration SetupConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    }
}