namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="ResourceDefinitionInfo"/>s
/// </summary>
public static class ResourceDefinitionInfoExtensions
{

    /// <summary>
    /// Gets the <see cref="ResourceDefinitionInfo"/>'s API version
    /// </summary>
    /// <param name="definition">The <see cref="ResourceDefinitionInfo"/> to get the API version of</param>
    /// <returns>The <see cref="ResourceDefinitionInfo"/>'s API version</returns>
    public static string GetApiVersion(this ResourceDefinitionInfo definition) => string.IsNullOrWhiteSpace(definition.Group) ? definition.Version : $"{definition.Group}/{definition.Version}";

}
