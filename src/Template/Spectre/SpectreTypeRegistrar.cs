using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Template.Spectre;

/// <summary>
///     The type registrar.
/// </summary>
internal class SpectreTypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _services;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpectreTypeRegistrar" /> class.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <exception cref="ArgumentNullException">The services is null</exception>
    public SpectreTypeRegistrar(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    ///     Registers the specified service.
    /// </summary>
    /// <param name="service">The service collection</param>
    /// <param name="implementation">The implementation of the service to register</param>
    public void Register(Type service, Type implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    /// <summary>
    ///     Registers the specified service.
    /// </summary>
    /// <param name="service">The service collection</param>
    /// <param name="implementation">The implementation of the service to register</param>
    public void RegisterInstance(Type service, object implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    /// <summary>
    ///     Registers the specified service.
    /// </summary>
    /// <param name="service">The service collection</param>
    /// <param name="factory">The factory to register</param>
    /// <exception cref="ArgumentNullException">The factory is null</exception>
    public void RegisterLazy(Type service, Func<object> factory)
    {
        if (factory is null) throw new ArgumentNullException(nameof(factory));

        _services.AddSingleton(service, factory());
    }

    /// <summary>
    ///     Builds the type resolver.
    /// </summary>
    /// <returns>The type resolver build</returns>
    public ITypeResolver Build()
    {
        return new SpectreTypeResolver(_services.BuildServiceProvider());
    }
}