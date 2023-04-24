using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents an <see cref="IResourceMutator"/> implementation that uses webhooks to validate <see cref="Resource"/>s
/// </summary>
public class WebhookResourceValidator
    : IResourceValidator
{

    /// <summary>
    /// Initializes a new <see cref="WebhookResourceValidator"/>
    /// </summary>
    /// <param name="loggerFactory">the service used to create <see cref="ILogger"/>s</param>
    /// <param name="httpClientFactory">The service used to create <see cref="System.Net.Http.HttpClient"/>s</param>
    /// <param name="webhook">The <see cref="MutatingWebhook"/> to execute</param>

    public WebhookResourceValidator(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, MutatingWebhook webhook)
    {
        this.Logger = loggerFactory.CreateLogger(this.GetType());
        this.HttpClient = httpClientFactory.CreateClient(this.GetType().Name);
        this.Webhook = webhook;
    }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="System.Net.Http.HttpClient"/> used to perform webhook requests
    /// </summary>
    protected HttpClient HttpClient { get; }

    /// <summary>
    /// Gets the <see cref="MutatingWebhook"/> to use to mutate the current resource
    /// </summary>
    public virtual MutatingWebhook Webhook { get; }

    /// <inheritdoc/>
    public virtual bool AppliesTo(Operation operation, string group, string version, string plural, string? @namespace = null) => this.Webhook.Spec.Resources?.Any(r => r.Matches(operation, group, version, plural, @namespace)) == true;

    /// <inheritdoc/>
    public virtual async Task<AdmissionReviewResponse> ValidateAsync(AdmissionReviewRequest admissionRequest, CancellationToken cancellationToken = default)
    {
        if (admissionRequest == null) throw new ArgumentNullException(nameof(admissionRequest));
        var admissionReview = new AdmissionReview(admissionRequest);
        var json = Serializer.Json.Serialize(admissionReview);
        using var request = new HttpRequestMessage(HttpMethod.Post, this.Webhook.Spec.Client.Uri) { Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json) };
        //todo: add payload signature: if (webhook.Spec.Client.Key != null) request.Headers.AddContentSignature();
        using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        //todo: verify response payload signature
        admissionReview = await response.Content.ReadFromJsonAsync<AdmissionReview>(cancellationToken: cancellationToken);
        if (admissionReview?.Response == null || !admissionReview.Response.Uid.Equals(admissionRequest.Uid, StringComparison.OrdinalIgnoreCase))
        {
            return new(admissionRequest.Uid, false, null, ProblemDetails.ResourceAdmissionFailed(admissionRequest.Operation, admissionRequest.Resource, new KeyValuePair<string, string[]>(this.Webhook.ToString(), new[] { $"Validating webhook {this.Webhook} responded with a success status code '{response.StatusCode}' but did not return a valid validating admission response or the response UID does not one the request's" })));
        }
        return admissionReview.Response;
    }

}