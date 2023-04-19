namespace Hylo;

/// <summary>
/// Defines methods to handle API versions
/// </summary>
public static class ApiVersion
{

    /// <summary>
    /// Builds a new API version
    /// </summary>
    /// <param name="group">The API group</param>
    /// <param name="version">The API version</param>
    /// <returns>A new API version</returns>
    public static string Build(string group, string version) => $"{group}/{version}";

}