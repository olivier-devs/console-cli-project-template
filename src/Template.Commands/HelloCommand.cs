using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Template.Commands;

/// <summary>
///     Sample command.
/// </summary>
public class HelloCommand : Command<HelloCommandSettings>
{
    private readonly ILogger<HelloCommand> _logger;

    public HelloCommand(ILogger<HelloCommand> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override int Execute(CommandContext context, HelloCommandSettings settings)
    {
        _logger.LogInformation("Hello World! {Message}", settings.Message);
        return 0;
    }
}

// Sample settings, must be add in Settings folder and namespace
public class HelloCommandSettings : CommandSettings
{
    [CommandOption("-m|--message <MESSAGE>")]
    public string Message { get; set; } = string.Empty;
}