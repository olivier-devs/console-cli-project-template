using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Template.Core;
using Template.Core.Helpers;
using Template.Extensions;

// Entry point, create host and run it
using var host = CreateHostBuilder(args).Build();
await host.RunAsync();
return Environment.ExitCode;

// Create host builder
IHostBuilder CreateHostBuilder(string[] args)
{
    // .NET Core apps configure and launch a host. The host is responsible for app startup and lifetime management.
    return Host
        // CreateDefaultBuilder() to setup a Host with default settings
        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder?view=dotnet-plat-ext-7.0
        //
        // The following defaults are applied to the returned HostBuilder:
        //    - set the ContentRootPath to the result of GetCurrentDirectory()
        //    - load host IConfiguration from "DOTNET_" prefixed environment variables
        //    - load app IConfiguration from 'appsettings.json' and 'appsettings.[EnvironmentName].json'
        //    - load app IConfiguration from User Secrets when EnvironmentName is 'Development' using the entry assembly
        //    - load app IConfiguration from environment variables
        //    - configure the ILoggerFactory to log to the console, debug, and event source output
        //    - enables scope validation on the dependency injection container when EnvironmentName is 'Development'
        .CreateDefaultBuilder(args)
        // Used to configure the properties of the IHostEnvironment; can be called multiple times
        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostbuilder.configurehostconfiguration?view=dotnet-plat-ext-7.0
        .ConfigureHostConfiguration(config => { })

        // The configuration created by ConfigureAppConfiguration is available at HostBuilderContext.Configuration
        // for subsequent operations and as a service from DI. The host configuration is also added to the app configuration.
        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostbuilder.configureappconfiguration?view=dotnet-plat-ext-7.0
        .ConfigureAppConfiguration((hostContext, config) =>
        {
            config
                // For single .exes AppContext.BaseDirectory is where .exe is, not where it gets unpacked or current directory.
                .SetBasePath(AppContext.BaseDirectory)

                // Add application name and version to InMemoryCollection;
                .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                {
                    new(Constants.Environments.Keys.AppName, EnvironmentHelper.GetApplicationName()),
                    new(Constants.Environments.Keys.AppVersion, EnvironmentHelper.GetApplicationVersion())
                })
                .AddAppSettings()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .AddUserSecrets();
        })
        // Add all services required to run the application
        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostbuilder.configureservices?view=dotnet-plat-ext-7.0
        .ConfigureServices((hostContext, services) => { services.ConfigureApplication(); })
        // Configure the logging logic
        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostbuilder.configurelogging?view=dotnet-plat-ext-7.0
        .ConfigureLogging((hostContext, logging) =>
        {
            logging.ClearProviders()
                .AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                .AddDebug()
                .AddConsole();
        });
}