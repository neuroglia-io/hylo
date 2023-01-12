namespace Hylo;

/// <summary>
/// Defines the fundamentals of an object that exposes metadata
/// </summary>
/// <typeparam name="TMetadata">The type of the object's metadata</typeparam>
public interface IMetadata<TMetadata>
    where TMetadata : class, new()
{

    /// <summary>
    /// Gets the object's metadata
    /// </summary>
    TMetadata Metadata { get; }

}