using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using Template.Commands.Extensions;
using Template.Core.Helpers;
using Template.Core.Services;
using Template.Helpers;
using Template.Spectre;

namespace Template;

/// <summary>
///     The console service is responsible for the application life cycle events and the console commands
/// </summary>
internal class ConsoleService : IConsoleService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<ConsoleService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed;
    private int? _exitCode;

    /// <summary>
    ///     Create the <see cref="ConsoleService" />
    /// </summary>
    /// <param name="hostApplicationLifetime">The host application lifetime</param>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="logger">The logger</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConsoleService(IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime, ILogger<ConsoleService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _hostApplicationLifetime =
            hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _disposed = false;
        CancellationSource = new CancellationTokenSource();

        // Set console title with application name and version
        ConsoleHelper.SetTitle();

        logger.LogInformation("Initializing of the {name} service", nameof(ConsoleService));
    }

    /// <summary>
    ///     The cancellation source for the console service
    /// </summary>
    public CancellationTokenSource CancellationSource { get; }

    /// <summary>
    ///     Registers the events for the application life cycle, logs the start of the ConsoleService, and returns a completed
    ///     task.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
        _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
        _hostApplicationLifetime.ApplicationStopped.Register(OnStopped);

        _logger.LogInformation("Registration of application life cycle events done");
        _logger.LogInformation("Starting of the {name} service", nameof(ConsoleService));

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Cancels the cancellation token, sets the exit code, and returns a completed task.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used by other objects or threads to receive notice of
    ///     cancellation.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationSource.Cancel();
        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Releases all resources used by the <see cref="ConsoleService" /> instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Initializes and runs the CommandApp and sets the exit code.
    ///     Handles exceptions, logs them, and stops the application.
    /// </summary>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="AggregateException">One or more errors occurred.</exception>
    /// <exception cref="Exception">An unhandled exception occurred.</exception>
    private void OnStarted()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var app = new CommandApp(_serviceProvider.GetRequiredService<ITypeRegistrar>());

                app.Configure(config =>
                {
                    // Without setting application name the .dll is rendered with extension (even if .exe is built) when --help is used
                    config.SetApplicationName(EnvironmentHelper.GetApplicationName());
                    // Set the version of the application
                    config.SetApplicationVersion(EnvironmentHelper.GetApplicationVersion());

                    // Validate examples if any are defined
                    config.ValidateExamples();

                    // Add all commands
                    config.AddCommands();

                    // Add by default the wait for debugger interceptor. This will allow to attach a debugger to the application
                    config.SetInterceptor(new WaitForDebuggerInterceptor());

                    // See https://spectreconsole.net/cli/exceptions
                    config.PropagateExceptions();
                });

                Task main = app.RunAsync(EnvironmentHelper.GetCommandLineArguments());

                await Task.WhenAny(main);

                if (CancellationSource.IsCancellationRequested || main.IsCanceled)
                    throw new OperationCanceledException(CancellationSource.Token);

                if (main.IsFaulted)
                {
                    if (main.Exception != null) throw main.Exception;

                    throw new Exception("Main task stopped due to an unhandled exception");
                }

                _exitCode = 0;
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
            {
                CancellationSource.Cancel();
                _logger.LogWarning(e, "{message}", e.Message);
                _exitCode = 0;
            }
            catch (CommandParseException e)
            {
                _logger.LogError(e, "{message}", e.Message);
            }
            catch (AggregateException e)
            {
                foreach (var ex in e.Flatten().InnerExceptions)
                    switch (ex)
                    {
                        case CommandParseException:
                            CancellationSource.Cancel();
                            _logger.LogError(e, "{message}", e.Message);
                            break;
                        case OperationCanceledException or TaskCanceledException:
                            CancellationSource.Cancel();
                            _logger.LogWarning(e, "{message}", e.Message);
                            _exitCode = 0;
                            break;
                    }
            }
            finally
            {
                // we need to call StopApplication so IHostedService lifetime finishes when our console application is done with its work/execution.
                _hostApplicationLifetime.StopApplication();
            }
        });
    }

    /// <summary>
    ///     Method to log the stop of <see cref="ConsoleService" />  with the exit code.
    /// </summary>
    private void OnStopped()
    {
        _logger.LogInformation("Stop of {name} with the code {exit code}", nameof(ConsoleService),
            Environment.ExitCode);
    }

    /// <summary>
    ///     Method to log the stopping of <see cref="ConsoleService" /> .
    /// </summary>
    private void OnStopping()
    {
        _logger.LogInformation("Stopping of {name}", nameof(ConsoleService));
    }

    /// <summary>
    ///     Method to dispose the <see cref="CancellationSource" />.
    /// </summary>
    /// <param name="disposing">Indicates if disposing is being done.</param>
    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) CancellationSource.Dispose();
        _disposed = true;
    }
}