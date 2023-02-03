using Template.Core.Helpers;

namespace Template.Helpers;

/// <summary>
///     Console helper class.
/// </summary>
internal static class ConsoleHelper
{
    /// <summary>
    ///     Sets the console title.
    /// </summary>
    public static void SetTitle()
    {
        Console.Title = $"{EnvironmentHelper.GetApplicationName()} - {EnvironmentHelper.GetApplicationVersion()}";
    }
}