using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;

namespace Hylo.Api.Admission.Infrastructure.Services;

/// <summary>
/// Represents an <see cref="IResourceMutator"/> implementation that uses webhooks to validate <see cref="V1Resource"/>s
/// </summary>
public class V1WebhookResourceValidator
    : IResourceValidator
{

    /// <summary>
    /// Initializes a new <see cref="V1WebhookResourceValidator"/>
    /// </summary>
    /// <param name="loggerFactory">the service used to create <see cref="ILogger"/>s</param>
    /// <param name="httpClientFactory">The service used to create <see cref="System.Net.Http.HttpClient"/>s</param>
    /// <param name="webhook">The <see cref="V1MutatingWebhook"/> to execute</param>

    public V1WebhookResourceValidator(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, V1MutatingWebhook webhook)
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
    public virtual async Task ValidateAsync(V1ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context)); Logger.LogDebug("Validating resource '{resource}' using dynamic admission webhook '{webhook}'...", context.ResourceReference, this.Webhook);
        var admissionRequest = new V1ResourceAdmissionReviewRequest(context.Resource);
        var admissionReview = new V1ResourceAdmissionReview(admissionRequest);
        var json = Serializer.Json.Serialize(admissionReview);
        using var request = new HttpRequestMessage(HttpMethod.Post, this.Webhook.Spec.Client.Uri) { Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json) };
        //todo: add payload signature: if (webhook.Spec.Client.Key != null) request.Headers.AddContentSignature();
        using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        //todo: verify response payload signature
        admissionReview = await response.Content.ReadFromJsonAsync<V1ResourceAdmissionReview>();
        if (admissionReview?.Response == null || !admissionReview.Response.Id.Equals(admissionRequest.Id, StringComparison.OrdinalIgnoreCase)) throw new Exception();
        if (admissionReview.Response.Succeeded) this.Logger.LogDebug("Resource '{resource}' succesfully validated by webhook '{webhook}'", context.ResourceReference, this.Webhook);
        else this.Logger.LogDebug("Dynamic admission webhook '{webhook}' failed to validate resource '{resource}'. Errors:/r/n{errors}", this.Webhook, context.ResourceReference, admissionReview.Response.Errors == null ? string.Empty : string.Join(Environment.NewLine, admissionReview.Response.Errors.Select(e => $"{e.Code}: {e.Message}")));
        context.Reviews.Add(admissionReview);
    }

}