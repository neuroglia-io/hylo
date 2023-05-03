namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to control <see cref="IResource"/>s of the specified type
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/>s to control</typeparam>
public interface IResourceController<TResource>
    : IHostedService, IObservable<IResourceWatchEvent<TResource>>, IDisposable, IAsyncDisposable
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the current state of controlled <see cref="IResource"/>s
    /// </summary>
    List<TResource> Resources { get; }

}