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
    IHyloApiResourceInitializerOptionsBuilder Resources { get; }

    /// <summary>
    /// Builds the Hylo API
    /// </summary>
    void Build();

}
