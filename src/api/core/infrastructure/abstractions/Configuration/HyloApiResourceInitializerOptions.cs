namespace Hylo.Api.Configuration;

/// <summary>
/// Represents the object to configure the resources of an Hylo API
/// </summary>
public class HyloApiResourceInitializerOptions
{

    /// <summary>
    /// Gets/sets a <see cref="List{T}"/> containing Hylo API well-known <see cref="V1ResourceDefinition"/>s
    /// </summary>
    public virtual List<V1ResourceDefinition>? ResourceDefinitions { get; set; }

}