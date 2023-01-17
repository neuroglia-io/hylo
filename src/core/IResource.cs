namespace Hylo;

/// <summary>
/// Defines the fundamentals of an Hylo resource
/// </summary>
public interface IResource
{

    /// <summary>
    /// Gets the resource's API version
    /// </summary>
    string ApiVersion { get; }

    /// <summary>
    /// Gets the resource's kind
    /// </summary>
    string Kind { get; }

}

/// <summary>
/// Defines the fundamentals of an Hylo resource
/// </summary>
/// <typeparam name="TMetadata">The type of the resource's metadata</typeparam>
public interface IResource<TMetadata>
    : IResource, IMetadata<TMetadata>
    where TMetadata : class, new()
{


}

/// <summary>
/// Defines the fundamentals of an Hylo resource
/// </summary>
/// <typeparam name="TMetadata">The type of the object's metadata</typeparam>
/// <typeparam name="TSpec">The type of the object's spec</typeparam>
public interface IResource<TMetadata, TSpec>
    : IResource<TMetadata>, ISpec<TSpec>
    where TMetadata : class, new()
    where TSpec : class, new()
{



}

/// <summary>
/// Defines the fundamentals of an Hylo resource
/// </summary>
/// <typeparam name="TMetadata">The type of the object's metadata</typeparam>
/// <typeparam name="TSpec">The type of the object's spec</typeparam>
/// <typeparam name="TStatus">The type of the object's status</typeparam>
public interface IResource<TMetadata, TSpec, TStatus>
    : IResource<TMetadata, TSpec>, IStatus<TStatus>
    where TMetadata : class, new()
    where TSpec : class, new()
    where TStatus : class, new()
{



}
