namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="IResourceWatchEvent"/>s
/// </summary>
public static class IResourceWatchEventExtensions
{

    /// <summary>
    /// Converts the <see cref="ResourceWatchEvent"/> into a new <see cref="ResourceWatchEvent{TResource}"/>
    /// </summary>
    /// <typeparam name="TResource">The type of watched <see cref="Resource"/>s</typeparam>
    /// <param name="e">The <see cref="ResourceWatchEvent"/> to convert</param>
    /// <returns>A new <see cref="ResourceWatchEvent{TResource}"/></returns>
    public static IResourceWatchEvent<TResource> OfType<TResource>(this IResourceWatchEvent e)
        where TResource : class, IResource, new()
    {
        if (e is IResourceWatchEvent<TResource> generic) return generic;
        return new ResourceWatchEvent<TResource>(e.Type, e.Resource.ConvertTo<TResource>()!);
    }

}
