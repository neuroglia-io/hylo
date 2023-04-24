using Hylo.Infrastructure.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hylo.Providers.Mongo;

/// <summary>
/// Represents an object used watch resource-related events
/// </summary>
public class MongoResourceWatch
    : IResourceWatch, IDisposable
{

    bool _Disposed;

    /// <summary>
    /// Initializes a new <see cref="MongoResourceWatch"/>
    /// </summary>
    /// <param name="cursor">The underlying <see cref="IChangeStreamCursor{TDocument}"/></param>
    /// <param name="cancellationTokenSource">The <see cref="System.Threading.CancellationTokenSource"/> used to cancel the <see cref="MongoResourceWatch"/></param>
    public MongoResourceWatch(IChangeStreamCursor<ChangeStreamDocument<BsonDocument>> cursor, CancellationTokenSource cancellationTokenSource)
    {
        this.Cursor = cursor;
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token);
        _ = this.IterateCursorAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<IResourceWatchEvent> observer) => this.Subject.Subscribe(observer);

    /// <summary>
    /// Gets the underlying <see cref="IChangeStreamCursor{TDocument}"/>
    /// </summary>
    protected IChangeStreamCursor<ChangeStreamDocument<BsonDocument>> Cursor { get; }

    /// <summary>
    /// Gets the <see cref="System.Threading.CancellationTokenSource"/> used to cancel the <see cref="MongoResourceWatch"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; }

    /// <summary>
    /// Gets the <see cref="Subject{T}"/> used to publish and subscribe to <see cref="IResourceWatchEvent"/>s
    /// </summary>
    protected Subject<IResourceWatchEvent> Subject { get; } = new();

    /// <summary>
    /// Iterates through the values produced by the <see cref="Cursor"/>
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task IterateCursorAsync()
    {
        bool dataAvailable = true;
        do
        {
            try
            {
                dataAvailable = await this.Cursor.MoveNextAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
                var events = this.Cursor.Current;
                if (events == null || !events.Any())
                {
                    await Task.Delay(10);
                    continue;
                }
                events.Where(e => e.OperationType.IsResourceWatchEventType() && e.FullDocument != null)
                    .Select(e =>
                    {
                        var watchEventType = e.FullDocument.TryGetElement("_deleted", out _) ? ResourceWatchEventType.Deleted : e.OperationType.ToResourceWatchEventType();
                        var resource = MongoDatabase.ReadResourceFromBsonDocument<Resource>(e.FullDocument);
                        return new ResourceWatchEvent(watchEventType, resource);
                    })
                    .ToList()
                    .ForEach(e => this.Subject.OnNext(e));
            }
            catch (Exception ex) when (ex is ObjectDisposedException or TaskCanceledException) { break; }
        }
        while (dataAvailable && !this._Disposed && !this.CancellationTokenSource.IsCancellationRequested);
    }

    /// <summary>
    /// Disposes of the <see cref="MongoResourceWatch"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoResourceWatch"/> is being disposed of</param>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (!this._Disposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource.Dispose();
                this.Cursor.Dispose();
                this.Subject.Dispose();
            }
            this._Disposed = true;
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
    /// Disposes of the <see cref="MongoResourceWatch"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoResourceWatch"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._Disposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource.Dispose();
                this.Cursor.Dispose();
                this.Subject.Dispose();
            }
            this._Disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}

/// <summary>
/// Represents an object used watch resource-related events
/// </summary>
public class MongoResourceWatch<TResource>
    : IResourceWatch<TResource>, IDisposable
    where TResource : class, IResource, new()
{

    bool _Disposed;

    /// <summary>
    /// Initializes a new <see cref="MongoResourceWatch"/>
    /// </summary>
    /// <param name="cursor">The underlying <see cref="IChangeStreamCursor{TDocument}"/></param>
    /// <param name="cancellationTokenSource">The <see cref="System.Threading.CancellationTokenSource"/> used to cancel the <see cref="MongoResourceWatch"/></param>
    public MongoResourceWatch(IChangeStreamCursor<ChangeStreamDocument<BsonDocument>> cursor, CancellationTokenSource cancellationTokenSource)
    {
        this.Cursor = cursor;
        this.CancellationTokenSource = cancellationTokenSource;
        _ = this.IterateCursorAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<IResourceWatchEvent<TResource>> observer) => this.Subject.Subscribe(observer);

    /// <summary>
    /// Gets the underlying <see cref="IChangeStreamCursor{TDocument}"/>
    /// </summary>
    protected IChangeStreamCursor<ChangeStreamDocument<BsonDocument>> Cursor { get; }

    /// <summary>
    /// Gets the <see cref="System.Threading.CancellationTokenSource"/> used to cancel the <see cref="MongoResourceWatch"/>
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; }

    /// <summary>
    /// Gets the <see cref="Subject{T}"/> used to publish and subscribe to <see cref="IResourceWatchEvent"/>s
    /// </summary>
    protected Subject<IResourceWatchEvent<TResource>> Subject { get; } = new();

    /// <summary>
    /// Iterates through the values produced by the <see cref="Cursor"/>
    /// </summary>
    /// <returns>A new awaitable <see cref="Task"/></returns>
    protected virtual async Task IterateCursorAsync()
    {
        bool dataAvailable = true;
        do
        {
            try
            {
                dataAvailable = await this.Cursor.MoveNextAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
                var events = this.Cursor.Current;
                if (events == null || !events.Any())
                {
                    await Task.Delay(10);
                    continue;
                }
                events.Where(e => e.OperationType.IsResourceWatchEventType() && e.FullDocument != null)
                    .Select(e =>
                    {
                        var watchEventType = e.FullDocument.TryGetElement("_deleted", out _) ? ResourceWatchEventType.Deleted : e.OperationType.ToResourceWatchEventType();
                        var resource = MongoDatabase.ReadResourceFromBsonDocument<TResource>(e.FullDocument);
                        return new ResourceWatchEvent<TResource>(watchEventType, resource);
                    })
                    .ToList()
                    .ForEach(this.Subject.OnNext);
            }
            catch (Exception ex) when (ex is ObjectDisposedException or TaskCanceledException) { break; }
        }
        while (dataAvailable && !this._Disposed && !this.CancellationTokenSource.IsCancellationRequested);
    }

    /// <summary>
    /// Disposes of the <see cref="MongoResourceWatch"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoResourceWatch"/> is being disposed of</param>
    protected virtual ValueTask DisposeAsync(bool disposing)
    {
        if (!this._Disposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource.Dispose();
                this.Cursor.Dispose();
                this.Subject.Dispose();
            }
            this._Disposed = true;
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
    /// Disposes of the <see cref="MongoResourceWatch"/>
    /// </summary>
    /// <param name="disposing">A boolean indicating whether or not the <see cref="MongoResourceWatch"/> is being disposed of</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._Disposed)
        {
            if (disposing)
            {
                this.CancellationTokenSource.Dispose();
                this.Cursor.Dispose();
                this.Subject.Dispose();
            }
            this._Disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
