namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to watch resources
/// </summary>
public interface IResourceWatch
    : IObservable<IResourceWatchEvent>, IDisposable, IAsyncDisposable
{


}

/// <summary>
/// Defines the fundamentals of a service used to watch resources
/// </summary>
/// <typeparam name="TResource">The type of resource to watch</typeparam>
public interface IResourceWatch<TResource>
    : IObservable<IResourceWatchEvent<TResource>>, IDisposable, IAsyncDisposable
    where TResource : class, IResource, new()
{


}
