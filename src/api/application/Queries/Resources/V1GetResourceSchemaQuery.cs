using Json.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Hylo.Api.Queries.Resources;

/// <summary>
/// Represents the <see cref="IQuery"/> used to get the <see cref="JsonSchema"/> of a specific <see cref="V1ResourceDefinition"/>
/// </summary>
public class V1GetResourceSchemaQuery
  : ApiQuery<JsonSchema?>
{

    /// <summary>
    /// Initializes a new <see cref="V1GetResourceSchemaQuery"/>
    /// </summary>
    public V1GetResourceSchemaQuery() { }

    /// <summary>
    /// Initializes a new <see cref="V1GetResourceSchemaQuery"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/> to get the schema of belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/> to get the schema of belongs to</param>
    /// <param name="plural">The version of the API the <see cref="V1Resource"/> to get the schema of belongs to</param>
    public V1GetResourceSchemaQuery(string group, string version, string plural)
    {
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
    }

    /// <summary>
    /// Gets the API group the <see cref="V1Resource"/> to get the schema of belongs to
    /// </summary>
    [FromRoute(Name = "group"), DataMember(Name = "group", Order = 1), JsonPropertyName("group"), Required]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/> to get the schema of belongs to
    /// </summary>
    [FromRoute(Name = "version"), DataMember(Name = "version", Order = 2), JsonPropertyName("version"), Required]
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/> to get the schema of belongs to
    /// </summary>
    [FromRoute(Name = "plural"), DataMember(Name = "plural", Order = 3), JsonPropertyName("plural"), Required]
    public virtual string Plural { get; set; } = null!;

}

/// <summary>
/// Represents the service used to handle <see cref="V1GetResourceSchemaQuery"/> instances
/// </summary>
public class V1GetResourceSchemaQueryHandler
    : ResourceQueryHandlerBase,
    IQueryHandler<V1GetResourceSchemaQuery, JsonSchema?>
{

    /// <inheritdoc/>
    public V1GetResourceSchemaQueryHandler(ILoggerFactory loggerFactory, IResourceRepository resources, IResourceVersionControl versionControl, IResourceEventBus eventBus) 
        : base(loggerFactory, resources, versionControl, eventBus)
    {
        
    }

    /// <inheritdoc/>
    public virtual async Task<IResponse<JsonSchema?>> Handle(V1GetResourceSchemaQuery query, CancellationToken cancellationToken)
    {
        var resourceDefinition = await this.Resources.GetResourceDefinitionAsync(query.Group, query.Version, query.Plural, cancellationToken);
        if (resourceDefinition == null) return ApiResponse.ResourceDefinitionNotFound<JsonSchema?>(query.Group, query.Version, query.Plural);
        var schema = resourceDefinition.Spec.Versions.First(v => v.Name == resourceDefinition.Spec.Version).Schema;
        return this.Ok(schema);
    }

}