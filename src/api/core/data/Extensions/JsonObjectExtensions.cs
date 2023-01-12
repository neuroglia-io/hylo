using Hylo.Api.Core.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace Hylo.Api.Core.Data;

/// <summary>
/// Defines extensions for <see cref="JsonObject"/>s
/// </summary>
public static class JsonObjectExtensions
{

    /// <summary>
    /// Attempts to get the value of the property with the specified name
    /// </summary>
    /// <typeparam name="TProperty">The expected type of the property to get</typeparam>
    /// <param name="jsonObject">The <see cref="JsonObject"/> to get the specified property of</param>
    /// <param name="propertyName">The name of the property to get the value of</param>
    /// <param name="propertyValue">The value of the property</param>
    /// <returns>A boolean indicating whether or not the property with the specified name exists</returns>
    public static bool TryGetPropertyValue<TProperty>(this JsonObject jsonObject, string propertyName, [MaybeNullWhen(false)] out TProperty? propertyValue)
    {
        propertyValue = default;
        if (!jsonObject.TryGetPropertyValue(propertyName, out JsonNode? node) || node == null) return false;
        propertyValue = Serializer.Json.Deserialize<TProperty>(node);
        return true;
    }

    /// <summary>
    /// Gets the resource's API version
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> that represents the resource to get the API version of</param>
    /// <returns>The resource's API version</returns>
    public static string? GetResourceApiVersion(this JsonObject jsonObject)
    {
        jsonObject.TryGetPropertyValue(nameof(V1Resource.ApiVersion).ToCamelCase(), out string? apiVersion);
        return apiVersion;
    }

    /// <summary>
    /// Gets the resource's kind
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> that represents the resource to get the kind of</param>
    /// <returns>The resource's kind</returns>
    public static string? GetResourceKind(this JsonObject jsonObject)
    {
        jsonObject.TryGetPropertyValue(nameof(V1Resource.Kind).ToCamelCase(), out string? kind);
        return kind;
    }

    /// <summary>
    /// Gets the resource's name
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> that represents the resource to get the name of</param>
    /// <returns>The resource's name/returns>
    public static string? GetResourceName(this JsonObject jsonObject)
    {
        if (!jsonObject.TryGetResourceMetadataNode(out var metadata) || metadata == null) return null;
        metadata.AsObject()!.TryGetPropertyValue(nameof(V1ResourceMetadata.Name).ToCamelCase(), out string? name);
        return name;
    }

    /// <summary>
    /// Gets the resource's namespace
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> that represents the resource to get the namespace of</param>
    /// <returns>The resource's namespace/returns>
    public static string? GetResourceNamespace(this JsonObject jsonObject)
    {
        if (!jsonObject.TryGetResourceMetadataNode(out var metadata) || metadata == null) return null;
        metadata.AsObject()!.TryGetPropertyValue(nameof(V1ResourceMetadata.Namespace).ToCamelCase(), out string? @namespace);
        return @namespace;
    }

    /// <summary>
    /// Sets the resource's namespace to <see cref="V1Namespace.Default"/>
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> that represents the resource to set the default namespace of</param>
    public static void SetDefaultNamespace(this JsonObject jsonObject)
    {
        if (!jsonObject.TryGetResourceMetadataNode(out var metadata) || metadata == null) return;
        metadata[nameof(V1ResourceMetadata.Namespace).ToCamelCase()] = V1Namespace.Default;
    }

    /// <summary>
    /// Extracts <see cref="V1ResourceMetadata"/> <see cref="JsonNode"/> from the <see cref="JsonObject"/>
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> to extract <see cref="V1ResourceMetadata"/> <see cref="JsonNode"/> from</param>
    /// <returns>The extracted <see cref="V1ResourceMetadata"/> <see cref="JsonNode"/>, if any</returns>
    public static JsonNode? GetResourceMetadataNode(this JsonObject jsonObject)
    {
        if (!jsonObject.TryGetPropertyValue(nameof(V1Resource.Metadata).ToCamelCase(), out var node)) return null;
        return node;
    }

    /// <summary>
    /// Attempts to extract <see cref="V1ResourceMetadata"/> <see cref="JsonNode"/> from the <see cref="JsonObject"/>
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> to extract <see cref="V1ResourceMetadata"/> <see cref="JsonNode"/> from</param>
    /// <param name="node">The extracted <see cref="V1ResourceMetadata"/> <see cref="JsonNode"/>, if any</param>
    /// <returns>A boolean indicating whether or not <see cref="V1ResourceMetadata"/> <see cref="JsonNode"/> could be extracted from the <see cref="JsonObject"/></returns>
    public static bool TryGetResourceMetadataNode(this JsonObject jsonObject, [MaybeNullWhen(false)] out JsonNode? node)
    {
        return jsonObject.TryGetPropertyValue(nameof(V1Resource.Metadata).ToCamelCase(), out node) && node != null;
    }

    /// <summary>
    /// Extracts <see cref="V1ResourceMetadata"/> from the <see cref="JsonObject"/>
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> to extract <see cref="V1ResourceMetadata"/> from</param>
    /// <returns>The extracted <see cref="V1ResourceMetadata"/>, if any</returns>
    public static V1ResourceMetadata? GetResourceMetadata(this JsonObject jsonObject)
    {
        if (!jsonObject.TryGetResourceMetadataNode(out var node) || node == null) return null;
        return Serializer.Json.Deserialize<V1ResourceMetadata>(node);
    }

    /// <summary>
    /// Attempts to extract <see cref="V1ResourceMetadata"/> from the <see cref="JsonObject"/>
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> to extract <see cref="V1ResourceMetadata"/> from</param>
    /// <param name="metadata">The extracted <see cref="V1ResourceMetadata"/>, if any</param>
    /// <returns>A boolean indicating whether or not <see cref="V1ResourceMetadata"/> could be extracted from the <see cref="JsonObject"/></returns>
    public static bool TryGetResourceMetadata(this JsonObject jsonObject, [MaybeNullWhen(false)] out V1ResourceMetadata? metadata)
    {
        metadata = jsonObject.GetResourceMetadata();
        return metadata != null;
    }

    /// <summary>
    /// Initializes the resource's <see cref="V1ResourceMetadata"/>
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> that represents the resource to initialize the <see cref="V1ResourceMetadata"/> of</param>
    public static void InitializeMetadata(this JsonObject jsonObject)
    {
        if (!jsonObject.TryGetResourceMetadataNode(out var node) || node == null)
        {
            node = new JsonObject();
            jsonObject.Add(nameof(V1Resource.Metadata).ToCamelCase(), node);
        }
        var metadata = node.AsObject();
        if (!metadata.ContainsKey(nameof(V1ResourceMetadata.Id).ToCamelCase())) metadata.Add(nameof(V1ResourceMetadata.Id).ToCamelCase(), Guid.NewGuid().ToString());
        if ((!metadata.TryGetPropertyValue(nameof(V1ResourceMetadata.Name).ToCamelCase(), out var nameNode) || nameNode == null || string.IsNullOrWhiteSpace(nameNode.GetValue<string>()))
            && metadata.TryGetPropertyValue(nameof(V1ResourceMetadata.NamePrefix).ToCamelCase(), out string? namePrefix) && !string.IsNullOrWhiteSpace(namePrefix))
            metadata[nameof(V1ResourceMetadata.Name).ToCamelCase()] = $"{namePrefix}{Guid.NewGuid().ToShortString()}";
        metadata[nameof(V1ResourceMetadata.CreatedAt).ToCamelCase()] = DateTimeOffset.Now.ToString("o");
        metadata[nameof(V1ResourceMetadata.LastModified).ToCamelCase()] = DateTimeOffset.Now.ToString("o");
        metadata[nameof(V1ResourceMetadata.StateVersion).ToCamelCase()] = 0;
    }

    /// <summary>
    /// Marks the resource as updated
    /// </summary>
    /// <param name="jsonObject">The <see cref="JsonObject"/> that represents the resource to mark as updated</param>
    public static void MarkAsUpdated(this JsonObject jsonObject)
    {
        if (!jsonObject.TryGetResourceMetadataNode(out var node) || node == null) throw new BadImageFormatException();
        var metadata = node.AsObject();
        metadata[nameof(V1ResourceMetadata.LastModified).ToCamelCase()] = DateTimeOffset.Now.ToString("o");
    }

    /// <summary>
    /// Determines whether or not the <see cref="JsonObject"/> is a <see cref="V1ResourceDefinition"/>
    /// </summary>
    /// <param name="jsonObject">The extended <see cref="JsonObject"/></param>
    /// <returns>A boolean indicating whether or not the <see cref="JsonObject"/> is a <see cref="V1ResourceDefinition"/></returns>
    public static bool IsResourceDefinition(this JsonObject jsonObject)
    {
        if (!jsonObject.TryGetPropertyValue(nameof(V1Resource.ApiVersion).ToCamelCase(), out string? apiVersion)) return false;
        if (!jsonObject.TryGetPropertyValue(nameof(V1Resource.Kind).ToCamelCase(), out string? kind) || string.IsNullOrWhiteSpace(kind)) return false;
        if (string.IsNullOrEmpty(apiVersion)) return false;

        return apiVersion.Equals(V1ResourceDefinition.ResourceApiVersion, StringComparison.InvariantCultureIgnoreCase)
            && kind.Equals(V1ResourceDefinition.ResourceKind, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Gets the <see cref="JsonObject"/> resource id <see cref="JsonNode"/>
    /// </summary>
    /// <param name="jsonObject">The extended <see cref="JsonObject"/></param>
    /// <returns>The resource id <see cref="JsonNode"/>, if any</returns>
    public static JsonNode? GetResourceIdNode(this JsonObject jsonObject)
    {
        if (!jsonObject.TryGetResourceMetadataNode(out var metadataNode) || metadataNode == null) return null;
        return metadataNode[nameof(V1ResourceMetadata.Id).ToCamelCase()];
    }

    /// <summary>
    /// Attempts to get the <see cref="JsonObject"/> resource id <see cref="JsonNode"/>
    /// </summary>
    /// <param name="jsonObject">The extended <see cref="JsonObject"/></param>
    /// <param name="node">The resource id <see cref="JsonNode"/>, if any</param>
    /// <returns>A boolean indicating whether or not the <see cref="JsonObject"/> defines a resource id <see cref="JsonNode"/></returns>
    public static bool TryGetResourceIdNode(this JsonObject jsonObject, out JsonNode? node)
    {
        node = jsonObject.GetResourceIdNode();
        return node != null;
    }

}
