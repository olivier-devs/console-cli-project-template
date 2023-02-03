using Microsoft.Extensions.Configuration;
using Template.Core.Helpers;

namespace Template.Extensions;

/// <summary>
///   The configuration builder extensions
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    ///  Add the app settings json files
    /// </summary>
    /// <param name="config">The configuration</param>
    /// <returns>The configuration</returns>
    public static IConfigurationBuilder AddAppSettings(this IConfigurationBuilder config)
    {
        config
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{EnvironmentHelper.GetEnvironment()}.json", true, true);
        return config;
    }

    /// <summary>
    ///  Add the user secrets if the environment is development
    /// </summary>
    /// <param name="config">The configuration</param>
    /// <returns>The configuration</returns>
    public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder config)
    {
        if (EnvironmentHelper.IsDevelopment())
            config.AddUserSecrets<Program>(true, true);

        return config;
    }
}