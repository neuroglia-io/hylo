namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IDatabaseInitializer"/> interface
/// </summary>
public class DatabaseInitializer
    : IHostedService, IDatabaseInitializer
{

    /// <summary>
    /// Initializes a new <see cref="DatabaseInitializer"/>
    /// </summary>
    /// <param name="database">The <see cref="IDatabase"/> to initialize</param>
    public DatabaseInitializer(IDatabase database)
    {
        this.Database = database;
    }

    /// <summary>
    /// Gets the <see cref="IDatabase"/> to initialize
    /// </summary>
    protected IDatabase Database { get; }

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken stoppingToken)
    {
        return this.InitializeAsync(stoppingToken);
    }

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (await this.Database.InitializeAsync(cancellationToken).ConfigureAwait(false)) await this.SeedAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Seeds the <see cref="IDatabase"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
