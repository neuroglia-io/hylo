using Hylo.Resources;

namespace Hylo;

/// <summary>
/// Represents a request to perform a review to determine whether or not an operation can be performed on a specific resource
/// </summary>
[DataContract]
public class AdmissionReviewRequest
{

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewRequest"/>
    /// </summary>
    public AdmissionReviewRequest() { }

    /// <summary>
    /// Initializes a new <see cref="AdmissionReviewRequest"/>
    /// </summary>
    /// <param name="uid">A string that uniquely and globally identifies the resource admission review request</param>
    /// <param name="operation">The operation to perform on the specified resource</param>
    /// <param name="resourceReference">A reference to the resource to perform the admission review for</param>
    /// <param name="subResourceReference">The sub resource the operation to review applies to, if any</param>
    /// <param name="actualState">The actual state of the (sub)resource being admitted. Null if operation has been set to 'delete'</param>
    /// <param name="originalState">The original state of the (sub)resource being admitted. Null if operation has been set to 'create'</param>
    /// <param name="user">The information about the authenticated user that has performed the operation that is being admitted</param>
    public AdmissionReviewRequest(string uid, ResourceOperation operation, ResourceReference resourceReference, string? subResourceReference = null, object? actualState = null, object? originalState = null, UserInfo? user = null)
    {
        if(string.IsNullOrWhiteSpace(uid)) throw new ArgumentNullException(nameof(uid));
        this.Uid = uid;
        this.Operation = operation;
        this.ResourceReference = resourceReference ?? throw new ArgumentNullException(nameof(resourceReference));
        this.SubResource = subResourceReference;
        this.ActualState = actualState;
        this.OriginalState = originalState;
        this.User = user;
    }

    /// <summary>
    /// Gets/sets a string that uniquely and globally identifies the resource admission review request
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "uid", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("uid"), YamlMember(Order = 1, Alias = "uid")]
    public virtual string Uid { get; set; } = null!;

    /// <summary>
    /// Gets/sets the operation to perform on the specified resource
    /// </summary>
    [Required]
    [DataMember(Order = 2, Name = "operation", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("operation"), YamlMember(Order = 2, Alias = "operation")]
    public virtual ResourceOperation Operation { get; set; }

    /// <summary>
    /// Gets/sets a reference to the resource to perform the admission review for
    /// </summary>
    [Required]
    [DataMember(Order = 3, Name = "resource", IsRequired = true), JsonPropertyOrder(3), JsonPropertyName("resource"), YamlMember(Order = 3, Alias = "resource")]
    public virtual ResourceReference ResourceReference { get; set; } = null!;

    /// <summary>
    /// Gets/sets the sub resource the operation to review applies to, if any
    /// </summary>
    [DataMember(Order = 4, Name = "subResource"), JsonPropertyOrder(4), JsonPropertyName("subResource"), YamlMember(Order = 4, Alias = "subResource")]
    public virtual string? SubResource { get; set; }

    /// <summary>
    /// Gets/sets the actual state of the (sub)resource being admitted. Null if operation has been set to 'delete'
    /// </summary>
    [DataMember(Order = 5, Name = "actualState"), JsonPropertyOrder(5), JsonPropertyName("actualState"), YamlMember(Order = 5, Alias = "actualState")]
    public virtual object? ActualState { get; set; }

    /// <summary>
    /// Gets/sets the original state of the (sub)resource being admitted. Null if operation has been set to 'create'
    /// </summary>
    [DataMember(Order = 6, Name = "originalState"), JsonPropertyOrder(6), JsonPropertyName("originalState"), YamlMember(Order = 6, Alias = "originalState")]
    public virtual object? OriginalState { get; set; }

    /// <summary>
    /// Gets/sets information about the authenticated user that has performed the operation that is being admitted
    /// </summary>
    [DataMember(Order = 7, Name = "user"), JsonPropertyOrder(7), JsonPropertyName("user"), YamlMember(Order = 7, Alias = "user")]
    public virtual UserInfo? User { get; set; }

}
