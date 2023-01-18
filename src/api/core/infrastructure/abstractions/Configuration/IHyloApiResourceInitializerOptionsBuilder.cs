namespace Hylo.Api.Configuration;

/// <summary>
/// Defines the fundamentals of a service used to configure and build <see cref="HyloApiResourceInitializerOptions"/>
/// </summary>
public interface IHyloApiResourceInitializerOptionsBuilder
{

    /// <summary>
    /// Registers a new well-known <see cref="V1ResourceDefinition"/>
    /// </summary>
    /// <param name="resourceDefinition">The well-known <see cref="V1ResourceDefinition"/> to register</param>
    /// <returns>The configured <see cref="IHyloApiResourceInitializerOptionsBuilder"/></returns>
    IHyloApiResourceInitializerOptionsBuilder RegisterResourceDefinition(V1ResourceDefinition resourceDefinition);

    /// <summary>
    /// Builds the <see cref="HyloApiResourceInitializerOptions"/>
    /// </summary>
    /// <returns>The configured <see cref="HyloApiResourceInitializerOptions"/></returns>
    HyloApiResourceInitializerOptions Build();

}
