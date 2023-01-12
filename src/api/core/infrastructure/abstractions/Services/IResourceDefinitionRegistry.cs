namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines the fundamentals of a service used to manage well known <see cref="V1ResourceDefinition"/>s
/// </summary>
public interface IResourceDefinitionRegistry
{

    /// <summary>
    /// Registers the specified <see cref="V1ResourceDefinition"/>
    /// </summary>
    /// <param name="resourceDefinition">The <see cref="V1ResourceDefinition"/> to register</param>
    void Register(V1ResourceDefinition resourceDefinition);

    /// <summary>
    /// Gets the well known <see cref="V1ResourceDefinition"/> with the specified kind
    /// </summary>
    /// <param name="group">The API group the <see cref="V1ResourceDefinition"/> to get belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1ResourceDefinition"/> to get belongs to</param>
    /// <param name="kind">The kind plural name of the <see cref="V1ResourceDefinition"/> to get</param>
    /// <returns>The <see cref="V1ResourceDefinition"/> with the specified kind, if any</returns>
    V1ResourceDefinition? GetByKind(string group, string version, string kind);

    /// <summary>
    /// Gets the well known <see cref="V1ResourceDefinition"/> with the specified plural name
    /// </summary>
    /// <param name="group">The API group the <see cref="V1ResourceDefinition"/> to get belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1ResourceDefinition"/> to get belongs to</param>
    /// <param name="pluralName">The plural name of the <see cref="V1ResourceDefinition"/> to get</param>
    /// <returns>The <see cref="V1ResourceDefinition"/> with the specified plural name, if any</returns>
    V1ResourceDefinition? GetByPluralName(string group, string version, string pluralName);

    /// <summary>
    /// Gets the well known <see cref="V1ResourceDefinition"/> with the specified short name
    /// </summary>
    /// <param name="group">The API group the <see cref="V1ResourceDefinition"/> to get belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1ResourceDefinition"/> to get belongs to</param>
    /// <param name="shortName">The short name of the <see cref="V1ResourceDefinition"/> to get</param>
    /// <returns>The <see cref="V1ResourceDefinition"/> with the specified plural name, if any</returns>
    V1ResourceDefinition? GetByShortName(string group, string version, string shortName);

    /// <summary>
    /// Lists all well known <see cref="V1ResourceDefinition"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all well known <see cref="V1ResourceDefinition"/>s</returns>
    IEnumerable<V1ResourceDefinition> List();

}