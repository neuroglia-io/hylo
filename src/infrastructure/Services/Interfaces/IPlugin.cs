namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of an <see cref="IPlugin"/>
/// </summary>
public interface IPlugin
    : IDisposable, IAsyncDisposable
{

    /// <summary>
    /// Initializes the <see cref="IPlugin"/>
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    ValueTask InitializeAsync(CancellationToken cancellationToken = default);

}
