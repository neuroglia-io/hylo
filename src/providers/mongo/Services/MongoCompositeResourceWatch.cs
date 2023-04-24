using Hylo.Infrastructure.Services;
using MongoDB.Driver;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Hylo.Providers.Mongo;

/// <summary>
/// Represents a composite <see cref="MongoResourceWatch"/>
/// </summary>
public class MongoCompositeResourceWatch
    : IResourceWatch
{

    readonly CompositeDisposable _CompositeDisposable;
    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="MongoResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="watches">A <see cref="List{T}"/> containing the <see cref="MongoResourceWatch"/>es the <see cref="MongoResourceWatch{TResource}"/> is made out of</param>
    public MongoCompositeResourceWatch(IEnumerable<MongoResourceWatch> watches)
    {
        this._CompositeDisposable = new CompositeDisposable(watches);
        this.Stream = Observable.Using(() => this._CompositeDisposable, _ => watches.Merge()).TakeUntil(r => this._disposed);
    }

    /// <summary>
    /// Gets the <see cref="IObservable{T}"/> used to observe the merged <see cref="IResourceWatchEvent"/>s produced by the watches the <see cref="MongoCompositeResourceWatch"/> is made out of
    /// </summary>
    protected IObservable<IResourceWatchEvent> Stream { get; }

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<IResourceWatchEvent> observer) => this.Stream.Subscribe(observer);

    /// <summary>
    /// Disposes of the <see cref="MongoResourceWatch"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoResourceWatch"/> is being disposed of</param>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._CompositeDisposable.Dispose();
            }
            this._disposed = true;
        }
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(disposing: true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || this._disposed) return;
        this._CompositeDisposable.Dispose();
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
/// Represents a composite <see cref="MongoResourceWatch{TResource}"/>
/// </summary>
/// <typeparam name="TResource">The type of <see cref="IResource"/> to watch</typeparam>
public class MongoCompositeResourceWatch<TResource>
    : IResourceWatch<TResource>
    where TResource : class, IResource, new()
{

    readonly CompositeDisposable _CompositeDisposable;
    bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="MongoResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="watches">A <see cref="List{T}"/> containing the <see cref="MongoResourceWatch"/>es the <see cref="MongoResourceWatch{TResource}"/> is made out of</param>
    public MongoCompositeResourceWatch(IEnumerable<MongoResourceWatch<TResource>> watches)
    {
        this._CompositeDisposable = new CompositeDisposable(watches);
        this.Stream = Observable.Using(() => this._CompositeDisposable, _ => watches.Merge()).TakeUntil(r => this._disposed);
    }

    /// <summary>
    /// Gets the <see cref="IObservable{T}"/> used to observe the merged <see cref="IResourceWatchEvent"/>s produced by the watches the <see cref="MongoCompositeResourceWatch{TResource}"/> is made out of
    /// </summary>
    protected IObservable<IResourceWatchEvent<TResource>> Stream { get; }

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<IResourceWatchEvent<TResource>> observer) => this.Stream.Subscribe(observer);

    /// <summary>
    /// Disposes of the <see cref="MongoResourceWatch"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoResourceWatch"/> is being disposed of</param>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                this._CompositeDisposable.Dispose();
            }
            this._disposed = true;
        }
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsync(disposing: true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="ResourceWatch{TResource}"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatch{TResource}"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || this._disposed) return;
        this._CompositeDisposable.Dispose();
        this._disposed = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}