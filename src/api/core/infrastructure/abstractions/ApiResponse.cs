using Hylo.Api.Core.Infrastructure.Properties;
using Json.Schema;
using System.Net;
using System.Runtime.Serialization;
using YamlDotNet.Serialization;

namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Represents the default implementation sof the <see cref="IResponse"/> interface
/// </summary>
[DataContract]
public class ApiResponse
    : IResponse
{

    /// <summary>
    /// Initializes a new <see cref="ApiResponse"/>
    /// </summary>
    public ApiResponse() { }

    /// <summary>
    /// Initializes a new <see cref="ApiResponse"/>
    /// </summary>
    /// <param name="status">The <see cref="IResponse"/>'s status code</param>
    /// <param name="type">An URI referencing a document that provides human-readable documentation for the response type</param>
    /// <param name="title">A short, human-readable summary of the response type</param>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the response</param>
    /// <param name="instance">An URI reference that identifies the specific occurrence of the response. It may or may not yield further information if dereferenced.</param>
    /// <param name="content">The <see cref="IResponse"/>'s content, if any</param>
    /// <param name="errors">An <see cref="IDictionary{TKey, TValue}"/> containing they code/message mappings of the errors that have occured during the <see cref="IRequest"/>'s execution</param>
    public ApiResponse(int status, Uri? type = null, string? title = null, string? detail = null, Uri? instance = null, object? content = null, IDictionary<string, string[]>? errors = null) 
    {
        this.Status = status;
        this.Type = type;
        this.Title = title;
        this.Detail = detail;
        this.Instance = instance;
        this.Content = content;
        this.Errors = errors;
    }

    /// <inheritdoc/>
    [DataMember(Name = "status", Order = 1), JsonPropertyName("status")]
    public int Status { get; set; }

    /// <inheritdoc/>
    [DataMember(Name = "type", Order = 2), JsonPropertyName("type")]
    public Uri? Type { get; set; }

    /// <inheritdoc/>
    [DataMember(Name = "title", Order = 3), JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <inheritdoc/>
    [DataMember(Name = "details", Order = 4), JsonPropertyName("details")]
    public string? Detail { get; set; }

    /// <inheritdoc/>
    [DataMember(Name = "instance", Order = 5), JsonPropertyName("instance")]
    public Uri? Instance { get; set; }

    /// <inheritdoc/>
    [DataMember(Name = "content", Order = 6), JsonPropertyName("content")]
    public object? Content { get; set; }

    /// <inheritdoc/>
    [DataMember(Name = "errors", Order = 7), JsonPropertyName("errors")]
    public IDictionary<string, string[]>? Errors { get; set; }

    /// <inheritdoc/>
    [DataMember(Name = "extensions", Order = 8), JsonExtensionData]
    public IDictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe a successfull operation
    /// </summary>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse Ok()
    {
        return new((int)HttpStatusCode.OK);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe a successfull operation
    /// </summary>
    /// <typeparam name="TContent">The type of content</typeparam>
    /// <param name="content">The content to wrap</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<TContent> Ok<TContent>(TContent content)
    {
        return new((int)HttpStatusCode.OK, content: content);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe the successfull creation of the specified content
    /// </summary>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse Created()
    {
        return new((int)HttpStatusCode.Created);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe the successfull creation of the specified content
    /// </summary>
    /// <typeparam name="TContent">The type of content</typeparam>
    /// <param name="content">The content to wrap</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<TContent> Created<TContent>(TContent content)
    {
        return new((int)HttpStatusCode.Created, content: content);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe a not found or null reference error
    /// </summary>
    /// <param name="detail">Details about the error</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse NotFound(string? detail = null)
    {
        return new((int)HttpStatusCode.NotFound, title: ApiProblemDetails.ResourceDefinitionNotFound, detail: detail);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe a not found or null reference error
    /// </summary>
    /// <param name="detail">Details about the error</param>
    /// <typeparam name="TContent">The type of content to wrap</typeparam>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<TContent> NotFound<TContent>(string? detail = null)
    {
        return new((int)HttpStatusCode.NotFound, title: ApiProblemTitles.NotFound, detail: detail);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe that a resource could not be found
    /// </summary>
    /// <param name="group">The API group the version that cannot be found belongs to</param>
    /// <param name="version">The version of the resource definition that cannot be found</param>
    /// <param name="plural">The plural name of the resource definition version that cannot be found</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<object> ResourceDefinitionVersionNotFound(string group, string version, string plural)
    {
        return NotFound<object>(StringExtensions.Format(ApiProblemDetails.ResourceDefinitionNotFound, group, version, plural));
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe that a resource definition could not be found
    /// </summary>
    /// <param name="group">The API group the definition that cannot be found belongs to</param>
    /// <param name="version">The version of the definition that cannot be found</param>
    /// <param name="plural">The plural name of the definition that cannot be found</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<T> ResourceDefinitionNotFound<T>(string group, string version, string plural)
    {
        return NotFound<T>(StringExtensions.Format(ApiProblemDetails.ResourceDefinitionNotFound, group, version, plural));
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe that a resource definition could not be found
    /// </summary>
    /// <param name="@namespace">The namespace of the resource that cannot be found</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<object> NamespaceNotFound(string @namespace)
    {
        return NotFound<object>(StringExtensions.Format(ApiProblemDetails.NamespaceNotFound, @namespace));
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe that a resource could not be found
    /// </summary>
    /// <param name="group">The API group the resource that cannot be found belongs to</param>
    /// <param name="version">The version of the resource that cannot be found</param>
    /// <param name="plural">The plural name of the resource that cannot be found</param>
    /// <param name="name">The name of the resource that cannot be found</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<object> ClusterResourceNotFound(string group, string version, string plural, string name)
    {
        return NotFound<object>(StringExtensions.Format(ApiProblemDetails.ClusterResourceNotFound, group, version, plural, name));
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe that a resource could not be found
    /// </summary>
    /// <param name="group">The API group the resource that cannot be found belongs to</param>
    /// <param name="version">The version of the resource that cannot be found</param>
    /// <param name="plural">The plural name of the resource that cannot be found</param>
    /// <param name="name">The name of the resource that cannot be found</param>
    /// <param name="namespace">The namespace of the resource that cannot be found</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<object> NamespacedResourceNotFound(string group, string version, string plural, string name, string @namespace)
    {
        return NotFound<object>(StringExtensions.Format(ApiProblemDetails.NamespacedResourceNotFound, group, version, plural, name, @namespace));
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe that a resource could not be found
    /// </summary>
    /// <param name="group">The API group the resource that failed admission belongs to</param>
    /// <param name="version">The version of the resource that failed admission</param>
    /// <param name="plural">The plural name of the resource that failed admission</param>
    /// <param name="errors">An <see cref="IEnumerable{T}"/> containing the <see cref="V1Error"/>s that have occured during admission</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<object> ResourceAdmissionFailed(string operation, string group, string version, string plural, IEnumerable<V1Error>? errors = null)
    {
        return new((int)HttpStatusCode.BadRequest)
        {
            Title = ApiProblemTitles.ResourceAdmissionFailed,
            Detail = StringExtensions.Format(ApiProblemDetails.ResourceAdmissionFailed, operation, group, version, plural),
            Errors = errors?.GroupBy(kvp => kvp.Code)?.ToDictionary(g => g.Key, g => g.Where(e => !string.IsNullOrWhiteSpace(e.Message)).Select(e => e.Message!).ToArray())
        };
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to inform about a resource validation failure
    /// </summary>
    /// <param name="validationResults">An object that represents the validation results</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<object> ResourceValidationFailed(ValidationResults? validationResults = null)
    {
        return new((int)HttpStatusCode.BadRequest)
        {
            Title = ApiProblemTitles.ValidationFailed,
            Detail = $"{validationResults?.InstanceLocation}: {validationResults?.Message}",
            Errors = validationResults?.NestedResults?.ToDictionary(r => r.InstanceLocation.ToString(), r => r.GetErrorMessages().ToArray())
        };
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> returned when attempting to act on a resource on a wrong API
    /// </summary>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<object> WrongApi()
    {
        return new((int)HttpStatusCode.BadRequest, title: ApiProblemTitles.WrongApi);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> returned when attempting to create a cluster resource by specifying a namespace
    /// </summary>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<object> ClusterResourceCannotDefineNamespace()
    {
        return new((int)HttpStatusCode.BadRequest, title: ApiProblemTitles.InvalidResourceFormat, detail: ApiProblemDetails.ClusterResourceCannotDefineNamespace);
    }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> returned whenever attempting to submit a resource in an invalid/unsupported format
    /// </summary>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<object> InvalidResourceFormat()
    {
        return new((int)HttpStatusCode.BadRequest, title: ApiProblemTitles.InvalidResourceFormat);
    }

}

/// <summary>
/// Represents the default implementation of the <see cref="IResponse{TContent}"/> interface
/// </summary>
public class ApiResponse<TContent>
    : ApiResponse, IResponse<TContent>
{

    /// <inheritdoc/>
    public ApiResponse() { }

    /// <inheritdoc/>
    public ApiResponse(int status, Uri? type = null, string? title = null, string? detail = null, Uri? instance = null, object? content = null, IDictionary<string, string[]>? errors = null) : base(status, type, title, detail, instance, content, errors) { }

    /// <inheritdoc/>
    [JsonIgnore, YamlIgnore]
    public new TContent? Content
    {
        get
        {
            return (TContent?)base.Content;
        }
        set
        {
            base.Content = value;
        }
    }

}
