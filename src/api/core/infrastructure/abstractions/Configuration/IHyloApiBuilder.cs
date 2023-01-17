using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hylo.Api.Configuration;

/// <summary>
/// Defines the fundamentals of a service used to build an Hylo API
/// </summary>
public interface IHyloApiBuilder
{

    /// <summary>
    /// Gets the current <see cref="IHostEnvironment"/>
    /// </summary>
    IHostEnvironment Environment { get; }

    /// <summary>
    /// Gets the current <see cref="IConfiguration"/>
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the current <see cref="IServiceCollection"/>
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the service used to manage well-known <see cref="V1Resource"/>s
    /// </summary>
    IResourceRegistry Resources { get; }

    /// <summary>
    /// Builds the Hylo API
    /// </summary>
    void Build();

}

/// <summary>
/// Represents the default implementation of the <see cref="IHyloApiBuilder"/> interface
/// </summary>
public class HyloApiBuilder
    : IHyloApiBuilder
{

    /// <summary>
    /// Initializes a new <see cref="HyloApiBuilder"/>
    /// </summary>
    /// <param name="environment">The current <see cref="IHostEnvironment"/></param>
    /// <param name="configuration">The current <see cref="IConfiguration"/></param>
    /// <param name="services">The current <see cref="IServiceCollection"/></param>
    /// <param name="resources">The service used to register well-known Hylo API resources</param>
    public HyloApiBuilder(IHostEnvironment environment, IConfiguration configuration, IServiceCollection services, IResourceRegistry resources)
    {
        this.Environment = environment;
        this.Configuration = configuration;
        this.Services = services;
        this.Resources = resources;
    }

    /// <inheritdoc/>
    public IHostEnvironment Environment { get; }

    /// <inheritdoc/>
    public IConfiguration Configuration { get; }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public IResourceRegistry Resources { get; }

    /// <inheritdoc/>
    public virtual void Build()
    {
        
    }

}