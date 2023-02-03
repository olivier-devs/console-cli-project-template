using Spectre.Console.Cli;

namespace Template.Commands.Extensions;

/// <summary>
///     The command app configuration extensions
/// </summary>
public static class ConfiguratorExtensions
{
    /// <summary>
    ///     Add the commands to the CommandApp
    /// </summary>
    /// <param name="config">The configuration</param>
    /// <returns>The configuration</returns>
    public static IConfigurator AddCommands(this IConfigurator config)
    {
        // TODO: Add all your commands here. They will be available for the user to run.
        config.AddCommand<HelloCommand>("hello");

        return config;
    }
}