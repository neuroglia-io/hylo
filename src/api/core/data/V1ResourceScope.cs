using Hylo.Api.Core.Data.Models;

namespace Hylo.Api.Core;

/// <summary>
/// Enumerates all default <see cref="V1Resource"/> scopes
/// </summary>
public static class V1ResourceScope
{

    /// <summary>
    /// Indicates a resource that is available cluster-wide
    /// </summary>
    public const string Cluster = "cluster";
    /// <summary>
    /// Indicates a namespaced resource
    /// </summary>
    public const string Namespaced = "namespaced";

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing all default <see cref="V1Resource"/> scopes
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> AsEnumerable()
    {
        yield return Cluster;
        yield return Namespaced;
    }

}