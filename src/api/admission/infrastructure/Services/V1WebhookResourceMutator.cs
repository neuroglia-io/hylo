using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Hylo.Api.Admission.Infrastructure.Services;

/// <summary>
/// Represents an <see cref="IResourceMutator"/> implementation that uses webhooks to mutate <see cref="V1Resource"/>s
/// </summary>
public class V1WebhookResourceMutator
    : IResourceMutator
{

    /// <summary>
    /// Initializes a new <see cref="V1WebhookResourceMutator"/>
    /// </summary>
    /// <param name="loggerFactory">the service used to create <see cref="ILogger"/>s</param>
    /// <param name="httpClientFactory">The service used to create <see cref="System.Net.Http.HttpClient"/>s</param>
    /// <param name="webhook">The <see cref="V1MutatingWebhook"/> to execute</param>

    public V1WebhookResourceMutator(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, V1MutatingWebhook webhook)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.HttpClient = httpClientFactory.CreateClient(this.GetType().Name);
        this.Webhook = webhook;
    }

    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="System.Net.Http.HttpClient"/> used to perform webhook requests
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// Gets the <see cref="V1MutatingWebhook"/> to use to mutate the current resource
    /// </summary>
    public virtual V1MutatingWebhook Webhook { get; }

    /// <inheritdoc/>
    public virtual int Priority => this.Webhook.Spec.Priority ?? int.MaxValue;

    /// <inheritdoc/>
    public virtual bool SupportsResourceType(V1ResourceDefinition resourceDefinition) => this.Webhook.Spec.Resources?.Any(r => r.Matches(resourceDefinition.Spec.Group, resourceDefinition.Spec.Version, resourceDefinition.Spec.Names.Plural, r.Operations?.FirstOrDefault()!)) == true;

    /// <inheritdoc/>
    public virtual async Task MutateAsync(V1ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default)
    {
        if(context == null) throw new ArgumentNullException(nameof(context));
        this.Logger.LogDebug("Mutating resource '{resource}' using webhook '{webhook}'...", context.ResourceReference, this.Webhook);
        var admissionReview = new V1ResourceAdmissionReview(new V1ResourceAdmissionReviewRequest(context.Resource));
        using var response = await HttpClient.PostAsJsonAsync(this.Webhook.Spec.Client.Uri, admissionReview, cancellationToken);
        response.EnsureSuccessStatusCode();
        admissionReview = await response.Content.ReadFromJsonAsync<V1ResourceAdmissionReview>(cancellationToken: cancellationToken);
        if (admissionReview?.Response == null || admissionReview.Response.Patch == null)
        {
            this.Logger.LogWarning("Mutating webhook {webhook} responded with a success status code '{statusCode}' but did not return a valid mutating admission response or did not define a valid resource patch", this.Webhook, response.StatusCode);
            return;
        }
        context.Reviews.Add(admissionReview);
        if (admissionReview.Response.Succeeded != true)
        {
            this.Logger.LogWarning("Mutating webhook {webhook} failed to mutate resource '{resource}':\r\n{errors}", this.Webhook, context.ResourceReference, admissionReview.Response?.Errors == null ? "N/A" : string.Join(Environment.NewLine, admissionReview.Response.Errors.Select(e => $"")));
            return;
        }
        context.Resource = admissionReview.Response.Patch.ApplyTo(Serializer.Json.SerializeToNode(context.Resource)!.AsObject()!);
        this.Logger.LogDebug("Resource '{resource}' succesfully mutated using webhook '{webhook}'", context.ResourceReference, this.Webhook);
    }

}
