namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to publish and subscribe to <see cref="V1ResourceEvent"/>s
/// </summary>
public interface IResourceEventBus
    : IObservable<V1ResourceEvent>
{

    /// <summary>
    /// Publishes the specified <see cref="V1ResourceEvent"/>
    /// </summary>
    /// <param name="e">The <see cref="V1ResourceEvent"/> to publish</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task PublishAsync(V1ResourceEvent e, CancellationToken cancellationToken = default);

}