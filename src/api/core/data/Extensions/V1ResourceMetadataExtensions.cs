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

}
