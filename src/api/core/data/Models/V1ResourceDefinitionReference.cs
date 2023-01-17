namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a reference to a <see cref="V1ResourceDefinition"/>
/// </summary>
[DataContract]
public class V1ResourceDefinitionReference
{

    /// <summary>
    /// Initializes a new <see cref="V1ResourceDefinitionReference"/>
    /// </summary>
    public V1ResourceDefinitionReference() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceDefinitionReference"/>
    /// </summary>
    /// <param name="apiVersion">The API version of the referenced resource</param>
    /// <param name="kind">The referenced resource's kind</param>
    /// <exception cref="ArgumentNullException"></exception>
    public V1ResourceDefinitionReference(string apiVersion, string kind)
    {
        if (string.IsNullOrWhiteSpace(apiVersion)) throw new ArgumentNullException(nameof(apiVersion));
        if (string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        this.ApiVersion = apiVersion;
        this.Kind = kind;
    }

    /// <summary>
    /// Gets/sets the API version of the referenced resource
    /// </summary>
    [DataMember(Name = "apiVersion", Order = 1), JsonPropertyName("apiVersion"), Required, MinLength(1)]
    public virtual string ApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets/sets the referenced resource's kind
    /// </summary>
    [DataMember(Name = "kind", Order = 2), JsonPropertyName("kind"), Required, MinLength(1)]
    public virtual string Kind { get; set; } = null!;

}