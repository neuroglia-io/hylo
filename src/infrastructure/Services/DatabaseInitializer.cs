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
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="databaseProvider">The service used to provide the <see cref="IDatabase"/> to initialize</param>
    public DatabaseInitializer(ILoggerFactory loggerFactory, IDatabaseProvider databaseProvider)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.DatabaseProvider = databaseProvider;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to provide the <see cref="IDatabase"/> to initialize
    /// </summary>
    protected IDatabaseProvider DatabaseProvider { get; }

    /// <summary>
    /// Gets the current <see cref="IDatabase"/>
    /// </summary>
    protected IDatabase Database => this.DatabaseProvider.GetDatabase();

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken stoppingToken) => this.InitializeAsync(stoppingToken);

    /// <inheritdoc/>
    public virtual Task StopAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        this.Logger.LogDebug("Initializing resource database...");
        if (await this.Database.InitializeAsync(cancellationToken).ConfigureAwait(false))
        {
            this.Logger.LogDebug("Seeding resource database...");
            try
            {
                await this.SeedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                this.Logger.LogError("An error occured while seeding the resource database: {ex}", ex);
            }
            this.Logger.LogDebug("Resource database has been seeded");
        }
        this.Logger.LogDebug("Resource database has been successfully initialized");
    }

    /// <summary>
    /// Seeds the <see cref="IDatabase"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual Task SeedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
