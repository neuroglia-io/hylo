using Microsoft.Extensions.Hosting;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to initialize the resources of an Hylo API
/// </summary>
public interface IApiResourcesInitializer
    : IHostedService
{

    /// <summary>
    /// Initializes Hylo API resources
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task InitializeAsync(CancellationToken cancellationToken = default);

}
