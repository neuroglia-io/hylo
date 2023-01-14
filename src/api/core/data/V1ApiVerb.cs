namespace Hylo.Api.Core;

/// <summary>
/// Enumerates all default API verbs
/// </summary>
public static class V1ApiVerb
{

    /// <summary>
    /// Indicates a create operation
    /// </summary>
    public const string Create = "create";
    /// <summary>
    /// Indicates a read operation
    /// </summary>
    public const string Read = "read";
    /// <summary>
    /// Indicates an update operation
    /// </summary>
    public const string Update = "update";
    /// <summary>
    /// Indicates a patch operation
    /// </summary>
    public const string Patch = "patch";
    /// <summary>
    /// Indicates a delete operation
    /// </summary>
    public const string Delete = "delete";
    /// <summary>
    /// Indicates a watch operation
    /// </summary>
    public const string Watch = "watch";

    /// <summary>
    /// Enunmerates all default <see cref="V1ApiVerb"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all default <see cref="V1ApiVerb"/>s</returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return Create;
        yield return Read;
        yield return Update;
        yield return Patch;
        yield return Delete;
        yield return Watch;
    }

}
