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
    /// <param name="databaseProvider">The service used to provide the <see cref="IDatabase"/> to initialize</param>
    public DatabaseInitializer(IDatabaseProvider databaseProvider)
    {
        this.DatabaseProvider = databaseProvider;
    }

    /// <summary>
    /// Gets the service used to provide the <see cref="IDatabase"/> to initialize
    /// </summary>
    protected IDatabaseProvider DatabaseProvider { get; }

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken stoppingToken) => this.InitializeAsync(stoppingToken);

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (await this.DatabaseProvider.GetDatabase().InitializeAsync(cancellationToken).ConfigureAwait(false)) await this.SeedAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Seeds the <see cref="IDatabase"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
