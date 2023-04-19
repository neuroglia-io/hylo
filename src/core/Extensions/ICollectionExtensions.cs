namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="ICollection"/>s
/// </summary>
public static class ICollectionExtensions
{

    /// <summary>
    /// Converts the <see cref="ICollection"/> into a new <see cref="ICollection{TObject}"/> of the specified type
    /// </summary>
    /// <typeparam name="TObject">The type to convert the collection items to</typeparam>
    /// <param name="collection">The <see cref="ICollection"/> to convert</param>
    /// <returns>A new <see cref="ICollection{TObject}"/></returns>
    public static ICollection<TObject> OfType<TObject>(this ICollection collection)
        where TObject : class, IObject, new()
    {
        if(collection is ICollection<TObject> generic) return generic;
        return new Collection<TObject>(collection.Metadata, collection.Items?.Select(i => i.ConvertTo<TObject>()!));
    }

}