namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="IResourceReference"/>s
/// </summary>
public static class IResourceReferenceExtensions
{

    /// <summary>
    /// Gets the API version of the referenced resource
    /// </summary>
    /// <param name="resourceReference">An object used to reference a resource</param>
    /// <returns>The API version of the referenced resource</returns>
    public static string GetApiVersion(this IResourceReference resourceReference) => $"{resourceReference.Definition.Group}/{resourceReference.Definition.Version}";

}