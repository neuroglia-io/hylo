using Hylo.Infrastructure;
using Hylo.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Providers.Redis.Services;

/// <summary>
/// Represents a <see href="https://redis.io/">Redis</see> implementation of the <see cref="IDatabaseProvider"/> interface
/// </summary>
[Plugin(typeof(IDatabaseProvider), typeof(RedisDatabaseProviderPluginBootstrapper))]
public class RedisDatabaseProvider
    : IDatabaseProvider
{

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="RedisDatabaseProvider"/>
    /// </summary>
    /// <param name="applicationServices">The current application's services</param>
    public RedisDatabaseProvider(IServiceProvider applicationServices)
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
    public virtual IDatabase GetDatabase()
    {
        if (this.PluginServices == null) return this.ApplicationServices.GetRequiredService<RedisDatabase>();
        else return this.PluginServices.GetRequiredService<RedisDatabase>();
    }

    /// <summary>
    /// Disposes of the <see cref="RedisDatabaseProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="RedisDatabaseProvider"/> is being disposed of</param>
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
    /// Disposes of the <see cref="RedisDatabaseProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="RedisDatabaseProvider"/> is being disposed of</param>
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
