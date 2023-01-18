namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to publish and subscribe to messages exchanged between Hylo API server instances
/// </summary>
public interface IServiceBus
    : IObservable<ApiServerMessage>
{

    /// <summary>
    /// Publishes a new <see cref="ApiServerMessage"/>
    /// </summary>
    /// <param name="message">The <see cref="ApiServerMessage"/> to publish</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task PublishAsync(ApiServerMessage message, CancellationToken cancellationToken = default);

}