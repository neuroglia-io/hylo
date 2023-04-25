using Hylo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hylo.Providers.FileSystem.Services;

/// <summary>
/// Represents a file system based implementation of the <see cref="IDatabaseProvider"/> interface.
/// </summary>
/// <remarks>Should only be used for test purposes</remarks>
public class FileSystemDatabaseProvider
    : IPlugin, IDatabaseProvider
{

    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="FileSystemDatabaseProvider"/>
    /// </summary>
    /// <param name="applicationServices">The current application's services</param>
    public FileSystemDatabaseProvider(IServiceProvider applicationServices)
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
        services.AddSingleton(this.ApplicationServices.GetRequiredService<IConfiguration>());
        services.AddSingleton(this.ApplicationServices.GetRequiredService<ILoggerFactory>());
        services.AddMemoryCache();
        services.AddSingleton<FileSystemDatabase>();
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<FileSystemDatabase>());
        this.PluginServices = services.BuildServiceProvider();
        foreach(var hostedService in this.PluginServices.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public virtual IDatabase GetDatabase()
    {
        if (this.PluginServices == null) return this.ApplicationServices.GetRequiredService<FileSystemDatabase>();
        else return this.PluginServices.GetRequiredService<FileSystemDatabase>();
    }

    /// <summary>
    /// Disposes of the <see cref="FileSystemDatabaseProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="FileSystemDatabaseProvider"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (this._disposed || !disposing) return;
        if (this.PluginServices != null)
        {
            switch (this.PluginServices)
            {
                case IAsyncDisposable asyncDisposable: await asyncDisposable.DisposeAsync().ConfigureAwait(false); break;
                case IDisposable disposable: disposable.Dispose(); break;
            }
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
    /// Disposes of the <see cref="FileSystemDatabaseProvider"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="FileSystemDatabaseProvider"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this._disposed || !disposing) return;
        if (this.PluginServices != null)
        {
            switch (this.PluginServices)
            {
                case IDisposable disposable: disposable.Dispose(); break;
            }
        }
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
