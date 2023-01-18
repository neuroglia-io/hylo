namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a <see cref="V1Resource"/> version conversion review
/// </summary>
[DataContract]
public class V1ConversionReview
    : IResource
{

    /// <summary>
    /// Gets the resource API group
    /// </summary>
    public const string HyloGroup = V1CoreApiDefaults.Resources.Group;
    /// <summary>
    /// Gets the resource API version
    /// </summary>
    public const string HyloApiVersion = V1CoreApiDefaults.Resources.Version;
    /// <summary>
    /// Gets the resource kind
    /// </summary>
    public const string HyloKind = "ConversionReview";

    /// <summary>
    /// Initializes a new <see cref="V1ConversionReview"/>
    /// </summary>
    public V1ConversionReview() { }

    /// <summary>
    /// Initializes a new <see cref="V1ResourceConversionReviewRequest"/>
    /// </summary>
    /// <param name="request">The <see cref="V1ResourceConversionReviewRequest"/>'s request</param>
    public V1ConversionReview(V1ConversionReviewRequest request)
    {
        this.Request = request;
    }

    /// <inheritdoc/>
    [DataMember(Name = "apiVersion", Order = 1), JsonPropertyName("apiVersion"), Required, MinLength(1)]
    public virtual string ApiVersion { get; } = Api.ApiVersion.Build(HyloGroup, HyloApiVersion);

    /// <inheritdoc/>
    [DataMember(Name = "kind", Order = 2), JsonPropertyName("kind"), Required, MinLength(1)]
    public virtual string Kind { get; } = HyloKind;

    /// <summary>
    /// Gets the <see cref="V1ResourceConversionReviewRequest"/>'s request
    /// </summary>
    [DataMember(Name = "request", Order = 3), JsonPropertyName("request")]
    public virtual V1ConversionReviewRequest Request { get; set; } = null!;

    /// <summary>
    /// Gets the <see cref="V1ResourceConversionReviewRequest"/>'s response
    /// </summary>
    [DataMember(Name = "response", Order = 4), JsonPropertyName("response")]
    public virtual V1ConversionReviewResponse? Response { get; set; }

}

