namespace Hylo.Api.Admission.Infrastructure.Services;

/// <summary>
/// Represents the context of a <see cref="V1AdmissionReview"/>
/// </summary>
public class ResourceAdmissionReviewContext
{

    /// <summary>
    /// Initializes a new <see cref="ResourceAdmissionReviewContext"/>
    /// </summary>
    /// <param name="operation">The operation to perform on the specified resource</param>
    /// <param name="resourceDefinition">The definition of the resource to validate</param>
    /// <param name="resourceReference">A reference to the resource to validate</param>
    /// <param name="resource">The resource to validate</param>
    public ResourceAdmissionReviewContext(string operation, V1ResourceDefinition resourceDefinition, V1ResourceReference resourceReference, object resource)
    {
        if (string.IsNullOrWhiteSpace(operation)) throw new ArgumentNullException(nameof(operation));
        if (resourceDefinition == null) throw new ArgumentNullException(nameof(resourceDefinition));
        if (resourceReference == null) throw new ArgumentNullException(nameof(resourceReference));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        this.Operation = operation;
        this.ResourceDefinition = resourceDefinition;
        this.ResourceReference = resourceReference;
        this.Resource = resource;
    }

    /// <summary>
    /// Gets the operation to perform on the specified resource
    /// </summary>
    public virtual string Operation { get; }

    /// <summary>
    /// Gets the definition of the resource to validate
    /// </summary>
    public virtual V1ResourceDefinition ResourceDefinition { get; }

    /// <summary>
    /// Gets a reference to the resource to validate
    /// </summary>
    public virtual V1ResourceReference ResourceReference { get; }

    /// <summary>
    /// Gets the resource to validate
    /// </summary>
    public virtual object Resource { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing all the <see cref="V1AdmissionReview"/>s that have been performed
    /// </summary>
    public virtual List<V1AdmissionReview> Reviews { get; } = new();

    /// <summary>
    /// Gets a boolean indicating whether or not the specified resource has been admitted
    /// </summary>
    public virtual bool Succeeded => this.Reviews.All(r => r.Response?.Succeeded == true);

}

/// <summary>
/// Represents the context of a <see cref="V1AdmissionReview"/>
/// </summary>
/// <typeparam name="TResource">The type of <see cref="V1Resource"/> to admit</typeparam>
public class ResourceAdmissionReviewContext<TResource>
    where TResource : V1Resource, new()
{

    /// <summary>
    /// Initializes a new <see cref="ResourceAdmissionReviewContext{TResource}"/>
    /// </summary>
    /// <param name="underlyingContext">The underlying <see cref="ResourceAdmissionReviewContext"/></param>
    public ResourceAdmissionReviewContext(ResourceAdmissionReviewContext underlyingContext)
    {
        if (underlyingContext == null) throw new ArgumentNullException(nameof(underlyingContext));
        this.UnderlyingContext = underlyingContext;
        this.Resource = Serializer.Json.Deserialize<TResource>(Serializer.Json.SerializeToNode(underlyingContext.Resource)!)!;
    }

    /// <summary>
    /// Gets the underlying <see cref="ResourceAdmissionReviewContext"/>
    /// </summary>
    protected ResourceAdmissionReviewContext UnderlyingContext { get; }

    /// <summary>
    /// Gets the operation to perform on the specified resource
    /// </summary>
    public virtual string Operation => this.UnderlyingContext.Operation;

    /// <summary>
    /// Gets the definition of the resource to validate
    /// </summary>
    public virtual V1ResourceDefinition ResourceDefinition => this.UnderlyingContext.ResourceDefinition;

    /// <summary>
    /// Gets a reference to the resource to validate
    /// </summary>
    public virtual V1ResourceReference ResourceReference => this.UnderlyingContext.ResourceReference;

    /// <summary>
    /// Gets the resource to validate
    /// </summary>
    public virtual TResource Resource { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing all the <see cref="V1AdmissionReview"/>s that have been performed
    /// </summary>
    public virtual List<V1AdmissionReview> Reviews => this.UnderlyingContext.Reviews;

    /// <summary>
    /// Gets a boolean indicating whether or not the specified resource has been admitted
    /// </summary>
    public virtual bool Succeeded => this.UnderlyingContext.Succeeded;

}