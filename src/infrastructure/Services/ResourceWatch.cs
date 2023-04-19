namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of a service used to watch events of <see cref="IResource"/>s of the specified type
/// </summary>
public class ResourceWatch
    : IResourceWatch
{

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="observable">The service used to observe the <see cref="IResourceWatchEvent{TResource}"/>s emitted by <see cref="IResource"/>s of the specified type</param>
    /// <param name="disposeObservable">A boolean used to configure whether or not to dispose of the <see cref="Observable"/> when disposing of the <see cref="ResourceWatch"/></param>
    public ResourceWatch(IObservable<IResourceWatchEvent> observable, bool disposeObservable)
    {
        this.Observable = observable;
        this.DisposeObservable = disposeObservable;
    }

    /// <summary>
    /// Gets the service used to observe the <see cref="IResourceWatchEvent{TResource}"/>s emitted by <see cref="IResource"/>s of the specified type
    /// </summary>
    protected IObservable<IResourceWatchEvent> Observable { get; }

    /// <summary>
    /// Gets a boolean used to configure whether or not to dispose of the <see cref="Observable"/> when disposing of the <see cref="ResourceWatch"/>
    /// </summary>
    internal protected bool DisposeObservable { get; }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<IResourceWatchEvent> observer) => this.Observable.Subscribe(observer);

    /// <summary>
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposing || this._disposed) return;
        if (this.DisposeObservable)
        {
            switch (this.Observable)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
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
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || this._disposed) return;
        if (this.DisposeObservable && this.Observable is IDisposable disposable) disposable.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}

/// <summary>
/// Represents the default implementation of a service used to watch events of <see cref="IResource"/>s of the specified type
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/> to watch the events of</typeparam>
public class ResourceWatch<TResource>
    : IResourceWatch<TResource>
    where TResource : class, IResource, new()
{

    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="observable">The service used to observe the <see cref="IResourceWatchEvent{TResource}"/>s emitted by <see cref="IResource"/>s of the specified type</param>
    /// <param name="disposeObservable">A boolean used to configure whether or not to dispose of the <see cref="Observable"/> when disposing of the <see cref="ResourceWatch"/></param>
    public ResourceWatch(IObservable<IResourceWatchEvent<TResource>> observable, bool disposeObservable)
    {
        this.Observable = observable;
        this.DisposeObservable = disposeObservable;
    }

    /// <summary>
    /// Initializes a new <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="observable">The service used to observe the <see cref="IResourceWatchEvent"/>s emitted by <see cref="IResource"/>s of the specified type</param>
    /// <param name="disposeObservable">A boolean used to configure whether or not to dispose of the <see cref="Observable"/> when disposing of the <see cref="ResourceWatch"/></param>
    public ResourceWatch(IObservable<IResourceWatchEvent> observable, bool disposeObservable) : this(observable.Select(e => e.OfType<TResource>()), disposeObservable) { }

    /// <summary>
    /// Gets the service used to observe the <see cref="IResourceWatchEvent{TResource}"/>s emitted by <see cref="IResource"/>s of the specified type
    /// </summary>
    protected IObservable<IResourceWatchEvent<TResource>> Observable { get; }

    /// <summary>
    /// Gets a boolean used to configure whether or not to dispose of the <see cref="Observable"/> when disposing of the <see cref="ResourceWatch"/>
    /// </summary>
    internal protected bool DisposeObservable { get; }

    /// <inheritdoc/>
    public virtual IDisposable Subscribe(IObserver<IResourceWatchEvent<TResource>> observer) => this.Observable.Subscribe(observer);

    /// <summary>
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    /// <returns>A new awaitable <see cref="ValueTask"/></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!disposing || this._disposed) return;

        switch (this.Observable)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
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
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || this._disposed) return;
        if (this.DisposeObservable && this.Observable is IDisposable disposable) disposable.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}