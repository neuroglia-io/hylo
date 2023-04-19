namespace Hylo;

/// <summary>
/// Represents the base class of all cloud stream resources
/// </summary>
[DataContract]
public class Resource
    : IResource
{

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    public Resource() { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the <see cref="Resource"/>'s definition</param>
    public Resource(ResourceDefinitionInfo definition) 
    {
        this.Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        this.ApiVersion = this.Definition.GetApiVersion();
        this.Kind = this.Definition.Kind;
    }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the <see cref="Resource"/>'s definition</param>
    /// <param name="metadata">The object that describes the resource</param>
    public Resource(ResourceDefinitionInfo definition, ResourceMetadata metadata)
        : this(definition)
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    /// <summary>
    /// Gets the resource's API version
    /// </summary>
    [Required]
    [DataMember(Order = -999, Name = "apiVersion", IsRequired = true), JsonPropertyOrder(-999), JsonPropertyName("apiVersion"), YamlMember(Order = -999, Alias = "apiVersion")]
    public virtual string ApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets the resource's kind
    /// </summary>
    [Required]
    [DataMember(Order = -998, Name = "kind"), JsonPropertyOrder(-998), JsonPropertyName("kind"), YamlMember(Order = -998, Alias = "kind")]
    public virtual string Kind { get; set; } = null!;

    /// <summary>
    /// Gets/sets the object that describes the resource
    /// </summary>
    [Required]
    [DataMember(Order = -997, Name = "metadata", IsRequired = true), JsonPropertyOrder(-997), JsonPropertyName("metadata"), YamlMember(Order = -997, Alias = "metadata")]
    public virtual ResourceMetadata Metadata { get; set; } = null!;

    object IMetadata.Metadata => this.Metadata;

    /// <inheritdoc/>
    [IgnoreDataMember, JsonIgnore, YamlIgnore]
    public virtual ResourceDefinitionInfo Definition { get; } = null!;

    /// <inheritdoc/>
    [DataMember(Order = 999, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

    /// <inheritdoc/>
    public override string ToString() => this.IsNamespaced() ? $"{this.GetNamespace()}/{this.GetName()}" : this.GetName();

}

/// <summary>
/// Represents the base class of all cloud stream resources
/// </summary>
/// <typeparam name="TSpec">The type of the resource's spec</typeparam>
[DataContract]
public class Resource<TSpec>
    : Resource, IResource<TSpec>
    where TSpec : class, new()
{

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    public Resource() { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the <see cref="Resource"/>'s definition</param>
    public Resource(ResourceDefinitionInfo definition) : base(definition) { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the resource's definition</param>
    /// <param name="metadata">The object that describes the resource</param>
    /// <param name="spec">The resource's spec</param>
    public Resource(ResourceDefinitionInfo definition, ResourceMetadata metadata, TSpec spec)
        : base(definition, metadata)
    {
        this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

    /// <summary>
    /// Gets/sets the object used to define and configure the resource
    /// </summary>
    [DataMember(Order = -996, Name = "spec"), JsonPropertyOrder(-996), JsonPropertyName("spec"), YamlMember(Order = -996, Alias = "spec")]
    public virtual TSpec Spec { get; set; } = null!;

    object ISpec.Spec => this.Spec;

}

/// <summary>
/// Represents the base class of all cloud stream resources
/// </summary>
/// <typeparam name="TSpec">The type of the resource's spec</typeparam>
/// <typeparam name="TStatus">The type of the resource's status</typeparam>
[DataContract]
public class Resource<TSpec, TStatus>
    : Resource<TSpec>, IResource<TSpec, TStatus>
    where TSpec : class, new()
    where TStatus : class, new()
{

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    public Resource() { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the <see cref="Resource"/>'s type</param>
    public Resource(ResourceDefinitionInfo definition) : base(definition) { }

    /// <summary>
    /// Initializes a new <see cref="Resource"/>
    /// </summary>
    /// <param name="definition">An object used to describe the resource's type</param>
    /// <param name="metadata">The object that describes the resource</param>
    /// <param name="spec">The resource's spec</param>
    /// <param name="status">An object that describes the resource's status</param>
    public Resource(ResourceDefinitionInfo definition, ResourceMetadata metadata, TSpec spec, TStatus? status = null)
        : base(definition, metadata, spec)
    {
        this.Status = status;
    }

    /// <summary>
    /// Gets/sets an object that describes the resource's status, if any
    /// </summary>
    [DataMember(Order = -995, Name = "status"), JsonPropertyOrder(-995), JsonPropertyName("status"), YamlMember(Order = -995, Alias = "status")]
    public virtual TStatus? Status { get; set; }

    object? IStatus.Status => this.Status;

    object? ISubResource<IStatus>.SubResource => this.Status;

    TStatus? ISubResource<IStatus, TStatus>.SubResource => this.Status;

}
