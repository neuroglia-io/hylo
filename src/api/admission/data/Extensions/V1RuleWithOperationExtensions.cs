using System.Text.RegularExpressions;

namespace Hylo.Api.Admission;

/// <summary>
/// Defines extensions for <see cref="V1RuleWithOperation"/>s
/// </summary>
public static class V1RuleWithOperationExtensions
{

    /// <summary>
    /// Determines whether or not the <see cref="V1RuleWithOperation"/> matches the specified resource
    /// </summary>
    /// <param name="rule">The <see cref="V1RuleWithOperation"/> to evaluate</param>
    /// <param name="operation">The operation to perform</param>
    /// <param name="group">The API group the resource to operate on belongs to</param>
    /// <param name="version">The version of the API the resource to operate on belongs to</param>
    /// <param name="plural">The plural name of the type of the resource to operate on</param>
    /// <param name="namespace">The namespace the resource to operate on belongs to</param>
    /// <returns>A boolean indicating whether or not the <see cref="V1RuleWithOperation"/> matches the specified resource</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool Matches(this V1RuleWithOperation rule, string group, string version, string plural, string operation, string? @namespace = null)
    {
        if(string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if(string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if(string.IsNullOrWhiteSpace(plural)) throw new ArgumentNullException(nameof(plural));
        if (string.IsNullOrWhiteSpace(operation)) throw new ArgumentNullException(nameof(operation));
        if (!string.IsNullOrWhiteSpace(rule.Scope) && !string.IsNullOrWhiteSpace(@namespace) && !Regex.IsMatch(@namespace, rule.Scope, RegexOptions.Compiled)) return false;
        if (rule.ApiGroups != null && !rule.ApiGroups.Any(g => Regex.IsMatch(group, g, RegexOptions.Compiled))) return false;
        if (rule.ApiVersions != null && !rule.ApiVersions.Any(v => Regex.IsMatch(version, v, RegexOptions.Compiled))) return false;
        if (rule.Kinds != null && !rule.Kinds.Any(k => Regex.IsMatch(plural, k, RegexOptions.Compiled))) return false;
        return true;
    }

}
