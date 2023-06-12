namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents an <see cref="IPlugin"/>-based implementation of the <see cref="IDatabaseProvider"/> interface
/// </summary>
public class PluginDatabaseProvider
    : IDatabaseProvider
{

    /// <summary>
    /// Initializes a new <see cref="PluginDatabaseProvider"/>
    /// </summary>
    /// <param name="pluginManager">The service used to manage <see cref="IPlugin"/>s</param>
    public PluginDatabaseProvider(IPluginManager pluginManager)
    {
        this.PluginManager = pluginManager;
    }

    /// <summary>
    /// Gets the service used to manage <see cref="IPlugin"/>s
    /// </summary>
    protected IPluginManager PluginManager { get; }

    /// <inheritdoc/>
    public IDatabase GetDatabase()
    {
        var plugin = this.PluginManager.FindPluginAsync<IDatabaseProvider>().GetAwaiter().GetResult() ?? throw new NullReferenceException("Failed to find a database provider plugin");
        return plugin.GetDatabase();
    }

}