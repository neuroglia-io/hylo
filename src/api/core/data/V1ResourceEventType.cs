using Hylo.Api.Core.Data.Models;

namespace Hylo.Api.Core;

/// <summary>
/// Enumerates all default types of <see cref="V1ResourceEvent"/>s
/// </summary>
public static class V1ResourceEventType
{

    /// <summary>
    /// Indicates an event that describes the creation of a <see cref="V1Resource"/>
    /// </summary>
    public const string Created = "created";
    /// <summary>
    /// Indicates an event that describes the update of a <see cref="V1Resource"/>
    /// </summary>
    public const string Updated = "updated";
    /// <summary>
    /// Indicates an event that describes the deletion of a <see cref="V1Resource"/>
    /// </summary>
    public const string Deleted = "deleted";

    /// <summary>
    /// Enunmerates all default <see cref="V1ResourceEventType"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all default <see cref="V1ResourceEventType"/>s</returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return Created;
        yield return Updated;
        yield return Deleted;
    }

}