using Hylo.Serialization.Json;

namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="JsonObject"/>s
/// </summary>
public static class JsonObjectExtensions
{

    /// <summary>
    /// Attempts to get the specified property
    /// </summary>
    /// <param name="jsonObject">The extended <see cref="JsonObject"/></param>
    /// <param name="reference">The reference of the property to get</param>
    /// <param name="propertyReferenceType">The type of the specified property reference</param>
    /// <param name="jsonNode">The resulting <see cref="JsonNode"/>, if any</param>
    /// <returns>A boolean indicating whether or not the specified property could be found</returns>
    public static bool TryGetPropertyValue(this JsonObject jsonObject, string reference, JsonObjectPropertyReferenceType propertyReferenceType, out JsonNode? jsonNode)
    {
        jsonNode = null;
        switch (propertyReferenceType)
        {
            case JsonObjectPropertyReferenceType.Name:
                return jsonObject.TryGetPropertyValue(reference, out jsonNode);
            case JsonObjectPropertyReferenceType.Path:
                var segments = reference.Split('.', StringSplitOptions.RemoveEmptyEntries);
                jsonNode = jsonObject;
                for (int i = 0; i < segments.Length; i++)
                {
                    if (jsonNode is not JsonObject jsonObjectValue) return false;
                    if (!jsonObjectValue.TryGetPropertyValue(segments[i], out jsonNode) || jsonNode == null) return false;
                }
                return jsonNode != null;
            default: throw new NotSupportedException();
        }
    }

}
