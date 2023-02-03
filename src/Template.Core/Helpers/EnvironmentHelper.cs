using System.Diagnostics;
using System.Reflection;

namespace Template.Core.Helpers;

/// <summary>
///     The environment helper
/// </summary>
public static class EnvironmentHelper
{
    /// <summary>
    ///     Gets the application name
    /// </summary>
    /// <returns>The application name</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetApplicationName()
    {
        var assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Entry assembly not found");
        return assembly.GetName().Name ??
               Path.GetFileNameWithoutExtension(assembly.Location);
    }

    /// <summary>
    ///     Gets the application version
    /// </summary>
    /// <returns>The application version</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetApplicationVersion()
    {
        var assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Entry assembly not found");
        return (assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version?.ToString()
                ?? FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion)!;
    }

    /// <summary>
    ///     Gets the command line arguments
    /// </summary>
    /// <returns>The list of command line arguments</returns>
    public static IEnumerable<string> GetCommandLineArguments()
    {
        return Environment.GetCommandLineArgs().Skip(1);
    }

    /// <summary>
    ///     Determines if the current environment is production.
    /// </summary>
    /// <returns>A boolean indicating if the current environment is production</returns>
    public static bool IsProduction()
    {
        return IsEnvironment(Constants.Environments.Production);
    }

    /// <summary>
    ///     Determines if the current environment is development.
    /// </summary>
    /// <returns>A boolean indicating if the current environment is development</returns>
    public static bool IsDevelopment()
    {
        return IsEnvironment(Constants.Environments.Development);
    }

    /// <summary>
    ///     Determines if the current environment is staging.
    /// </summary>
    /// <returns>A boolean indicating if the current environment is staging</returns>
    public static bool IsStaging()
    {
        return IsEnvironment(Constants.Environments.Staging);
    }

    /// <summary>
    ///     Determines if the current environment is QA.
    /// </summary>
    /// <returns>A boolean indicating if the current environment is QA</returns>
    public static bool IsQa()
    {
        return IsEnvironment(Constants.Environments.Qa);
    }

    /// <summary>
    ///     Determines if the current environment matches the provided environment name.
    /// </summary>
    /// <param name="environmentName">The environment name to compare with the current environment</param>
    /// <returns>A boolean indicating if the current environment matches the provided environment name</returns>
    public static bool IsEnvironment(string environmentName)
    {
        if (string.IsNullOrWhiteSpace(environmentName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(environmentName));

        return environmentName.Equals(GetEnvironment(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Gets the current environment.
    /// </summary>
    /// <returns>The current environment name</returns>
    public static string GetEnvironment()
    {
        var environment =
            Environment.GetEnvironmentVariable(Constants.Environments.DotNetEnvironment)
            ?? Environment.GetEnvironmentVariable(Constants.Environments.AspNetCoreEnvironment)!;

        if (string.IsNullOrWhiteSpace(environment))
            throw new Exception(
                $"Missing environment variable {Constants.Environments.DotNetEnvironment} or {Constants.Environments.AspNetCoreEnvironment}.");

        return environment;
    }
}