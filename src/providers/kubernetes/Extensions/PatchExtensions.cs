using k8s.Models;

namespace Hylo.Providers.Kubernetes;

/// <summary>
/// Defines extensions for <see cref="Patch"/>es
/// </summary>
public static class PatchExtensions
{

    /// <summary>
    /// Converts the <see cref="Patch"/> to a new <see cref="V1Patch"/>
    /// </summary>
    /// <param name="patch">The <see cref="Patch"/> to convert</param>
    /// <returns>A new <see cref="V1Patch"/></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static V1Patch ToV1Patch(this Patch patch)
    {
        var patchType = patch.Type switch
        {
            PatchType.JsonMergePatch => V1Patch.PatchType.MergePatch,
            PatchType.JsonPatch => V1Patch.PatchType.JsonPatch,
            PatchType.StrategicMergePatch => V1Patch.PatchType.StrategicMergePatch,
            _ => throw new NotSupportedException($"The specified patch type '{patch.Type}' is not supported")
        };
        return new V1Patch(patch.Document, patchType);
    }

}