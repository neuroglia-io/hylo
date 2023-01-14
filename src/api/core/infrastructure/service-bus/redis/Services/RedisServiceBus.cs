using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reactive.Subjects;
using System.Threading.Channels;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Represents the default, REDIS-based implementation of the <see cref="IServiceBus"/> interface
/// </summary>
public class RedisServiceBus
    : BackgroundService, IServiceBus
{

    /// <summary>
    /// Gets the key of the channel used to publish and subscribe to <see cref="V1ApiServerMessage"/>s
    /// </summary>
    public const string RedisChannelKey = "api-server-channel";

    /// <summary>
    /// Initializes a new <see cref="RedisServiceBus"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="redis">The current <see cref="IConnectionMultiplexer"/></param>
    public RedisServiceBus(ILoggerFactory loggerFactory, IConnectionMultiplexer redis)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Redis = redis;
        this.RedisSubscriber = this.Redis.GetSubscriber();
        this.Channel = System.Threading.Channels.Channel.CreateUnbounded<V1ApiServerMessage>();
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the current <see cref="IConnectionMultiplexer"/>
    /// </summary>
    protected IConnectionMultiplexer Redis { get; }

    /// <summary>
    /// Gets the service used to subscribe to messages publishes on REDIS
    /// </summary>
    protected ISubscriber RedisSubscriber { get; }

    /// <summary>
    /// Gets the service used to stream outbound <see cref="V1ApiServerMessage"/>s
    /// </summary>
    protected Channel<V1ApiServerMessage> Channel { get; }

    /// <summary>
    /// Gets the service used to observe consumed <see cref="V1ApiServerMessage"/>s
    /// </summary>
    protected ISubject<V1ApiServerMessage> MessageStream { get; } = new Subject<V1ApiServerMessage>();

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this.RedisSubscriber.SubscribeAsync(RedisChannelKey, this.OnRedisMessage);
        _ = this.ProcessOutboundMessagesAsync(stoppingToken);
    }

    /// <summary>
    /// Processes pending outbound <see cref="V1ApiServerMessage"/>s
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task ProcessOutboundMessagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await this.Channel.Reader.ReadAsync(cancellationToken);
                if (message == null) continue;
                try
                {
                    this.Logger.LogDebug("Publishing a new message on the API server channel...");
                    var json = Serializer.Json.Serialize(message);
                    await this.RedisSubscriber.PublishAsync(RedisChannelKey, json);
                    this.Logger.LogDebug("Message successfully published");
                }
                catch(Exception ex)
                {
                    this.Logger.LogError("An error occured while publishing an outbound API server message: {ex}", ex);
                }
            }
        }
        catch(TaskCanceledException) { }
    }

    /// <inheritdoc/>
    public virtual async Task PublishAsync(V1ApiServerMessage message, CancellationToken cancellationToken = default)
    {
        await this.Channel.Writer.WriteAsync(message, cancellationToken);
    }

    protected virtual void OnRedisMessage(RedisChannel channel, RedisValue value)
    {
        var json = value.ToString();
        if (string.IsNullOrWhiteSpace(json))
        {
            this.Logger.LogWarning("Received on empty message on the API server channel. Skipping.");
            return;
        }
        var message = Serializer.Json.Deserialize<V1ApiServerMessage>(json);
        if(message == null)
        {
            this.Logger.LogWarning("Received on empty message on the API server channel. Skipping.");
            return;
        }
        if (message.SourceId == Environment.MachineName)
        {
            this.Logger.LogDebug("Received a round-trip message published by the application. Skipping.");
            return;
        }
        this.Logger.LogDebug("Received a message of type '{messageType}' from API server '{apiServer}'", message.Type, message.SourceId);
        this.MessageStream.OnNext(message);
    }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<V1ApiServerMessage> observer)
    {
        return this.MessageStream.Subscribe(observer);
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        this.MessageStream.OnCompleted();
        base.Dispose();
        GC.SuppressFinalize(this);
    }

}
