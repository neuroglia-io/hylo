using Hylo.Api.Core.Data.Models;

namespace Hylo.Api;

/// <summary>
/// Defines extensions for <see cref="V1ResourceReference"/>s
/// </summary>
public static class V1ResourceReferenceExtensions
{

    /// <summary>
    /// Gets the resource's namespaced name
    /// </summary>
    /// <param name="reference">The extended <see cref="V1ResourceReference"/></param>
    /// <returns>The resource's namespaced name</returns>
    public static string GetNamespacedName(this V1ResourceReference reference) => string.IsNullOrWhiteSpace(reference.Namespace) ? reference.Name : $"{reference.Namespace}/{reference.Name}";

}