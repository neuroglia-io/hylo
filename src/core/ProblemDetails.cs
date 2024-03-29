﻿using Hylo.Properties;
using System.Net;

namespace Hylo;

/// <summary>
/// Represents an object used to describe a problem, as defined by <see href="https://www.rfc-editor.org/rfc/rfc7807">RFC 7807</see>
/// </summary>
[DataContract]
public record ProblemDetails
    : IExtensible
{

    /// <summary>
    /// Initialize a new <see cref="ProblemDetails"/>
    /// </summary>
    public ProblemDetails() { }

    /// <summary>
    /// Initialize a new <see cref="ProblemDetails"/>
    /// </summary>
    /// <param name="type">An uri that reference the type of the described problem</param>
    /// <param name="title">A short, human-readable summary of the problem type.It SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization</param>
    /// <param name="status">The status code produced by the described problem</param>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem</param>
    /// <param name="instance">A <see cref="Uri"/> reference that identifies the specific occurrence of the problem.It may or may not yield further information if dereferenced</param>
    /// <param name="errors">An optional collection containing error messages mapped per error code</param>
    /// <param name="extensionData">A mapping containing problem details extension data, if any</param>
    public ProblemDetails(Uri type, string title, int status, string? detail = null, Uri? instance = null, IEnumerable<KeyValuePair<string, string[]>>? errors = null, IDictionary<string, object>? extensionData = null)
    {
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.Title = title ?? throw new ArgumentNullException(nameof(title));
        this.Status = status;
        this.Detail = detail;
        this.Instance = instance;
        this.Errors = errors?.WithValueSemantics();
        this.ExtensionData = extensionData;
    }

    /// <summary>
    /// Gets/sets an uri that reference the type of the described problem.
    /// </summary>
    [DataMember(Order = 1, Name = "type"), JsonPropertyName("type"), YamlMember(Alias = "type")]
    public virtual Uri? Type { get; set; } = null!;

    /// <summary>
    /// Gets/sets a short, human-readable summary of the problem type.It SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization.
    /// </summary>
    [DataMember(Order = 2, Name = "title"), JsonPropertyName("title"), YamlMember(Alias = "title")]
    public virtual string? Title { get; set; } = null!;

    /// <summary>
    /// Gets/sets the status code produced by the described problem
    /// </summary>
    [DataMember(Order = 3, Name = "status"), JsonPropertyName("status"), YamlMember(Alias = "status")]
    public virtual int Status { get; set; }

    /// <summary>
    /// Gets/sets a human-readable explanation specific to this occurrence of the problem.
    /// </summary>
    [DataMember(Order = 4, Name = "detail"), JsonPropertyName("detail"), YamlMember(Alias = "detail")]
    public virtual string? Detail { get; set; }

    /// <summary>
    /// Gets/sets a <see cref="Uri"/> reference that identifies the specific occurrence of the problem.It may or may not yield further information if dereferenced.
    /// </summary>
    [DataMember(Order = 5, Name = "instance"), JsonPropertyName("instance"), YamlMember(Alias = "instance")]
    public virtual Uri? Instance { get; set; }

    /// <summary>
    /// Gets/sets an optional collection containing error messages mapped per error code
    /// </summary>
    [DataMember(Order = 6, Name = "errors"), JsonPropertyName("errors"), YamlMember(Alias = "errors")]
    public virtual EquatableList<KeyValuePair<string, string[]>>? Errors { get; set; }

    /// <summary>
    /// Gets/sets a mapping containing problem details extension data, if any
    /// </summary>
    [DataMember(Order = 7, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

    /// <inheritdoc/>
    public override string ToString() => this.Detail ?? Environment.NewLine + string.Join(Environment.NewLine, this.Errors?.Select(e => string.Join(Environment.NewLine, e.Value.Select(v => $"{e.Key}: {v}")))!);

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to validate a resource against its schema
    /// </summary>
    /// <param name="resource">A reference to the invalid resource</param>
    /// <param name="evaluationResults">The <see cref="EvaluationResults"/> to create new <see cref="ProblemDetails"/> for</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceSchemaValidationFailed(IResourceReference resource, EvaluationResults evaluationResults)
    {
        return new
        (
            ProblemTypes.Resources.SchemaValidationFailed, 
            ProblemTitles.ValidationFailed, 
            (int)HttpStatusCode.BadRequest,
            StringExtensions.Format(ProblemDescriptions.ResourceSchemaValidationFailed, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural),
            null,
            evaluationResults.Errors?.Select(e => new KeyValuePair<string, string[]>(e.Key, new[] { e.Value }))
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to find a specific resource
    /// </summary>
    /// <param name="resource">The resource that could not be found</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceNotFound(IResourceReference resource)
    {
        return new
        (
            ProblemTypes.Resources.NotFound,
            ProblemTitles.NotFound,
            (int)HttpStatusCode.NotFound,
            StringExtensions.Format(ProblemDescriptions.ResourceNotFound, resource)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error where a patch had no effect 
    /// </summary>
    /// <param name="resource">The resource that was not modified</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceNotModified(IResourceReference resource)
    {
        return new
        (
            ProblemTypes.Resources.NotModified,
            ProblemTitles.NotModified,
            (int)HttpStatusCode.NotModified,
            StringExtensions.Format(ProblemDescriptions.ResourceNotModified, resource)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to an unsupported sub resource 
    /// </summary>
    /// <param name="subResource">The unsupported sub resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails UnsupportedSubResource(ISubResourceReference subResource)
    {
        return new
        (
            ProblemTypes.Resources.UnsupportedSubResource,
            ProblemTitles.Unsupported,
            (int)HttpStatusCode.BadRequest,
            StringExtensions.Format(ProblemDescriptions.UnsupportedSubResource, subResource)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to allow an operation on a resource
    /// </summary>
    /// <param name="operation">The operation that was rejected</param>
    /// <param name="resource">The resource that could not be admitted</param>
    /// <param name="errors">An array containing the errors that have occured during admission</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceAdmissionFailed(Operation operation, IResourceReference resource, params KeyValuePair<string, string[]>[] errors)
    {
        return new
        (
            ProblemTypes.Resources.AdmissionFailed,
            ProblemTitles.AdmissionFailed,
            (int)HttpStatusCode.NotFound,
            StringExtensions.Format(ProblemDescriptions.ResourceAdmissionFailed, EnumHelper.Stringify(operation), resource, errors == null ? string.Empty : string.Join(Environment.NewLine, errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}")))
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to convert a resource to the specified version
    /// </summary>
    /// <param name="resource">The resource that could not be converted</param>
    /// <param name="toVersion">The version the resource was converted to</param>
    /// <param name="errors">An array containing the errors that have occured during conversion</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceConversionFailed(IResourceReference resource, string toVersion, params KeyValuePair<string, string[]>[] errors)
    {
        return new
        (
            ProblemTypes.Resources.ConversionFailed,
            ProblemTitles.ConversionFailed,
            (int)HttpStatusCode.BadRequest,
            StringExtensions.Format(ProblemDescriptions.ResourceConversionFailed, resource, toVersion, string.Join(Environment.NewLine, errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}")))
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to find a specific resource definition
    /// </summary>
    /// <param name="resource">The invalid resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceDefinitionNotFound(IResourceDefinitionReference resource)
    {
        return new
       (
           ProblemTypes.Resources.DefinitionNotFound,
           ProblemTitles.NotFound,
           (int)HttpStatusCode.NotFound,
           StringExtensions.Format(ProblemDescriptions.ResourceDefinitionNotFound, resource.Group, resource.Version, resource.Plural)
       );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to find a specific version of a resource definition
    /// </summary>
    /// <param name="resource">The invalid resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceVersionNotFound(IResourceReference resource)
    {
        return new
        (
            ProblemTypes.Resources.VersionNotFound,
            ProblemTitles.NotFound,
            (int)HttpStatusCode.NotFound,
            StringExtensions.Format(ProblemDescriptions.ResourceVersionNotFound, resource.Definition.Version, resource.Definition.Plural, resource.Definition.Group)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to find a specific version of a resource definition
    /// </summary>
    /// <param name="resourceDefinition">The invalid resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceStorageVersionNotFound(IResourceDefinitionReference resourceDefinition)
    {
        return new
        (
            ProblemTypes.Resources.StorageVersionNotFound,
            ProblemTitles.NotFound,
            (int)HttpStatusCode.NotFound,
            StringExtensions.Format(ProblemDescriptions.ResourceStorageVersionNotFound, resourceDefinition.Plural, resourceDefinition.Group)
        );
    }
    
    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to the fact that the '.metadata.resourceVersion' has not been set in the context of a replace operation
    /// </summary>
    /// <param name="resourceReference">A reference to the resource that could not be replaced</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceVersionRequired(IResourceReference resourceReference)
    {
        return new
        (
            ProblemTypes.Resources.ResourceVersionRequired,
            ProblemTitles.ValidationFailed,
            (int)HttpStatusCode.BadRequest,
            StringExtensions.Format(ProblemDescriptions.ResourceVersionRequired, resourceReference)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to the fact that the '.metadata.resourceVersion' has not been set in the context of a sub resource replace operation
    /// </summary>
    /// <param name="subResourceReference">A reference to the sub resource that could not be replaced</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails SubResourceVersionRequired(ISubResourceReference subResourceReference)
    {
        return new
        (
            ProblemTypes.Resources.ResourceVersionRequired,
            ProblemTitles.ValidationFailed,
            (int)HttpStatusCode.BadRequest,
            StringExtensions.Format(ProblemDescriptions.ResourceVersionRequired, subResourceReference)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to an invalid resource patch
    /// </summary>
    /// <param name="resourceReference">The <see cref="IResource"/> that could not be patched</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails InvalidResourcePatch(IResourceReference resourceReference)
    {
        return new
        (
            ProblemTypes.Resources.InvalidPatch,
            ProblemTitles.ValidationFailed,
            (int)HttpStatusCode.BadRequest,
            StringExtensions.Format(ProblemDescriptions.InvalidResourcePatch, resourceReference)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes an error due to an invalid sub resource patch
    /// </summary>
    /// <param name="subResourceReference">A reference to the sub resource that could not be patched</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails InvalidSubResourcePatch(IResourceReference subResourceReference)
    {
        return new
        (
            ProblemTypes.Resources.InvalidPatch,
            ProblemTitles.ValidationFailed,
            (int)HttpStatusCode.BadRequest,
            StringExtensions.Format(ProblemDescriptions.InvalidResourcePatch, subResourceReference)
        );
    }

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> that describes failure to perform optimistic concurrency checks on the specified resource
    /// </summary>
    /// <param name="resource">The invalid resource</param>
    /// <param name="targetVersion">The target version of the resource</param>
    /// <param name="actualVersion">The current version of the resource</param>
    /// <returns>A new <see cref="ProblemDetails"/></returns>
    public static ProblemDetails ResourceOptimisticConcurrencyCheckFailed(IResourceReference resource, string targetVersion, string actualVersion)
    {
        return new
        (
            ProblemTypes.Resources.OptimisticConcurrencyCheckFailed,
            ProblemTitles.Conflict,
            (int)HttpStatusCode.Conflict,
            StringExtensions.Format(ProblemDescriptions.ResourceOptimisticConcurrencyCheckFailed, resource, targetVersion, actualVersion)
        );
    }

}
