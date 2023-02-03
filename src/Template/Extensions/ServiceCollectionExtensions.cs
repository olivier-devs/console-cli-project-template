using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;
using Template.Spectre;

namespace Template.Extensions;

/// <summary>
///     Service collection extensions.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    ///   Configure the application.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        // Add Spectre service logic to used dependency injection
        services.AddSingleton<ITypeRegistrar>(new SpectreTypeRegistrar(services));

        // Add the main service to the service collection and configure it to run as a hosted service (console app)
        services.AddHostedService<ConsoleService>();
        
        // TODO: Add other services here. For example: services.AddSingleton<ISomeService, SomeService>();

        // for simple, one-operation CLIs, we do not want to see host lifetime messages
        // https://learn.microsoft.com/en-us/dotnet/api/Microsoft.Extensions.Hosting.ConsoleLifetimeOptions.SuppressStatusMessages?view=dotnet-plat-ext-7.0&viewFallbackFrom=net-7.0
        services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
        return services;
    }
}