namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Enumerates all default types of <see cref="V1ResourceEvent"/>s
/// </summary>
public static class V1ResourceEventType
{

    /// <summary>
    /// Indicates an event that describes the creation of a <see cref="V1Resource"/>
    /// </summary>
    public const string Create = "create";
    /// <summary>
    /// Indicates an event that describes the update of a <see cref="V1Resource"/>
    /// </summary>
    public const string Update = "update";
    /// <summary>
    /// Indicates an event that describes the deletion of a <see cref="V1Resource"/>
    /// </summary>
    public const string Delete = "delete";

    /// <summary>
    /// Enunmerates all default <see cref="V1ResourceEventType"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all default <see cref="V1ResourceEventType"/>s</returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return Create;
        yield return Update;
        yield return Delete;
    }

}