namespace Hylo.Api.Admission.Infrastructure.Services;

/// <summary>
/// Represents a base class for all <see cref="IResourceValidator"/> implementations
/// </summary>
/// <typeparam name="TResource">The type of <see cref="V1Resource"/> to validate</typeparam>
public abstract class ResourceValidator<TResource>
    : IResourceValidator<TResource>
    where TResource : V1Resource, new()
{

    /// <summary>
    /// Initializes a new <see cref="ResourceValidator{TResource}"/>
    /// </summary>
    protected ResourceValidator()
    {
        if (!this.GetType().TryGetCustomAttribute<ResourceAttribute>(out var resourceAttribute) || resourceAttribute == null) throw new NotSupportedException($"The specified resource type '{typeof(TResource).FullName}' is not marked with the required 'Resource' attribute");
        this.ResourceApiVersion = ApiVersion.Build(resourceAttribute.Group, resourceAttribute.Version);
        this.ResourceKind = resourceAttribute.Kind;
    }

    /// <summary>
    /// Gets the API version of resources to validate
    /// </summary>
    protected string ResourceApiVersion { get; }

    /// <summary>
    /// Gets the kind of resource to validate
    /// </summary>
    protected string ResourceKind { get; }

    /// <inheritdoc/>
    public virtual bool SupportsResourceType(V1ResourceDefinition resourceDefinition)
    {
        if (resourceDefinition == null) throw new ArgumentNullException(nameof(resourceDefinition));
        return this.ResourceApiVersion == ApiVersion.Build(resourceDefinition.Spec.Group, resourceDefinition.Spec.Version) && this.ResourceKind == resourceDefinition.Spec.Names.Kind;
    }

    async Task IResourceValidator.ValidateAsync(V1ResourceAdmissionReviewContext context, CancellationToken cancellationToken)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        await this.ValidateAsync(context.OfType<TResource>(), cancellationToken);
    }

    public abstract Task<V1Patch> ValidateAsync(V1ResourceAdmissionReviewContext<TResource> context, CancellationToken cancellationToken);

}