using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reactive.Subjects;
using System.Threading.Channels;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IResourceEventBus"/> interface
/// </summary>
public class ResourceEventBus
    : BackgroundService, IResourceEventBus
{

    /// <summary>
    /// Initializes a new <see cref="ResourceEventBus"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="serviceBus">The service used to publish and subscribe to events exchanged between API server instances</param>
    /// <param name="subject"></param>
    public ResourceEventBus(ILoggerFactory loggerFactory, IServiceBus serviceBus)
    {
        Logger = loggerFactory.CreateLogger(GetType());
        ServiceBus = serviceBus;
        Channel = System.Threading.Channels.Channel.CreateUnbounded<V1ResourceEvent>();
        EventStream = new Subject<V1ResourceEvent>();
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to publish and subscribe to events exchanged between API server instances
    /// </summary>
    protected IServiceBus ServiceBus { get; }

    /// <summary>
    /// Gets the Gets the service used to stream outbound <see cref="V1ResourceEvent"/>s
    /// </summary>
    protected Channel<V1ResourceEvent> Channel { get; }

    /// <summary>
    /// Gets the <see cref="ISubject{T}"/> used to observe <see cref="V1ResourceEvent"/>s
    /// </summary>
    protected ISubject<V1ResourceEvent> EventStream { get; }

    /// <inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = ProcessOutboundMessagesAsync(stoppingToken);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Processes pending outbound <see cref="V1ResourceEvent"/>s
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ProcessOutboundMessagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var e = await Channel.Reader.ReadAsync(cancellationToken);
                if (e == null) continue;
                try
                {
                    var content = Serializer.Json.SerializeToNode(e)?.AsObject();
                    await ServiceBus.PublishAsync(new(ApiServerMessageType.ResourceEvent, Environment.MachineName, content), cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError("An error occured while publishing an outbound resource event: {ex}", ex);
                }
            }
        }
        catch (TaskCanceledException) { }
    }

    /// <inheritdoc/>
    public virtual async Task PublishAsync(V1ResourceEvent e, CancellationToken cancellationToken = default)
    {
        EventStream.OnNext(e);
        await Channel.Writer.WriteAsync(e, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<V1ResourceEvent> observer)
    {
        return EventStream.Subscribe(observer);
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        EventStream.OnCompleted();
        base.Dispose();
        GC.SuppressFinalize(this);
    }

}
