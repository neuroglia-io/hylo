namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents an authorization rule
/// </summary>
[DataContract]
public class V1AuthorizationRule
{

    /// <summary>
    /// Initializes a new <see cref="V1AuthorizationRule"/>
    /// </summary>
    public V1AuthorizationRule() { }

    /// <summary>
    /// Initializes a new <see cref="V1AuthorizationRule"/>
    /// </summary>
    /// <param name="apiGroups">A list containing all the API groups the role grants access to</param>
    /// <param name="resources">A list containing all the resources the role grants access to. You can use a single '*' string to authorize the role for all resources</param>
    /// <param name="verbs">A list containing all the verbs the role grants access to. You can use a single '*' string to authorize the role for all verbs</param>
    /// <param name="resourceNames">An optional list containing the names of all resources the role grants access to. Set the property to null or declare a single '*' string to authorize the role for all resources.</param>
    public V1AuthorizationRule(List<string> apiGroups, List<string> resources, List<string> verbs, List<string>? resourceNames = null)
    {
        this.ApiGroups = apiGroups;
        this.Resources = resources;
        this.Verbs = verbs;
        this.ResourceNames = resourceNames;
    }

    /// <summary>
    /// Gets/sets a list containing all the API groups the role grants access to
    /// </summary>
    [DataMember(Name = "apiGroups", Order = 1), JsonPropertyName("apiGroups"), Required, MinLength(1)]
    public virtual List<string> ApiGroups { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing all the resources the role grants access to. You can use a single '*' string to authorize the role for all resources
    /// </summary>
    [DataMember(Name = "resources", Order = 2), JsonPropertyName("resources"), Required, MinLength(1)]
    public virtual List<string> Resources { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing all the verbs the role grants access to. You can use a single '*' string to authorize the role for all verbs
    /// </summary>
    [DataMember(Name = "verbs", Order = 3), JsonPropertyName("verbs"), Required, MinLength(1)]
    public virtual List<string> Verbs { get; set; } = null!;

    /// <summary>
    /// Gets/sets an optional list containing the names of all resources the role grants access to. Set the property to null or declare a single '*' string to authorize the role for all resources.
    /// </summary>
    [DataMember(Name = "resourceNames", Order = 4), JsonPropertyName("resourceNames"), Required, MinLength(1)]
    public virtual List<string>? ResourceNames { get; set; } = null!;

}
