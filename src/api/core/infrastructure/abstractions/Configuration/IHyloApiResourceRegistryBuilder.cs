namespace Hylo.Api.Configuration;

/// <summary>
/// Defines the fundamentals of a service used to build the resource registry of an Hylo API
/// </summary>
public interface IHyloApiResourceRegistryBuilder
{

    /// <summary>
    /// Registers the specified <see cref="V1Resource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="V1Resource"/> to register</typeparam>
    /// <param name="resource">The <see cref="V1Resource"/> to register</param>
    /// <returns>The configured <see cref="IHyloApiResourceRegistryBuilder"/></returns>
    IHyloApiResourceBuilder Register<TResource>(TResource resource);

}
