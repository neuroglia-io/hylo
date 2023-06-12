using Hylo.Infrastructure;
using Hylo.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hylo.Providers.FileSystem.Services;

/// <summary>
/// Represents a file system based implementation of the <see cref="IDatabaseProvider"/> interface.
/// </summary>
/// <remarks>Should only be used for test purposes</remarks>
[Plugin(typeof(IDatabaseProvider), typeof(FileSystemDatabaseProviderPluginBoostrapper))]
public class FileSystemDatabaseProvider
    : IDatabaseProvider, IDisposable, IAsyncDisposable
{

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="FileSystemDatabaseProvider"/>
    /// </summary>
    /// <param name="services">The current <see cref="IServiceProvider"/></param>
    public FileSystemDatabaseProvider(IServiceProvider services)
    {
        this.Services = services;
    }

    /// <summary>
    /// Gets the current <see cref="IServiceProvider"/>
    /// </summary>
    protected IServiceProvider Services { get; }

    /// <inheritdoc/>
    public virtual IDatabase GetDatabase() => this.Services.GetRequiredService<FileSystemDatabase>();


    /// <summary>
    /// Disposes of the <see cref="FileSystemDatabaseProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="FileSystemDatabaseProvider"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this._disposed = true;
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="FileSystemDatabaseProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="FileSystemDatabaseProvider"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed || !disposing) return;
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
