using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IResourceVersionControl"/> interface
/// </summary>
public class ResourceVersionControl
    : IResourceVersionControl
{

    /// <summary>
    /// Initializes a new <see cref="ResourceVersionControl"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="resources">The service used to manage <see cref="V1Resource"/>s</param>
    /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> used to perform webhook requests</param>
    public ResourceVersionControl(ILoggerFactory loggerFactory, IResourceRepository resources, HttpClient httpClient)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.Resources = resources;
        this.HttpClient = httpClient;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the service used to manage <see cref="V1Resource"/>s
    /// </summary>
    protected IResourceRepository Resources { get; }

    /// <summary>
    /// Gets the <see cref="System.Net.Http.HttpClient"/> used to perform webhook requests
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <inheritdoc/>
    public virtual async Task<object> ConvertToStorageVersionAsync(ResourceVersioningContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (context.ResourceDefinition.Spec.Conversion?.Strategy == V1ResourceConversionStrategy.None) return context.Resource;
        if (!context.ResourceDefinition.TryGetVersion(context.ResourceReference.Version, out var version) || version == null) throw ApiException.ResourceDefinitionVersionNotFound(context.ResourceReference);
        var resource = context.Resource;
        if (version.Storage) return resource;
        var storageVersion = context.ResourceDefinition.GetStorageVersion();
        if (storageVersion == null) throw ApiException.ResourceStorageVersionNotFound(context.ResourceReference);
        return (context.ResourceDefinition.Spec.Conversion?.Strategy) switch
        {
            V1ResourceConversionStrategy.Webhook => await this.PerformWebhookConversionAsync(context, context.ResourceDefinition.Spec.Conversion?.Webhook?.Client, version, storageVersion, cancellationToken),
            _ => throw new NotSupportedException($"The specified strategy '{context.ResourceDefinition.Spec.Conversion?.Strategy}' is not supported")
        };
    }

    /// <summary>
    /// Performs the conversion of the current resource to the specified version
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<object> PerformWebhookConversionAsync(ResourceVersioningContext context, V1WebhookClientConfiguration? webhook, 
        V1ResourceDefinitionVersion fromVersion, V1ResourceDefinitionVersion toVersion, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (webhook == null) throw new ArgumentNullException(nameof(context));
        if (fromVersion == null) throw new ArgumentNullException(nameof(fromVersion));
        if (toVersion == null) throw new ArgumentNullException(nameof(toVersion));
        var conversionReview = new V1ConversionReview(new(toVersion.Name, context.Resource));
        using var response = await this.HttpClient.PostAsJsonAsync(webhook.Uri, conversionReview, cancellationToken);
        response.EnsureSuccessStatusCode();
        conversionReview = await response.Content.ReadFromJsonAsync<V1ConversionReview>(cancellationToken: cancellationToken);
        if (conversionReview?.Response == null || conversionReview.Response.ConvertedResource == null)
        {
            this.Logger.LogWarning("Versioning webhook {webhook} responded with a success status code '{statusCode}' but did not return a valid conversion response or did not define a valid resource patch", webhook, response.StatusCode);
            throw ApiException.ResourceConversionFailed(context.ResourceReference, toVersion.Name, conversionReview!.Response?.Errors?.Select(e => new KeyValuePair<string, string[]>(e.Code, string.IsNullOrWhiteSpace(e.Message) ? Array.Empty<string>() : new string[] { e.Message })).ToArray());
        }
        if (!conversionReview.Response.Succeeded)
        {
            this.Logger.LogWarning("Versioning webhook {webhook} failed to convert the resource to version '{version}'" , webhook, toVersion.Name);
            throw ApiException.ResourceConversionFailed(context.ResourceReference, toVersion.Name, conversionReview.Response?.Errors?.Select(e => new KeyValuePair<string, string[]>(e.Code, string.IsNullOrWhiteSpace(e.Message) ? Array.Empty<string>() : new string[] { e.Message })).ToArray());
        }
        this.Logger.LogDebug("Resource '{resource}' succesfully converted to version '{version}'", context.ResourceReference, toVersion.Name);
        return conversionReview.Response.ConvertedResource;
    }

}
