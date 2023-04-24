using Json.Patch;

namespace Hylo;

/// <summary>
/// Exposes methods to help handling <see cref="JsonPatch"/>es
/// </summary>
public static class JsonPatchHelper
{

    /// <summary>
    /// Creates a new <see cref="JsonPatch"/> based on the differences between the specified values
    /// </summary>
    /// <param name="source">The source object</param>
    /// <param name="target">The target object</param>
    /// <returns>A new <see cref="JsonPatch"/> based on the differences between the specified values</returns>
    public static JsonPatch CreateJsonPatchFromDiff(object? source, object? target)
    {
        source ??= new();
        target ??= new();
        var sourceToken = Serializer.Json.SerializeToElement(source)!.Value;
        var targetToken = Serializer.Json.SerializeToElement(target)!.Value;
        var patchDocument = JsonCons.Utilities.JsonPatch.FromDiff(sourceToken, targetToken);
        return Serializer.Json.Deserialize<JsonPatch>(patchDocument.RootElement)!;
    }

}
