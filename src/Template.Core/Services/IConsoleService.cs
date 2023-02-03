using Microsoft.Extensions.Hosting;

namespace Template.Core.Services;

/// <summary>
///     Console service interface
/// </summary>
public interface IConsoleService : IHostedService, IDisposable
{
    CancellationTokenSource CancellationSource { get; }
}