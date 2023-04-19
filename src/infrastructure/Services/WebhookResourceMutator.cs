using System.Net.Http.Json;

namespace Hylo.Infrastructure.Services;

/// <summary>
/// Represents an <see cref="IResourceMutator"/> implementation that uses webhooks to mutate <see cref="Resource"/>s
/// </summary>
public class WebhookResourceMutator
    : IResourceMutator
{

    /// <summary>
    /// Initializes a new <see cref="WebhookResourceMutator"/>
    /// </summary>
    /// <param name="loggerFactory">the service used to create <see cref="ILogger"/>s</param>
    /// <param name="httpClientFactory">The service used to create <see cref="System.Net.Http.HttpClient"/>s</param>
    /// <param name="webhook">The <see cref="MutatingWebhook"/> to execute</param>

    public WebhookResourceMutator(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, MutatingWebhook webhook)
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
    public virtual int Priority => this.Webhook.Spec.Priority ?? int.MaxValue;

    /// <inheritdoc/>
    public virtual bool AppliesTo(ResourceOperation operation, string group, string version, string plural, string? @namespace = null) => this.Webhook.Spec.Resources?.Any(r => r.Matches(operation, group, version, plural, @namespace)) == true;

    /// <inheritdoc/>
    public virtual async Task MutateAsync(ResourceAdmissionReviewContext context, CancellationToken cancellationToken = default)
    {
        if(context == null) throw new ArgumentNullException(nameof(context));
        this.Logger.LogDebug("Mutating resource '{resource}' using webhook '{webhook}'...", context.Resource, this.Webhook);
        var admissionReview = context.ToAdmissionReview();
        using var response = await HttpClient.PostAsJsonAsync(this.Webhook.Spec.Client.Uri, admissionReview, cancellationToken);
        response.EnsureSuccessStatusCode();
        admissionReview = await response.Content.ReadFromJsonAsync<AdmissionReview>(cancellationToken: cancellationToken);
        if (admissionReview?.Response == null || admissionReview.Response.Patch == null)
        {
            this.Logger.LogWarning("Mutating webhook {webhook} responded with a success status code '{statusCode}' but did not return a valid mutating admission response or did not define a valid resource patch", this.Webhook, response.StatusCode);
            return;
        }
        context.Reviews.Add(admissionReview);
        if (!admissionReview.Response.Allowed)
        {
            this.Logger.LogWarning("Mutating webhook {webhook} failed to mutate resource '{resource}':\r\n{errors}", this.Webhook, context.Resource, admissionReview.Response?.Problem);
            return;
        }
        context.UpdatedState = Serializer.Json.Deserialize<IResource>(admissionReview.Response.Patch.ApplyTo(Serializer.Json.SerializeToNode(context.Resource)!.AsObject()!)!)!;
        this.Logger.LogDebug("Resource '{resource}' succesfully mutated using webhook '{webhook}'", context.Resource, this.Webhook);
    }

}
