using System.Net.Http.Json;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents the default implementation of the <see cref="IVersionControl"/> interface
/// </summary>
public class VersionControl
    : IVersionControl
{

    /// <summary>
    /// Initializes a new <see cref="VersionControl"/>
    /// </summary>
    /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
    /// <param name="httpClientFactory">The services used to create <see cref="System.Net.Http.HttpClient"/>s</param>
    public VersionControl(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.HttpClient = httpClientFactory.CreateClient();
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="System.Net.Http.HttpClient"/> used to perform webhook requests
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <inheritdoc/>
    public virtual async Task<IResource> ConvertToStorageVersionAsync(ResourceVersioningContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (context.ResourceDefinition.Spec.Conversion?.Strategy == ConversionStrategy.None) return context.Resource;
        if (!context.ResourceDefinition.TryGetVersion(context.ResourceReference.Definition.Version, out var version) || version == null) throw new HyloException(ProblemDetails.ResourceVersionNotFound(context.ResourceReference));
        var resource = context.Resource;
        if (version.Storage) return resource;
        var storageVersion = context.ResourceDefinition.GetStorageVersion();
        return storageVersion == null
            ? throw new HyloException(ProblemDetails.ResourceStorageVersionNotFound(context.ResourceReference.Definition))
            : (context.ResourceDefinition.Spec.Conversion?.Strategy) switch
        {
            ConversionStrategy.Webhook => await this.PerformWebhookConversionAsync(context, context.ResourceDefinition.Spec.Conversion?.Webhook?.Client, version, storageVersion, cancellationToken),
            _ => throw new NotSupportedException($"The specified strategy '{context.ResourceDefinition.Spec.Conversion?.Strategy}' is not supported")
        };
    }

    /// <summary>
    /// Performs the conversion of the current resource to the specified version
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<IResource> PerformWebhookConversionAsync(ResourceVersioningContext context, WebhookClientConfiguration? webhook,
        ResourceDefinitionVersion fromVersion, ResourceDefinitionVersion toVersion, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (webhook == null) throw new ArgumentNullException(nameof(context));
        if (fromVersion == null) throw new ArgumentNullException(nameof(fromVersion));
        if (toVersion == null) throw new ArgumentNullException(nameof(toVersion));
        var conversionReview = new ConversionReview(new(Guid.NewGuid().ToShortString(), toVersion.Name, context.Resource));
        using var response = await this.HttpClient.PostAsJsonAsync(webhook.Uri, conversionReview, cancellationToken);
        response.EnsureSuccessStatusCode();
        conversionReview = await response.Content.ReadFromJsonAsync<ConversionReview>(cancellationToken: cancellationToken);
        if (conversionReview?.Response == null || conversionReview.Response.ConvertedResource == null)
        {
            this.Logger.LogWarning("Versioning webhook {webhook} responded with a success status code '{statusCode}' but did not return a valid conversion response or did not define a valid resource patch", webhook, response.StatusCode);
            throw new HyloException(ProblemDetails.ResourceConversionFailed(context.ResourceReference, toVersion.Name, conversionReview!.Response?.Errors?.ToArray()!));
        }
        if (!conversionReview.Response.Succeeded)
        {
            this.Logger.LogWarning("Versioning webhook {webhook} failed to convert the resource to version '{version}'", webhook, toVersion.Name);
            throw new HyloException(ProblemDetails.ResourceConversionFailed(context.ResourceReference, toVersion.Name, conversionReview.Response?.Errors?.ToArray()!));
        }
        this.Logger.LogDebug("Resource '{resource}' succesfully converted to version '{version}'", context.ResourceReference, toVersion.Name);
        return conversionReview.Response.ConvertedResource;
    }

}