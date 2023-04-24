using Hylo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hylo.Providers.Mongo.Services;

/// <summary>
/// Represents a <see href="https://www.mongodb.com/">MongoDB</see> implementation of the <see cref="IDatabaseProvider"/> interface
/// </summary>
public class MongoDatabaseProvider
    : IPlugin, IDatabaseProvider
{

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="MongoDatabaseProvider"/>
    /// </summary>
    /// <param name="applicationServices">The current application's services</param>
    public MongoDatabaseProvider(IServiceProvider applicationServices)
    {
        this.ApplicationServices = applicationServices;
    }

    /// <summary>
    /// Gets the current application's services
    /// </summary>
    protected IServiceProvider ApplicationServices { get; }

    /// <summary>
    /// Gets the plugin's services
    /// </summary>
    protected IServiceProvider? PluginServices { get; set; }

    /// <inheritdoc/>
    public virtual async ValueTask InitializeAsync(CancellationToken cancellationToken = default)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMongoClient(this.ApplicationServices.GetRequiredService<IConfiguration>().GetConnectionString(MongoDatabase.ConnectionStringName)!);
        services.AddSingleton<MongoDatabase>();
        this.PluginServices = services.BuildServiceProvider();
        foreach (var hostedService in this.PluginServices.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public virtual IDatabase GetDatabase()
    {
        if (this.PluginServices == null) return this.ApplicationServices.GetRequiredService<MongoDatabase>();
        else return this.PluginServices.GetRequiredService<MongoDatabase>();
    }

    /// <summary>
    /// Disposes of the <see cref="MongoDatabaseProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoDatabaseProvider"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposing || this._disposed) return;
        switch (this.PluginServices)
        {
            case IAsyncDisposable asyncDisposable: await asyncDisposable.DisposeAsync().ConfigureAwait(false); break;
            case IDisposable disposable: disposable.Dispose(); break;
        }
        this._disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="MongoDatabaseProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoDatabaseProvider"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || this._disposed) return;
        if(this.PluginServices is IDisposable disposable) disposable.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

}