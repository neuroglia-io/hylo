using System.Net;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Hylo.Api.Application;

/// <summary>
/// Represents an object used to describe a response to a request
/// </summary>
[DataContract]
public record ApiResponse
    : ProblemDetails
{

    /// <summary>
    /// Initializes a new <see cref="ApiResponse"/>
    /// </summary>
    public ApiResponse() { }

    /// <summary>
    /// Initialize a new successfull <see cref="ApiResponse"/>
    /// </summary>
    /// <param name="status">The response's status code</param>
    /// <param name="content">The response's content</param>
    public ApiResponse(int status, object? content = null)
    {
        if (!status.IsSuccessStatusCode()) throw new NotSupportedException("This constructor can only be used for successfull responses");
        this.Status = status;
        this.Content = content;
    }

    /// <inheritdoc/>
    public ApiResponse(Uri type, string title, int status, string? detail = null, Uri? instance = null, IDictionary<string, string[]>? errors = null, IDictionary<string, object>? extensionData = null)
        : base(type, title, status, detail, instance, errors, extensionData)
    {

    }

    /// <inheritdoc/>
    [DataMember(Name = "content", Order = 10), JsonPropertyOrder(10), JsonPropertyName("content"), YamlMember(Order = 10, Alias = "content")]
    public virtual object? Content { get; set; }

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe a successfull operation
    /// </summary>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse Ok() => new((int)HttpStatusCode.OK);

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe a successfull operation
    /// </summary>
    /// <typeparam name="TContent">The type of content</typeparam>
    /// <param name="content">The content to wrap</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<TContent> Ok<TContent>(TContent content) => new((int)HttpStatusCode.OK, content: content);

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe a successfull operation
    /// </summary>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse Accepted() => new((int)HttpStatusCode.Accepted);

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe the successfull creation of the specified content
    /// </summary>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse Created() => new((int)HttpStatusCode.Created);

    /// <summary>
    /// Creates a new <see cref="ApiResponse"/> to describe the successfull creation of the specified content
    /// </summary>
    /// <typeparam name="TContent">The type of content</typeparam>
    /// <param name="content">The content to wrap</param>
    /// <returns>A new <see cref="ApiResponse"/></returns>
    public static ApiResponse<TContent> Created<TContent>(TContent content) => new((int)HttpStatusCode.Created, content: content);

}

/// <summary>
/// Describes a response produced to a request
/// </summary>
/// <typeparam name="TContent">The type of content wrapped by the <see cref="ApiResponse"/></typeparam>
public record ApiResponse<TContent>
    : ApiResponse
{

    /// <inheritdoc/>
    public ApiResponse() { }

    /// <inheritdoc/>
    public ApiResponse(int status, object? content = null) : base(status, content) { }

    /// <inheritdoc/>
    public ApiResponse(Uri type, string title, int status, string? detail = null, Uri? instance = null, IDictionary<string, string[]>? errors = null, IDictionary<string, object>? extensionData = null)
        : base(type, title, status, detail, instance, errors, extensionData)
    {

    }

    /// <inheritdoc/>
    [IgnoreDataMember, JsonIgnore, YamlIgnore]
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