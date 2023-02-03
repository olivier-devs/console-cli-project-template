using System.Diagnostics;
using Spectre.Console.Cli;
using Template.Commands.Settings;
using Template.Core;

namespace Template.Spectre;

/// <summary>
///     The wait for debugger interceptor
/// </summary>
internal class WaitForDebuggerInterceptor : ICommandInterceptor
{
    /// <summary>
    ///     Intercept the command execution
    /// </summary>
    /// <param name="context">The command context object</param>
    /// <param name="settings">The command settings object</param>
    public void Intercept(CommandContext context, CommandSettings settings)
    {
        if (settings is not SharedCommandSettings { WaitForDebugger: true })
            return;
        Console.WriteLine("Waiting for debugger to attach...");
        while (!Debugger.IsAttached) Thread.Sleep(Constants.WaitDebuggerTimeout);
    }
}