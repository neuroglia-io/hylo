using Json.Patch;

namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="Patch"/>es
/// </summary>
public static class PatchExtensions
{

    /// <summary>
    /// Applies the <see cref="Patch"/> to the specified target
    /// </summary>
    /// <param name="patch">The <see cref="Patch"/> to apply</param>
    /// <param name="target">The target to apply the patch to</param>
    /// <returns>The patched target</returns>
    public static T? ApplyTo<T>(this Patch patch, T? target)
    {
        if (patch == null) return target;
        if (target == null) return default;
        var node = patch.Type switch
        {
            PatchType.JsonMergePatch => Serializer.Json.SerializeToNode(JsonCons.Utilities.JsonMergePatch.ApplyMergePatch(Serializer.Json.SerializeToElement(target)!.Value, Serializer.Json.SerializeToElement(patch.Document)!.Value)),
            PatchType.JsonPatch => Serializer.Json.Deserialize<JsonPatch>(Serializer.Json.Serialize(patch.Document))!.Apply(Serializer.Json.SerializeToNode(target)).Result,
            PatchType.StrategicMergePatch => JsonStrategicMergePatch.ApplyPatch(Serializer.Json.SerializeToNode(target), Serializer.Json.SerializeToNode(patch.Document)),
            _ => throw new NotSupportedException($"The specified {nameof(PatchType)} '{patch.Type}' is not supported")
        };
        return Serializer.Json.Deserialize<T>(node!);
    }

}
