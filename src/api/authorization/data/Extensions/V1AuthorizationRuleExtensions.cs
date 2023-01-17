using Hylo.Api.Authorization.Data.Models;

namespace Hylo.Api;

/// <summary>
/// Defines extensions for <see cref="V1AuthorizationRule"/>s
/// </summary>
public static class V1AuthorizationRuleExtensions
{

    /// <summary>
    /// Determines whether or not the <see cref="V1AuthorizationRule"/> authorizes all verbs
    /// </summary>
    /// <param name="rule">The <see cref="V1AuthorizationRule"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="V1AuthorizationRule"/> authorizes all verbs</returns>
    public static bool AuthorizesAllVerbs(this V1AuthorizationRule rule) => rule.Verbs.SingleOrDefault() == "*";

    /// <summary>
    /// Determines whether or not the <see cref="V1AuthorizationRule"/> authorizes all API groups
    /// </summary>
    /// <param name="rule">The <see cref="V1AuthorizationRule"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="V1AuthorizationRule"/> authorizes all API groups</returns>
    public static bool AuthorizesAllApiGroups(this V1AuthorizationRule rule) => rule.ApiGroups.SingleOrDefault() == "*";

    /// <summary>
    /// Determines whether or not the <see cref="V1AuthorizationRule"/> authorizes all resource types
    /// </summary>
    /// <param name="rule">The <see cref="V1AuthorizationRule"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="V1AuthorizationRule"/> authorizes all resource types</returns>
    public static bool AuthorizesAllResourceTypes(this V1AuthorizationRule rule) => rule.Resources.SingleOrDefault() == "*";

    /// <summary>
    /// Determines whether or not the <see cref="V1AuthorizationRule"/> authorizes all resources
    /// </summary>
    /// <param name="rule">The <see cref="V1AuthorizationRule"/> to check</param>
    /// <returns>A boolean indicating whether or not the <see cref="V1AuthorizationRule"/> authorizes all resources</returns>
    public static bool AuthorizesAllResources(this V1AuthorizationRule rule) => rule.ResourceNames == null || rule.ResourceNames?.SingleOrDefault() == "*";

    /// <summary>
    /// Determines whether or not the <see cref="V1AuthorizationRule"/> authorizes the specified operation
    /// </summary>
    /// <param name="rule">The <see cref="V1AuthorizationRule"/> to evaluate</param>
    /// <param name="verb">The verb that describes the operation to perform</param>
    /// <param name="apiGroup">The API group the resource to operate on belongs to</param>
    /// <param name="pluralName">The plural name of the type of the resource to operate on</param>
    /// <param name="name">The name of the resource to operate on</param>
    /// <returns>A boolean indicating whether or not the <see cref="V1AuthorizationRule"/> authorizes the specified operation</returns>
    public static bool Authorizes(this V1AuthorizationRule rule, string verb, string apiGroup, string pluralName, string? name = null)
    {
        if (rule == null) throw new ArgumentNullException(nameof(rule));
        if (string.IsNullOrWhiteSpace(verb)) throw new ArgumentNullException(nameof(verb));
        if (string.IsNullOrWhiteSpace(apiGroup)) throw new ArgumentNullException(nameof(apiGroup));
        if (string.IsNullOrWhiteSpace(pluralName)) throw new ArgumentNullException(nameof(pluralName));
        if (!rule.AuthorizesAllVerbs() && rule.Verbs.Contains(verb) == false) return false;
        if (!rule.AuthorizesAllApiGroups() && rule.ApiGroups.Contains(apiGroup) == false) return false;
        if (!rule.AuthorizesAllResourceTypes() && rule.Resources.Contains(pluralName) == false) return false;
        if (!rule.AuthorizesAllResources() && !string.IsNullOrWhiteSpace(name) && rule.ResourceNames?.Contains(name) == false) return false;
        return true;
    }

}