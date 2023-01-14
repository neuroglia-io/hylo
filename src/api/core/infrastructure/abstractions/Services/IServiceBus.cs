namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to publish and subscribe to messages exchanged between Synapse API server instances
/// </summary>
public interface IServiceBus
    : IObservable<V1ApiServerMessage>
{

    /// <summary>
    /// Publishes a new <see cref="V1ApiServerMessage"/>
    /// </summary>
    /// <param name="message">The <see cref="V1ApiServerMessage"/> to publish</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task PublishAsync(V1ApiServerMessage message, CancellationToken cancellationToken = default);

}