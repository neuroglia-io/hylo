namespace Hylo.Resources.Definitions;

/// <summary>
/// Represents the definition of resource namespaces
/// </summary>
[DataContract]
public class NamespaceDefinition
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
    public static string ResourceSingular { get; set; } = "namespace";
    /// <summary>
    /// Gets/sets the plural name of namespaces
    /// </summary>
    public new static string ResourcePlural { get; set; } = "namespaces";
    /// <summary>
    /// Gets/sets the kind of namespaces
    /// </summary>
    public new static string ResourceKind { get; set; } = "Namespace";
    /// <summary>
    /// Gets/sets the short names of namespaces
    /// </summary>
    public static HashSet<string> ResourceShortNames { get; set; } = new() { "n", "ns" };

    /// <summary>
    /// Initializes a new <see cref="NamespaceDefinition"/>
    /// </summary>
    public NamespaceDefinition() : base(new(ResourceScope.Cluster, ResourceGroup, new(ResourceSingular, ResourcePlural, ResourceKind, ResourceShortNames), new ResourceDefinitionVersion(ResourceVersion, new()) { Served = true, Storage = true })) { }

}
