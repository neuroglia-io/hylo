namespace Hylo.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to monitor the state of a specific resource
/// </summary>
public interface IResourceMonitor
    : IDisposable, IAsyncDisposable, IObservable<IResourceWatchEvent>
{

    /// <summary>
    /// Gets the current state of the monitored <see cref="IResource"/>
    /// </summary>
    IResource Resource { get; }

}

/// <summary>
/// Defines the fundamentals of a service used to monitor the state of a specific resource
/// </summary>
/// <typeparam name="TResource">The type of the <see cref="IResource"/> to monitor</typeparam>
public interface IResourceMonitor<TResource>
    : IDisposable, IAsyncDisposable, IObservable<IResourceWatchEvent<TResource>>
    where TResource : class, IResource, new()
{

    /// <summary>
    /// Gets the current state of the monitored <see cref="IResource"/>
    /// </summary>
    TResource Resource { get; }

}
