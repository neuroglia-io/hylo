namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Enumerates all default <see cref="V1Patch"/> types
/// </summary>
public static class V1PatchType
{
    /// <summary>
    /// Indicates a <see href="https://www.rfc-editor.org/rfc/rfc6902">Json Patch</see>
    /// </summary>
    public const string JsonPatch = "patch";
    /// <summary>
    /// Indicates a <see href="https://www.rfc-editor.org/rfc/rfc7386">Json Merge Patch</see>
    /// </summary>
    public const string JsonMergePatch = "merge";
    /// <summary>
    /// Indicates a <see href="https://github.com/kubernetes/community/blob/master/contributors/devel/sig-api-machinery/strategic-merge-patch.md">Json Strategic Merge Patch</see>
    /// </summary>
    public const string StrategicMergePatch = "strategic";

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing all default <see cref="V1Patch"/> types
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return JsonPatch;
        yield return JsonMergePatch;
        yield return StrategicMergePatch;
    }

}
