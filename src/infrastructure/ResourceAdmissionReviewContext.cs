namespace Hylo.Infrastructure;

/// <summary>
/// Represents the context of a <see cref="AdmissionReview"/>
/// </summary>
public class ResourceAdmissionReviewContext
{

    /// <summary>
    /// Initializes a new <see cref="ResourceAdmissionReviewContext"/>
    /// </summary>
    /// <param name="id">The context's unique identifier</param>
    /// <param name="operation">The operation to perform on the specified resource</param>
    /// <param name="resourceDefinition">The definition of the resource to evaluate the admission of</param>
    /// <param name="resource">A reference to the resource to evaluate the admission of</param>
    /// <param name="subResource">The sub resource the operation to review applies to, if any</param>
    /// <param name="updatedState">The updated state of the (sub)resource being admitted. Null if operation has been set to 'delete'</param>
    /// <param name="originalState">The original state of the (sub)resource being admitted. Null if operation has been set to 'create'</param>
    /// <param name="user">The information about the authenticated user that has performed the operation that is being admitted</param>
    /// <param name="dryRun">A boolean indicating whether or not to persist changed induced by the operation being admitted</param>
    public ResourceAdmissionReviewContext(string id, ResourceOperation operation, IResourceDefinition resourceDefinition, IResourceReference resource, string? subResource = null, IResource? updatedState = null, IResource? originalState = null, UserInfo? user = null, bool dryRun = false)
    {
        if(string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
        this.Id = id;
        this.Operation = operation;
        this.ResourceDefinition = resourceDefinition ?? throw new ArgumentNullException(nameof(resourceDefinition));
        this.Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        this.SubResource = subResource;
        this.UpdatedState = updatedState;
        this.OriginalState = originalState;
        this.User = user;
        this.DryRun = dryRun;
    }

    /// <summary>
    /// Gets the context's unique identifier
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the operation to perform on the specified resource
    /// </summary>
    public ResourceOperation Operation { get; }

    /// <summary>
    /// Gets the definition of the resource to evaluate the admission of
    /// </summary>
    public IResourceDefinition ResourceDefinition { get; }

    /// <summary>
    /// Gets a reference to the resource to evaluate the admission of
    /// </summary>
    public IResourceReference Resource { get; }

    /// <summary>
    /// Gets the sub resource the operation to review applies to, if any
    /// </summary>
    public string? SubResource { get; }

    /// <summary>
    /// Gets/sets the updated state of the (sub)resource being admitted. Null if operation has been set to 'delete'
    /// </summary>
    public IResource? UpdatedState { get; set; }

    /// <summary>
    /// Gets the original state of the (sub)resource being admitted. Null if operation has been set to 'create'
    /// </summary>
    public IResource? OriginalState { get; }

    /// <summary>
    /// Gets information about the authenticated user that has performed the operation that is being admitted
    /// </summary>
    public UserInfo? User { get; }

    /// <summary>
    /// Gets a boolean indicating whether or not to persist changed induced by the operation being admitted
    /// </summary>
    public bool DryRun { get; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing all the reviews that have been performed
    /// </summary>
    public List<AdmissionReview> Reviews { get; } = new();

    /// <summary>
    /// Gets a boolean indicating whether or not the operation being admitted is allowed
    /// </summary>
    public bool Allowed => this.Reviews.All(r => r.Response?.Allowed == true);

}
