using Json.Patch;

namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="JsonPatch"/>es
/// </summary>
public static class JsonPatchExtensions
{

    /// <summary>
    /// Applies the <see cref="JsonPatch"/> to the specified target
    /// </summary>
    /// <param name="patch">The <see cref="JsonPatch"/> to apply</param>
    /// <param name="target">The target to apply the patch to</param>
    /// <returns>The patched target</returns>
    public static T? ApplyTo<T>(this JsonPatch patch, T? target)
    {
        if (patch == null) return target;
        if (target == null) return default;
        var result = patch.Apply(Serializer.Json.SerializeToNode(target)).Result;
        if(result == null) return default;
        return Serializer.Json.Deserialize<T>(result);
    }

}