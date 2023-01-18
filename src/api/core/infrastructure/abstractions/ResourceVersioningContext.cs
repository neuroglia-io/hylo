namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Represents the context of the conversion of a resource version
/// </summary>
public class ResourceVersioningContext
{

    /// <summary>
    /// Initializes a new <see cref="ResourceVersioningContext"/>
    /// </summary>
    /// <param name="resourceReference">An object used to reference the <see cref="V1Resource"/> to evaluate</param>
    /// <param name="resourceDefinition">The <see cref="V1ResourceDefinition"/> of the <see cref="V1Resource"/> to evaluate</param>
    /// <param name="resource">The resource to evaluate for admission</param>
    public ResourceVersioningContext(V1ResourceReference resourceReference, V1ResourceDefinition resourceDefinition, object resource)
    {
        if (resourceReference == null) throw new ArgumentNullException(nameof(resourceReference));
        if (resourceDefinition == null) throw new ArgumentNullException(nameof(resourceDefinition));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        this.ResourceReference = resourceReference;
        this.ResourceDefinition = resourceDefinition;
        this.Resource = resource;
    }

    /// <summary>
    /// Gets an object used to reference the <see cref="V1Resource"/> to evaluate
    /// </summary>
    public V1ResourceReference ResourceReference { get; }

    /// <summary>
    /// Gets the <see cref="V1ResourceDefinition"/> of the <see cref="V1Resource"/> to evaluate
    /// </summary>
    public V1ResourceDefinition ResourceDefinition { get; }

    /// <summary>
    /// Gets the <see cref="V1Resource"/> to evaluate
    /// </summary>
    public object Resource { get; set; }

}
