using Hylo.Api.Core.Data.Models;

namespace Hylo.Api;

/// <summary>
/// Defines extensions for <see cref="V1ResourceDefinition"/>
/// </summary>
public static class V1ResourceDefinitionExtensions
{

    /// <summary>
    /// Attempts to get the specified <see cref="V1ResourceDefinitionVersion"/>
    /// </summary>
    /// <param name="resourceDefinition">The extended <see cref="V1ResourceDefinition"/></param>
    /// <param name="version">The version to get</param>
    /// <param name="resourceDefinitionVersion">The specified <see cref="V1ResourceDefinitionVersion"/>, if any</param>
    /// <returns>A boolean indicating whether or not the specified <see cref="V1ResourceDefinitionVersion"/> exists</returns>
    public static bool TryGetVersion(this V1ResourceDefinition resourceDefinition, string version, out V1ResourceDefinitionVersion? resourceDefinitionVersion)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        resourceDefinitionVersion = resourceDefinition.Spec.Versions.SingleOrDefault(v => v.Name.Equals(version, StringComparison.Ordinal));
        return resourceDefinitionVersion != null;
    }

    /// <summary>
    /// Gets the <see cref="V1ResourceDefinition"/>'s storage <see cref="V1ResourceDefinitionVersion"/>
    /// </summary>
    /// <param name="resourceDefinition">The extended <see cref="V1ResourceDefinition"/></param>
    /// <returns>The storage <see cref="V1ResourceDefinitionVersion"/></returns>
    public static V1ResourceDefinitionVersion? GetStorageVersion(this V1ResourceDefinition resourceDefinition)
    {
        return resourceDefinition.Spec.Versions.SingleOrDefault(v => v.Storage);
    }

}
