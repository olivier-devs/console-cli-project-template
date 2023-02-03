using Spectre.Console.Cli;

namespace Template.Spectre;

/// <summary>
///     The type resolver.
/// </summary>
internal class SpectreTypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpectreTypeResolver" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <exception cref="ArgumentNullException">The service provider is null</exception>
    public SpectreTypeResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    ///     Releases all resources used by the <see cref="SpectreTypeResolver" /> instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Resolves the specified type.
    /// </summary>
    /// <param name="type">The service type</param>
    /// <returns>The service object resolved</returns>
    public object? Resolve(Type? type)
    {
        return type is null ? null : _serviceProvider.GetService(type);
    }

    /// <summary>
    ///     Releases all resources used by the <see cref="SpectreTypeResolver" /> instance.
    /// </summary>
    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (!disposing) return;
        if (_serviceProvider is not IDisposable disposable) return;

        disposable.Dispose();
        _disposed = true;
    }
}