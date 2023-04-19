using Microsoft.Extensions.Logging;
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
    public virtual bool AppliesTo(ResourceOperation operation, string group, string version, string plural, string? @namespace = null) => this.Webhook.Spec.Resources?.Any(r => r.Matches(operation, group, version, plural, @namespace)) == true;

    /// <inheritdoc/>
    public virtual async Task ValidateAsync(ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default)
    {
        if (context == null) throw new ArgumentNullException(nameof(context)); Logger.LogDebug("Validating resource '{resource}' using dynamic admission webhook '{webhook}'...", context.Resource, this.Webhook);
        var admissionReview = context.ToAdmissionReview();
        var admissionRequest = admissionReview.Request!;
        var json = Serializer.Json.Serialize(admissionReview);
        using var request = new HttpRequestMessage(HttpMethod.Post, this.Webhook.Spec.Client.Uri) { Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json) };
        //todo: add payload signature: if (webhook.Spec.Client.Key != null) request.Headers.AddContentSignature();
        using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        //todo: verify response payload signature
        admissionReview = await response.Content.ReadFromJsonAsync<AdmissionReview>(cancellationToken: cancellationToken);
        if (admissionReview?.Response == null || !admissionReview.Response.Uid.Equals(admissionRequest.Uid, StringComparison.OrdinalIgnoreCase)) throw new Exception();
        if (admissionReview.Response.Allowed) this.Logger.LogDebug("Resource '{resource}' succesfully validated by webhook '{webhook}'", context.Resource, this.Webhook);
        else this.Logger.LogDebug("Dynamic admission webhook '{webhook}' failed to validate resource '{resource}'. Errors:/r/n{errors}", this.Webhook, context.Resource, admissionReview.Response.Problem?.Errors == null ? string.Empty : string.Join(Environment.NewLine, admissionReview.Response.Problem.Errors.Select(e => $"{e.Key}: {string.Join(Environment.NewLine, e.Value)}")));
        context.Reviews.Add(admissionReview);
    }

}