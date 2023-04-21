namespace Hylo;

/// <summary>
/// Represents the definition of a resource type
/// </summary>
[DataContract]
public class ResourceDefinition
    : Resource<ResourceDefinitionSpec>, IResourceDefinition
{

    /// <summary>
    /// Gets/sets the group resource definitions belong to
    /// </summary>
    public static string ResourceGroup { get; set; } = HyloDefaults.ResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of resource definitions
    /// </summary>
    public static string ResourceVersion { get; set; } = "v1";
    /// <summary>
    /// Gets/sets the plural name of resource definitions
    /// </summary>
    public static string ResourcePlural { get; set; } = "resource-definitions";
    /// <summary>
    /// Gets/sets the kind of resource definitions
    /// </summary>
    public static string ResourceKind { get; set; } = "ResourceDefinition";

    /// <summary>
    /// Gets the definition of <see cref="ResourceDefinition"/>s
    /// </summary>
    public static ResourceDefinition Instance { get; set; } = Serializer.Yaml.Deserialize<ResourceDefinition>(EmbeddedResources.ReadToEnd(EmbeddedResources.Assets.Definitions.ResourceDefinition))!;

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinition"/>
    /// </summary>
    public ResourceDefinition() : base(new (ResourceGroup, ResourceVersion, ResourcePlural, ResourceKind)) { }

    /// <summary>
    /// Initializes a new <see cref="ResourceDefinition"/>
    /// </summary>
    /// <param name="spec">The resource definition's spec</param>
    public ResourceDefinition(ResourceDefinitionSpec spec) : base(new(ResourceGroup, ResourceVersion, ResourcePlural, ResourceKind), new($"{spec.Names.Plural}.{spec.Group}"), spec) { }

}
