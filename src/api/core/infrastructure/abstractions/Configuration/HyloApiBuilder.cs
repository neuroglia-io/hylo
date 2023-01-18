using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Hylo.Api.Configuration;

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
    public HyloApiBuilder(IHostEnvironment environment, IConfiguration configuration, IServiceCollection services)
    {
        this.Environment = environment;
        this.Configuration = configuration;
        this.Services = services;
        this.Resources = new HyloApiResourceInitializerOptionsBuilder();
    }

    /// <inheritdoc/>
    public virtual IHostEnvironment Environment { get; }

    /// <inheritdoc/>
    public virtual IConfiguration Configuration { get; }

    /// <inheritdoc/>
    public virtual IServiceCollection Services { get; }

    /// <inheritdoc/>
    public virtual IHyloApiResourceInitializerOptionsBuilder Resources { get; }

    /// <inheritdoc/>
    public virtual void Build()
    {
        this.Services.AddSingleton(Options.Create(this.Resources.Build()));
    }

}
