namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a request to review a resource-related authorization
/// </summary>
[DataContract]
public class V1ResourceAuthorizationReviewRequest
{

    /// <summary>
    /// Initializes a new <see cref="V1ResourceAuthorizationReviewRequest"/>
    /// </summary>
    public V1ResourceAuthorizationReviewRequest() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceAuthorizationReviewRequest"/>
    /// </summary>
    /// <param name="subject">A reference of the subject to authorize</param>
    /// <param name="verb">The verb that describes the operation to perform</param>
    /// <param name="group">The API group of the resource to operate on</param>
    /// <param name="apiVersion">The version of the API the resource to operate on belongs to</param>
    /// <param name="pluralName">The plural name of the type of the resource to operate on</param>
    /// <param name="name">The name of the resource to operate on</param>
    /// <param name="namespace">The namespace of the resource to operate on, if any</param>
    public V1ResourceAuthorizationReviewRequest(V1ResourceReference subject, string verb, string group, string apiVersion, string pluralName, string? name, string? @namespace)
    {
        this.Subject = subject;
        this.Verb = verb;
        this.Group = group;
        this.ApiVersion = apiVersion;
        this.PluralName = pluralName;
        this.Name = name;
        this.Namespace = @namespace;
    }

    /// <summary>
    /// Gets/sets a reference of the subject to authorize
    /// </summary>
    [DataMember(Name = "subject", Order = 1), JsonPropertyName("subject"), Required]
    public virtual V1ResourceReference Subject { get; set; } = null!;

    /// <summary>
    /// Gets/sets the verb that describes the operation to perform
    /// </summary>
    [DataMember(Name = "verb", Order = 2), JsonPropertyName("verb"), Required, MinLength(1)]
    public virtual string Verb { get; set; } = null!;

    /// <summary>
    /// Gets/sets the API group of the resource to operate on
    /// </summary>
    [DataMember(Name = "group", Order = 3), JsonPropertyName("group"), Required, MinLength(1)]
    public virtual string Group { get; set; } = null!;

    /// <summary>
    /// Gets/sets the version of the API the resource to operate on belongs to
    /// </summary>
    [DataMember(Name = "apiVersion", Order = 4), JsonPropertyName("apiVersion"), Required, MinLength(1)]
    public virtual string ApiVersion { get; set; } = null!;

    /// <summary>
    /// Gets/sets the plural name of the type of the resource to operate on
    /// </summary>
    [DataMember(Name = "pluralName", Order = 5), JsonPropertyName("pluralName"), Required, MinLength(1)]
    public virtual string PluralName { get; set; } = null!;

    /// <summary>
    /// Gets/sets the name of the resource to operate on
    /// </summary>
    [DataMember(Name = "name", Order = 6), JsonPropertyName("name")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets/sets the namespace of the resource to operate on, if any
    /// </summary>
    [DataMember(Name = "namespace", Order = 7), JsonPropertyName("namespace")]
    public virtual string? Namespace { get; set; }

}
