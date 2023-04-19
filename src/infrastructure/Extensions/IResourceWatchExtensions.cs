namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="IResourceWatch"/>es
/// </summary>
public static class IResourceWatchExtensions
{

    /// <summary>
    /// Converts the <see cref="IResourceWatch"/> into a new <see cref="IResourceWatch{TResource}"/> of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="IResource"/> to watch</typeparam>
    /// <param name="watch">The <see cref="IResourceWatch"/> to convert</param>
    /// <returns>A new <see cref="IResourceWatch{TResource}"/> of the specified type</returns>
    public static IResourceWatch<TResource> OfType<TResource>(this IResourceWatch watch)
        where TResource : class, IResource, new()
    {
        if (watch is IResourceWatch<TResource> generic) return generic;
        var dispose = watch is not ResourceWatch knownWatch || knownWatch.DisposeObservable;
        return new ResourceWatch<TResource>(watch, dispose);
    }

}
