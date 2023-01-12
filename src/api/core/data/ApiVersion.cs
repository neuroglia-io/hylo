namespace Hylo.Api;

/// <summary>
/// Exposes functionality to handle Hylo API versions
/// </summary>
public static class ApiVersion
{

    /// <summary>
    /// Gets the group component of the specified API version
    /// </summary>
    /// <param name="apiVersion">The API version to get the group of</param>
    /// <returns>The group component of the specified API version</returns>
    public static string GetGroup(string apiVersion)
    {
        if (string.IsNullOrWhiteSpace(apiVersion)) throw new ArgumentNullException(nameof(apiVersion));
        var components = apiVersion.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return components.Length switch
        {
            1 => string.Empty,
            2 => components[0],
            _ => throw new ArgumentException(nameof(apiVersion)) //todo: exception
        };
    }

    /// <summary>
    /// Gets the version component of the specified API version
    /// </summary>
    /// <param name="apiVersion">The API version to get the version of</param>
    /// <returns>The version component of the specified API version</returns>
    public static string GetVersion(string apiVersion)
    {
        if (string.IsNullOrWhiteSpace(apiVersion)) throw new ArgumentNullException(nameof(apiVersion));
        var components = apiVersion.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return components.Length switch
        {
            1 => components[0],
            2 => components[1],
            _ => throw new ArgumentException(nameof(apiVersion)) //todo: exception
        };
    }

    /// <summary>
    /// Builds a new <see cref="ApiVersion"/>
    /// </summary>
    /// <param name="group">The group</param>
    /// <param name="version">The version</param>
    /// <returns>A new API version</returns>
    public static string Build(string group, string version)
    {
        return $"{group}/{version}";
    }

}
