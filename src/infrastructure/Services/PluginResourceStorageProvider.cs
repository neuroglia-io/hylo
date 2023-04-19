namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents an <see cref="IPlugin"/>-based implementation of the <see cref="IResourceStorageProvider"/> interface
/// </summary>
public class PluginResourceStorageProvider
    : IResourceStorageProvider
{

    /// <summary>
    /// Initializes a new <see cref="PluginResourceStorageProvider"/>
    /// </summary>
    /// <param name="pluginManager">The service used to manage <see cref="IPlugin"/>s</param>
    public PluginResourceStorageProvider(IPluginManager pluginManager)
    {
        this.PluginManager = pluginManager;
    }

    /// <summary>
    /// Gets the service used to manage <see cref="IPlugin"/>s
    /// </summary>
    protected IPluginManager PluginManager { get; }

    /// <inheritdoc/>
    public IResourceStorage GetResourceStorage()
    {
        var plugin = this.PluginManager.Plugins.OfType<IResourceStorageProvider>().FirstOrDefault() ?? throw new NullReferenceException("Failed to find a resource repository provider plugin");
        return plugin.GetResourceStorage();
    }

}