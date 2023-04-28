namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to initialize an <see cref="IDatabase"/>
/// </summary>
public interface IDatabaseInitializer
{

    /// <summary>
    /// Initializes the specified <see cref="IDatabase"/>
    /// </summary>
    /// <param name="database">The <see cref="IDatabase"/> to initialize</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    Task InitializeAsync(IDatabase database, CancellationToken cancellationToken = default);

}
