using Hylo.Resources.Definitions;

namespace Hylo.Resources;

/// <summary>
/// Represents a webhook used to validate <see cref="IResource"/>s
/// </summary>
[DataContract]
public record ValidatingWebhook
    : Resource<ValidatingWebhookSpec>
{

    /// <summary>
    /// Gets/sets the group validating webhooks belong to
    /// </summary>
    public static string ResourceGroup => ValidatingWebhookDefinition.ResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of validating webhooks
    /// </summary>
    public static string ResourceVersion => ValidatingWebhookDefinition.ResourceVersion;
    /// <summary>
    /// Gets/sets the plural name of validating webhooks
    /// </summary>
    public static string ResourcePlural => ValidatingWebhookDefinition.ResourcePlural;
    /// <summary>
    /// Gets/sets the kind of validating webhooks
    /// </summary>
    public static string ResourceKind => ValidatingWebhookDefinition.ResourceKind;

    /// <summary>
    /// Gets the <see cref="ValidatingWebhook"/>'s resource type.
    /// </summary>
    public static ResourceDefinition ResourceDefinition { get; set; } = Serializer.Yaml.Deserialize<ResourceDefinition>(EmbeddedResources.ReadToEnd(EmbeddedResources.Assets.Definitions.ValidatingWebhook))!;

    /// <summary>
    /// Initializes a new <see cref="ValidatingWebhook"/>
    /// </summary>
    public ValidatingWebhook() : base(new ResourceDefinitionInfo(ResourceGroup, ResourceVersion, ResourcePlural, ResourceKind)) { }

    /// <summary>
    /// Initializes a new <see cref="ValidatingWebhook"/>
    /// </summary>
    /// <param name="metadata">The <see cref="ValidatingWebhook"/>'s metadata</param>
    /// <param name="spec">The <see cref="ValidatingWebhook"/>'s <see cref="ValidatingWebhookSpec"/></param>
    public ValidatingWebhook(ResourceMetadata metadata, ValidatingWebhookSpec spec)
        : this()
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

}
