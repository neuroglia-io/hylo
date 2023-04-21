namespace Hylo.Infrastructure;

/// <summary>
/// Represents the context of the conversion of a resource version
/// </summary>
public class VersioningContext
{

    /// <summary>
    /// Initializes a new <see cref="VersioningContext"/>
    /// </summary>
    /// <param name="resourceReference">An object used to reference the <see cref="IResource"/> to evaluate</param>
    /// <param name="resourceDefinition">The <see cref="IResourceDefinition"/> of the <see cref="IResource"/> to evaluate</param>
    /// <param name="resource">The <see cref="IResource"/> to evaluate for admission</param>
    public VersioningContext(IResourceReference resourceReference, IResourceDefinition resourceDefinition, IResource resource)
    {
        this.ResourceReference = resourceReference ?? throw new ArgumentNullException(nameof(resourceReference));
        this.ResourceDefinition = resourceDefinition ?? throw new ArgumentNullException(nameof(resourceDefinition));
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <summary>
    /// Gets an object used to reference the <see cref="IResource"/> to convert
    /// </summary>
    public IResourceReference ResourceReference { get; }

    /// <summary>
    /// Gets the <see cref="IResourceDefinition"/> of the <see cref="IResource"/> to convert
    /// </summary>
    public IResourceDefinition ResourceDefinition { get; }

    /// <summary>
    /// Gets the <see cref="IResource"/> to convert
    /// </summary>
    public IResource Resource { get; set; }

}