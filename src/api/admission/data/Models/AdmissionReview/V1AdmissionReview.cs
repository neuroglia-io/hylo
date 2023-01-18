namespace Hylo.Api.Admission.Data.Models;

/// <summary>
/// Represents a <see cref="V1Resource"/> admission request
/// </summary>
[DataContract]
public class V1AdmissionReview
    : IResource
{

    /// <summary>
    /// Gets the resource API group
    /// </summary>
    public const string HyloGroup = V1AdmissionApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the resource API version
    /// </summary>
    public const string HyloApiVersion = V1AdmissionApiDefaults.Resources.ApiVersion;
    /// <summary>
    /// Gets the resource kind
    /// </summary>
    public const string HyloKind = "AdmissionReview";

    /// <summary>
    /// Initializes a new <see cref="V1AdmissionReview"/>
    /// </summary>
    public V1AdmissionReview() { }

    /// <summary>
    /// Initializes a new <see cref="V1AdmissionReview"/>
    /// </summary>
    /// <param name="request">The resource admission review request to perform</param>
    public V1AdmissionReview(V1AdmissionReviewRequest request)
    {
        if(request == null) throw new ArgumentNullException(nameof(request));
        this.Request = request;
    }

    /// <summary>
    /// Initializes a new <see cref="V1AdmissionReview"/>
    /// </summary>
    /// <param name="response">The response to a resource admission review request</param>
    public V1AdmissionReview(V1AdmissionReviewResponse response)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));
        this.Response = response;
    }

    /// <inheritdoc/>
    [DataMember(Name = "apiVersion", Order = 1), JsonPropertyName("apiVersion"), Required, MinLength(1)]
    public virtual string ApiVersion { get; } = Api.ApiVersion.Build(HyloGroup, HyloApiVersion);

    /// <inheritdoc/>
    [DataMember(Name = "kind", Order = 2), JsonPropertyName("kind"), Required, MinLength(1)]
    public virtual string Kind { get; } = HyloKind;

    /// <summary>
    /// Gets/sets the resource admission review request to perform
    /// </summary>
    [DataMember(Name = "request", Order = 3), JsonPropertyName("request")]
    public virtual V1AdmissionReviewRequest? Request { get; set; }

    /// <summary>
    /// Gets/sets the response to a resource admission review request
    /// </summary>
    [DataMember(Name = "response", Order = 4), JsonPropertyName("response")]
    public virtual V1AdmissionReviewResponse? Response { get; set; }

}
