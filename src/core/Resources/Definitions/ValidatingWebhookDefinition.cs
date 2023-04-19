namespace Hylo.Resources.Definitions;

/// <summary>
/// Represents the definition of validating webhooks
/// </summary>
[DataContract]
public class ValidatingWebhookDefinition
    : ResourceDefinition
{

    /// <summary>
    /// Gets/sets the group namespaces belong to
    /// </summary>
    public new static string ResourceGroup { get; set; } = HyloDefaults.ResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of namespaces
    /// </summary>
    public new static string ResourceVersion { get; set; } = "v1";
    /// <summary>
    /// Gets/sets the singular name of namespaces
    /// </summary>
    public static string ResourceSingular { get; set; } = "validating-webhook";
    /// <summary>
    /// Gets/sets the plural name of namespaces
    /// </summary>
    public new static string ResourcePlural { get; set; } = "validating-webhooks";
    /// <summary>
    /// Gets/sets the kind of namespaces
    /// </summary>
    public new static string ResourceKind { get; set; } = "ValidatingWebhook";
    /// <summary>
    /// Gets/sets the short names of validating webhooks
    /// </summary>
    public static HashSet<string> ResourceShortNames { get; set; } = new() { "vwh" };

    /// <summary>
    /// Initializes a new <see cref="ValidatingWebhookDefinition"/>
    /// </summary>
    public ValidatingWebhookDefinition() : base(new(ResourceScope.Cluster, ResourceGroup, new(ResourceSingular, ResourcePlural, ResourceKind, ResourceShortNames), new ResourceDefinitionVersion(ResourceVersion, new()))) { }

}
