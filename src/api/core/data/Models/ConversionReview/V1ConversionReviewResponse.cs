namespace Hylo.Api.Core.Data.Models;

/// <summary>
/// Represents a <see cref="V1Resource"/> version conversion response
/// </summary>
public class V1ConversionReviewResponse
{

    /// <summary>
    /// Initializes a new <see cref="V1ConversionReviewResponse"/>
    /// </summary>
    public V1ConversionReviewResponse() { }

    /// <summary>
    /// Initializes a new <see cref="V1ConversionReviewResponse"/>
    /// </summary>
    /// <param name="succeeded">A boolean indicating whether or not the version conversion succeeded</param>
    /// <param name="convertedResource">The converted resource</param>
    /// <param name="errors">An <see cref="IEnumerable{T}"/> containing the <see cref="V1Error"/>s that have occured during conversion</param>
    public V1ConversionReviewResponse(bool succeeded, object? convertedResource = null, IEnumerable<V1Error>? errors = null)
    {
        this.Succeeded = succeeded;
        this.ConvertedResource = convertedResource;
        this.Errors = errors;
    }

    /// <summary>
    /// Gets a boolean indicating whether or not the version conversion succeeded
    /// </summary>
    public virtual bool Succeeded { get; set; }

    /// <summary>
    /// Gets the converted resource
    /// </summary>
    public virtual object? ConvertedResource { get; set; }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the <see cref="V1Error"/>s that have occured during conversion
    /// </summary>
    public virtual IEnumerable<V1Error>? Errors { get; set; }

}

