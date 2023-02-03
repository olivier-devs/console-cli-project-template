using System.ComponentModel;
using Spectre.Console.Cli;

namespace Template.Commands.Settings;

/// <summary>
///     The shared command settings
/// </summary>
public abstract class SharedCommandSettings : CommandSettings
{
    [CommandOption("-w|--wait-for-debugger", IsHidden = true)]
    [Description("When specified it blocks execution until the Visual Studio debugger is attached")]
    public bool WaitForDebugger { get; set; }
}