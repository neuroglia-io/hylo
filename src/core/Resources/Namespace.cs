using Hylo.Resources.Definitions;

namespace Hylo.Resources;

/// <summary>
/// Represents a namespace
/// </summary>
[DataContract]
public record Namespace
    : Resource
{

    /// <summary>
    /// Gets the name of the default namespace
    /// </summary>
    public static string DefaultNamespaceName { get; set; } = "default";
    /// <summary>
    /// Gets/sets the group namespaces belong to
    /// </summary>
    public static string ResourceGroup => NamespaceDefinition.ResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of namespaces
    /// </summary>
    public static string ResourceVersion => NamespaceDefinition.ResourceVersion;
    /// <summary>
    /// Gets/sets the plural name of namespaces
    /// </summary>
    public static string ResourcePlural => NamespaceDefinition.ResourcePlural;
    /// <summary>
    /// Gets/sets the kind of namespaces
    /// </summary>
    public static string ResourceKind => NamespaceDefinition.ResourceKind;

    /// <summary>
    /// Gets the <see cref="Namespace"/>'s resource type.
    /// </summary>
    public static readonly ResourceDefinitionInfo ResourceDefinition = new(ResourceGroup, ResourceVersion, ResourcePlural, ResourceKind);

    /// <summary>
    /// Initializes a new <see cref="Namespace"/>
    /// </summary>
    public Namespace() : base(ResourceDefinition) { }

    /// <summary>
    /// Initializes a new <see cref="Namespace"/>
    /// </summary>
    /// <param name="name">The name of the namespace to create</param>
    public Namespace(string name) : base(ResourceDefinition, new(name)) { }

}