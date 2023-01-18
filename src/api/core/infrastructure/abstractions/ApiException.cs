using Hylo;
using Hylo.Api.Core.Infrastructure.Properties;
using System.Net;

namespace Hylo.Api;

/// <summary>
/// Represents an <see cref="Exception"/> thrown by the Hylo API
/// </summary>
public class ApiException
    : Exception
{

    /// <summary>
    /// Initializes a new <see cref="ApiException"/>
    /// </summary>
    /// <param name="status">The response status associated to the <see cref="ApiException"/></param>
    /// <param name="title">The <see cref="ApiException"/>'s title</param>
    /// <param name="detail">The <see cref="ApiException"/>'s detail message</param>
    /// <param name="errors">An <see cref="IDictionary{TKey, TValue}"/> containing the <see cref="ApiException"/>'s inner errors, if any</param>
    public ApiException(int status, string title, string? detail = null, IDictionary<string, string[]>? errors = null)
        : base(detail)
    {
        this.Status = status;
        this.Title = title;
        this.Detail = detail;
        this.Errors = errors;
    }

    /// <summary>
    /// Gets the response status associated to the <see cref="ApiException"/>
    /// </summary>
    public int Status { get; }

    /// <summary>
    /// Gets the <see cref="ApiException"/>'s title
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the <see cref="ApiException"/>'s detail message
    /// </summary>
    public string? Detail { get; }

    /// <summary>
    /// Gets an <see cref="IDictionary{TKey, TValue}"/> containing the <see cref="ApiException"/>'s inner errors, if any
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; }

    /// <summary>
    /// Creates a new <see cref="ApiException"/> thrown when a resource is badly formatted
    /// </summary>
    /// <returns>A new <see cref="ApiException"/></returns>
    public static ApiException InvalidResourceFormat() => new((int)HttpStatusCode.BadRequest, ApiProblemTitles.InvalidResourceFormat);

    public static ApiException ResourceStorageVersionNotFound(V1ResourceReference resource) => new((int)HttpStatusCode.NotFound, ApiProblemTitles.NotFound, StringExtensions.Format(ApiProblemDetails.ResourceDefinitionStorageVersionNotFound, resource.Group, resource.Plural));

    public static ApiException ResourceConversionFailed(string group, string version, string plural, string toVersion, params KeyValuePair<string, string[]>[]? errors) 
        => new((int)HttpStatusCode.UnprocessableEntity, StringExtensions.Format(ApiProblemTitles.ResourceVersionConversionFailed, group, version, plural, toVersion), errors: errors?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

    public static ApiException ResourceConversionFailed(V1ResourceReference resource, string toVersion, params KeyValuePair<string, string[]>[]? errors) => ResourceConversionFailed(resource.Group, resource.Version, resource.Plural, toVersion, errors);

    public static ApiException ResourceDefinitionVersionNotFound(V1ResourceReference resource) => new((int)HttpStatusCode.NotFound, ApiProblemTitles.NotFound, StringExtensions.Format(ApiProblemDetails.ResourceDefinitionVersionNotFound, resource.Group, resource.Version, resource.Plural));

    public static ApiException OptimisticConcurrency(long expectedVersion, long actualVersion) => new((int)HttpStatusCode.Conflict, ApiProblemTitles.OptimisticConcurrencyError, StringExtensions.Format(ApiProblemDetails.OptimisticConcurrencyFailed, expectedVersion, actualVersion));

}
