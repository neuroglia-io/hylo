using Hylo.Infrastructure;
using Hylo.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Providers.Mongo.Services;

/// <summary>
/// Represents a <see href="https://www.mongodb.com/">MongoDB</see> implementation of the <see cref="IDatabaseProvider"/> interface
/// </summary>
[Plugin(typeof(IDatabaseProvider), typeof(MongoDatabaseProviderPluginBootstrapper))]
public class MongoDatabaseProvider
    : IDatabaseProvider, IDisposable, IAsyncDisposable
{

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="MongoDatabaseProvider"/>
    /// </summary>
    /// <param name="services">The current <see cref="IServiceProvider"/></param>
    public MongoDatabaseProvider(IServiceProvider services)
    {
        this.Services = services;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider Services { get; }

    /// <inheritdoc/>
    public virtual IDatabase GetDatabase() => this.Services.GetRequiredService<MongoDatabase>();

    /// <summary>
    /// Disposes of the <see cref="MongoDatabaseProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoDatabaseProvider"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposing || this._disposed) return;
        this._disposed = true;
        await ValueTask.CompletedTask;
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
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

}
