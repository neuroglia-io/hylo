using Hylo.Api.Core.Data.Models;

namespace Hylo.Api;

/// <summary>
/// Defines extensions for <see cref="V1ResourceMetadata"/>s
/// </summary>
public static class V1ResourceMetadataExtensions
{

    /// <summary>
    /// Determines whether or not the <see cref="V1ResourceMetadata"/> describes a namespaced <see cref="V1Resource"/>
    /// </summary>
    /// <param name="metadata">The extended <see cref="V1ResourceMetadata"/></param>
    /// <returns>A boolean indicating whether or not the <see cref="V1Resource"/> described by <see cref="V1ResourceMetadata"/> is namespaced</returns>
    public static bool IsNamespaced(this V1ResourceMetadata metadata) => !string.IsNullOrWhiteSpace(metadata.Namespace);

    /// <summary>
    /// Gets the resource's namespaced name
    /// </summary>
    /// <param name="metadata">The extended <see cref="V1ResourceMetadata"/></param>
    /// <returns>The resource's namespaced name</returns>
    public static string GetNamespacedName(this V1ResourceMetadata metadata) => string.IsNullOrWhiteSpace(metadata.Namespace) ? metadata.Name : $"{metadata.Namespace}/{metadata.Name}";

}
