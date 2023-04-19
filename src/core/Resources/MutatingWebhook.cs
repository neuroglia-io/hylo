using Hylo.Resources.Definitions;

namespace Hylo.Resources;

/// <summary>
/// Represents a webhook used to validate <see cref="Resource"/>s
/// </summary>
[DataContract]
public class MutatingWebhook
    : Resource<MutatingWebhookSpec>
{

    /// <summary>
    /// Gets/sets the group mutating webhooks belong to
    /// </summary>
    public static string ResourceGroup => MutatingWebhookDefinition.ResourceGroup;
    /// <summary>
    /// Gets/sets the resource version of mutating webhooks
    /// </summary>
    public static string ResourceVersion => MutatingWebhookDefinition.ResourceVersion;
    /// <summary>
    /// Gets/sets the plural name of mutating webhooks
    /// </summary>
    public static string ResourcePlural => MutatingWebhookDefinition.ResourcePlural;
    /// <summary>
    /// Gets/sets the kind of mutating webhooks
    /// </summary>
    public static string ResourceKind => MutatingWebhookDefinition.ResourceKind;

    /// <summary>
    /// Gets the <see cref="MutatingWebhook"/>'s resource type.
    /// </summary>
    public static ResourceDefinition ResourceDefinition { get; set; } = Serializer.Yaml.Deserialize<ResourceDefinition>(EmbeddedResources.ReadToEnd(EmbeddedResources.Assets.Definitions.MutatingWebhook))!;

    /// <summary>
    /// Initializes a new <see cref="MutatingWebhook"/>
    /// </summary>
    public MutatingWebhook() : base(ResourceDefinition!) { }

    /// <summary>
    /// Initializes a new <see cref="MutatingWebhook"/>
    /// </summary>
    /// <param name="metadata">The <see cref="MutatingWebhook"/>'s metadata</param>
    /// <param name="spec">The <see cref="MutatingWebhook"/>'s <see cref="MutatingWebhookSpec"/></param>
    public MutatingWebhook(ResourceMetadata metadata, MutatingWebhookSpec spec)
        : base(ResourceDefinition!, metadata, spec)
    {

    }

}
