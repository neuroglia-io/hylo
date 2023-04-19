using System.Reflection;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of a service used to control an <see cref="IPlugin"/>
/// </summary>
public class PluginController
    : IPluginController
{

    /// <inheritdoc/>
    public event EventHandler? Disposed;

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="PluginController"/>
    /// </summary>
    /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="metadata">An object used to describe the handled <see cref="IPlugin"/></param>
    /// <param name="metadataFilePath">The path to the handled <see cref="IPlugin"/> <see cref="System.Reflection.Assembly"/> file</param>
    public PluginController(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, PluginMetadata metadata, string metadataFilePath)
    {
        this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.MetadataFilePath = metadataFilePath ?? throw new ArgumentNullException(nameof(metadataFilePath));
        this.AssemblyFilePath = Path.Combine(Path.GetDirectoryName(this.MetadataFilePath)!, this.Metadata.AssemblyFileName);
        if (!File.Exists(metadataFilePath)) throw new FileNotFoundException(nameof(metadataFilePath));
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <inheritdoc/>
    public virtual PluginMetadata Metadata { get; } = null!;

    /// <summary>
    /// Gets the path to the <see cref="IPlugin"/>'s <see cref="PluginMetadata"/> file
    /// </summary>
    public virtual string MetadataFilePath { get; } = null!;

    /// <summary>
    /// Gets the path to the <see cref="IPlugin"/>'s <see cref="System.Reflection.Assembly"/> file
    /// </summary>
    protected virtual string AssemblyFilePath { get; } = null!;

    /// <summary>
    /// Gets the <see cref="PluginAssemblyLoadContext"/> used to load <see cref="IPlugin"/> assemblies
    /// </summary>
    protected PluginAssemblyLoadContext? AssemblyLoadContext { get; set; }

    /// <summary>
    /// Gets the <see cref="System.Reflection.Assembly"/> that defines the controlled <see cref="IPlugin"/> 
    /// </summary>
    protected Assembly? Assembly { get; set; }

    /// <inheritdoc/>
    public virtual IPlugin? Plugin { get; protected set; }

    /// <summary>
    /// Gets the <see cref="PluginController"/>'s <see cref="System.Threading.CancellationTokenSource"/>
    /// </summary>
    protected CancellationTokenSource? CancellationTokenSource { get; set; }

    /// <inheritdoc/>
    public virtual async ValueTask LoadAsync(CancellationToken stoppingToken)
    {
        if (this.Plugin != null) return;
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        try
        {
            this.Logger.LogDebug("Loading plugin '{pluginName}'...", this.Metadata);
            this.AssemblyLoadContext = new PluginAssemblyLoadContext(this.AssemblyFilePath);
            this.Assembly = this.AssemblyLoadContext.LoadFromAssemblyName(new(Path.GetFileNameWithoutExtension(this.AssemblyFilePath)));
            var pluginType = this.Assembly.GetTypes().FirstOrDefault(t => t.IsClass
                && !t.IsInterface
                && !t.IsGenericTypeDefinition
                && !t.IsAbstract
                && typeof(IPlugin).IsAssignableFrom(t)) ?? throw new TypeLoadException($"Failed to find a valid plugin type in the specified assembly '{this.Assembly.FullName}'");
            this.Plugin = (IPlugin)ActivatorUtilities.CreateInstance(this.ServiceProvider, pluginType);
            await this.Plugin.InitializeAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
            this.Logger.LogDebug("Plugin '{pluginName}' successfully loaded and initialized", this.Metadata);
        }
        catch(Exception ex)
        {
            await this.UnloadAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
            this.Logger.LogError("An error occured while loading plugin '{pluginName}': {ex}", this.Metadata, ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask UnloadAsync(CancellationToken cancellationToken = default)
    {
        if (this.Plugin == null) return;
        this.Assembly = null;
        this.AssemblyLoadContext?.Unload();
        this.AssemblyLoadContext = null;
        if(this.Plugin != null) await this.Plugin.DisposeAsync().ConfigureAwait(false);
        this.Plugin = null;
    }

    /// <summary>
    /// Disposes of the <see cref="PluginController"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="PluginController"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposing || this._disposed) return;

        this.CancellationTokenSource?.Dispose();
        await this.UnloadAsync().ConfigureAwait(false);
        this.Disposed?.Invoke(this, new());

        this._disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

}
