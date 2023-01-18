namespace Hylo.Api.Core.Data;

/// <summary>
/// Defines extensions for <see cref="object"/>s
/// </summary>
public static class ObjectExtensions
{

    /// <summary>
    /// Converts the object into a <see cref="JsonNode"/>
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <returns>The converted object</returns>
    public static JsonNode? AsJsonNode(this object obj)
    {
        return Serializer.Json.SerializeToNode(obj);
    }

    /// <summary>
    /// Converts the object into a <see cref="JsonObject"/>
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <returns>The converted object</returns>
    public static JsonObject? AsJsonObject(this object obj)
    {
        return obj.AsJsonNode()?.AsObject();
    }

}
