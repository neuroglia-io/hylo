namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to control an <see cref="IPlugin"/>
/// </summary>
public interface IPluginController
    : IAsyncDisposable
{

    /// <summary>
    /// Gets the event fired whenever the <see cref="IPluginController"/> has been disposed of
    /// </summary>
    event EventHandler? Disposed; 

    /// <summary>
    /// Gets the path to the controlled <see cref="IPlugin"/>'s metadata file
    /// </summary>
    string MetadataFilePath { get; }

    /// <summary>
    /// Gets an object used to describe the controlled <see cref="IPlugin"/>
    /// </summary>
    PluginMetadata Metadata { get; }

    /// <summary>
    /// Gets the controlled <see cref="IPlugin"/>, if loaded
    /// </summary>
    IPlugin? Plugin { get; }

    /// <summary>
    /// Loads and initializes the <see cref="IPlugin"/>
    /// </summary>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> used to manage the handled <see cref="IPlugin"/>'s lifetime</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    ValueTask LoadAsync(CancellationToken stoppingToken);

    /// <summary>
    /// Unloads the <see cref="IPlugin"/>
    /// </summary>
    /// <param name="cancellationToken">Unloads the <see cref="IPlugin"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    ValueTask UnloadAsync(CancellationToken cancellationToken = default);

}
