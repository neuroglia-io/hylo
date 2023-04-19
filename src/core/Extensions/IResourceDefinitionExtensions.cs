namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="IResourceDefinition"/>s
/// </summary>
public static class IResourceDefinitionExtensions
{

    /// <summary>
    /// Attempts to get the specified <see cref="ResourceDefinitionVersion"/>
    /// </summary>
    /// <param name="resourceDefinition">The extended <see cref="IResourceDefinition"/></param>
    /// <param name="version">The version to get</param>
    /// <param name="resourceDefinitionVersion">The specified <see cref="ResourceDefinitionVersion"/>, if any</param>
    /// <returns>A boolean indicating whether or not the specified <see cref="ResourceDefinitionVersion"/> exists</returns>
    public static bool TryGetVersion(this IResourceDefinition resourceDefinition, string version, out ResourceDefinitionVersion? resourceDefinitionVersion)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        resourceDefinitionVersion = resourceDefinition.Spec.Versions.SingleOrDefault(v => v.Name.Equals(version, StringComparison.Ordinal));
        return resourceDefinitionVersion != null;
    }

    /// <summary>
    /// Gets the <see cref="IResourceDefinition"/>'s storage version 
    /// </summary>
    /// <param name="resourceDefinition">The <see cref="IResourceDefinition"/> to get the storage version of</param>
    /// <returns>The <see cref="ResourceDefinitionVersion"/> used for storage</returns>
    public static ResourceDefinitionVersion GetStorageVersion(this IResourceDefinition resourceDefinition) => 
        resourceDefinition.Spec.Versions.SingleOrDefault(v => v.Storage) 
        ?? throw new HyloException(ProblemDetails.ResourceStorageVersionNotFound(new ResourceDefinitionReference(resourceDefinition.Spec.Group, resourceDefinition.GetVersion(), resourceDefinition.Spec.Names.Plural)));

}