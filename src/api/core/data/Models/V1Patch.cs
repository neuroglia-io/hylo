using Hylo.Serialization.Json.Utilities;
using Json.More;
using JsonCons.Utilities;
using System.Text.Json.Nodes;

namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Describes a patch
/// </summary>
[DataContract]
public class V1Patch
{

    /// <summary>
    /// Initializes a new <see cref="V1Patch"/>
    /// </summary>
    public V1Patch() { }

    /// <summary>
    /// Initializes a new <see cref="V1Patch"/>
    /// </summary>
    /// <param name="type">The patch's type<para></para>See <see cref="V1PatchType"/></param>
    /// <param name="document">The patch document</param>
    public V1Patch(string type, object document)
    {
        this.Type = type;
        this.Document = document;
    }

    /// <summary>
    /// Gets the patch's type
    /// </summary>
    [DataMember(Name = "type", Order = 1), JsonPropertyName("type")]
    public virtual string Type { get; set; } = null!;

    /// <summary>
    /// Gets the patch document
    /// </summary>
    [DataMember(Name = "document", Order = 2), JsonPropertyName("document")]
    public virtual object Document { get; set; } = null!;

    /// <summary>
    /// Applies the <see cref="V1Patch"/> to the specified object
    /// </summary>
    /// <param name="target">The object to apply the <see cref="V1Patch"/> to</param>
    /// <returns>A new <see cref="JsonObject"/> that represents the patched target</returns>
    public virtual JsonObject ApplyTo(JsonObject target)
    {
        var patch = Serializer.Json.SerializeToNode(this.Document)?.AsObject();
        if (patch == null) return target;
        return this.Type switch
        {
            V1PatchType.JsonPatch => JsonPatch.ApplyPatch(target.AsJsonElement(), patch["operations"]!.AsJsonElement()).RootElement.AsNode()!.AsObject()!,
            V1PatchType.JsonMergePatch => JsonMergePatch.ApplyMergePatch(target.AsJsonElement(), patch.AsJsonElement()).RootElement.AsNode()!.AsObject()!,
            V1PatchType.StrategicMergePatch => JsonStrategicMergePatch.ApplyPatch(target, patch)!,
            _ => throw new NotSupportedException($"The specified {nameof(V1Patch)} type '{this.Type}' is not supported")
        };
    }

}
