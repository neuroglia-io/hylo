namespace Hylo.Infrastructure.Configuration;

/// <summary>
/// Represents the options used to configure an <see cref="IRepository"/>
/// </summary>
public class ResourceRepositoryOptions
{

    /// <summary>
    /// Gets/sets a <see cref="List{T}"/> containing well known <see cref="IResourceDefinition"/>s to seed the <see cref="IRepository"/> with upon initialization
    /// </summary>
    public virtual List<IResourceDefinition> WellKnownDefinitions { get; set; } = new();

    /// <summary>
    /// Gets/sets a <see cref="List{T}"/> containing well known <see cref="IResource"/>s to seed the <see cref="IRepository"/> with upon initialization
    /// </summary>
    public virtual List<IResource> WellKnownResources { get; set; } = new();

}