namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents the <see cref="IResource{TMetadata}"/> implementation used by the Hylo V1 API
/// </summary>
[DataContract]
public class V1Resource
    : IResource<V1ResourceMetadata>
{

    /// <summary>
    /// Initializes a new <see cref="V1Resource"/>
    /// </summary>
    public V1Resource() { }

    /// <summary>
    /// Initializes a new <see cref="V1Resource"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/> belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/> belongs to</param>
    /// <param name="kind">The <see cref="V1Resource"/>'s kind</param>
    public V1Resource(string group, string version, string kind)
    {
        if(string.IsNullOrWhiteSpace(group)) throw new ArgumentNullException(nameof(group));
        if(string.IsNullOrWhiteSpace(version)) throw new ArgumentNullException(nameof(version));
        if(string.IsNullOrWhiteSpace(kind)) throw new ArgumentNullException(nameof(kind));
        this.ApiVersion = Api.ApiVersion.Build(group, version);
        this.Kind = kind;
    }

    /// <summary>
    /// Initializes a new <see cref="V1Resource"/>
    /// </summary>
    /// <param name="definition">The <see cref="V1Resource"/> definition</param>
    public V1Resource(V1ResourceDefinition definition) : this(definition.Spec.Group, definition.Spec.Version, definition.Spec.Names.Kind) { }

    /// <inheritdoc/>
    [DataMember(Name = "apiVersion", Order = 1), JsonPropertyName("apiVersion"), Required]
    public virtual string ApiVersion { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "kind", Order = 2), JsonPropertyName("kind"), Required]
    public virtual string Kind { get; set; } = null!;

    /// <inheritdoc/>
    [DataMember(Name = "metadata", Order = 3), JsonPropertyName("metadata"), Required]
    public virtual V1ResourceMetadata Metadata { get; set; } = null!;

    /// <summary>
    /// Gets all the properties that can be used to sort <see cref="V1Resource"/>s
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing the names of the properties <see cref="V1Resource"/>s can be sorted by</returns>
    public static IEnumerable<string> GetSortableProperties()
    {
        yield return $"{nameof(ApiVersion).ToCamelCase()}";
        yield return $"{nameof(Kind).ToCamelCase()}";
        yield return $"{nameof(Metadata).ToCamelCase()}.{nameof(V1ResourceMetadata.Id).ToCamelCase()}";
        yield return $"{nameof(Metadata).ToCamelCase()}.{nameof(V1ResourceMetadata.CreatedAt).ToCamelCase()}";
        yield return $"{nameof(Metadata).ToCamelCase()}.{nameof(V1ResourceMetadata.LastModified).ToCamelCase()}";
        yield return $"{nameof(Metadata).ToCamelCase()}.{nameof(V1ResourceMetadata.Name).ToCamelCase()}";
        yield return $"{nameof(Metadata).ToCamelCase()}.{nameof(V1ResourceMetadata.Namespace).ToCamelCase()}";
    }

}

/// <summary>
/// Represents the <see cref="IResource{TMetadata}"/> implementation used by the Hylo V1 API
/// </summary>
/// <typeparam name="TSpec">The type of the <see cref="V1Resource"/>'s spec</typeparam>
[DataContract]
public class V1Resource<TSpec>
    : V1Resource, IResource<V1ResourceMetadata, TSpec>
    where TSpec : class, new()
{

    /// <inheritdoc/>
    public V1Resource() { }

    /// <inheritdoc/>
    public V1Resource(string group, string version, string kind) : base(group, version, kind) { }

    /// <inheritdoc/>
    public V1Resource(V1ResourceDefinition definition) : this(definition.Spec.Group, definition.Spec.Version, definition.Spec.Names.Kind) { }

    /// <inheritdoc/>
    [DataMember(Name = "spec", Order = 1 ), JsonPropertyName("spec")]
    public virtual TSpec Spec { get; set; } = null!;

}

/// <summary>
/// Represents the <see cref="IResource{TMetadata}"/> implementation used by the Hylo V1 API
/// </summary>
/// <typeparam name="TSpec">The type of the <see cref="V1Resource"/>'s spec</typeparam>
/// <typeparam name="TStatus">the type of the <see cref="V1Resource"/>'s status</typeparam>
[DataContract]
public class V1Resource<TSpec, TStatus>
    : V1Resource<TSpec>, IResource<V1ResourceMetadata, TSpec, TStatus>
    where TSpec : class, new()
    where TStatus : class, new()
{

    /// <inheritdoc/>
    public V1Resource(string group, string version, string kind) : base(group, version, kind) { }

    /// <inheritdoc/>
    public V1Resource(V1ResourceDefinition definition) : base(definition.Spec.Group, definition.Spec.Version, definition.Spec.Names.Kind) { }

    /// <inheritdoc/>
    [DataMember(Name = "status", Order = 1), JsonPropertyName("status")]
    public virtual TStatus? Status { get; set; }

}
