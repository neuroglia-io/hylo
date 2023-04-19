namespace Hylo;

/// <summary>
/// Represents a resource version conversion response
/// </summary>
[DataContract]
public class ConversionReviewResponse
{

    /// <summary>
    /// Initializes a new <see cref="ConversionReviewResponse"/>
    /// </summary>
    public ConversionReviewResponse() { }

    /// <summary>
    /// Initializes a new <see cref="ConversionReviewResponse"/>
    /// </summary>
    /// <param name="succeeded">A boolean indicating whether or not the version conversion succeeded</param>
    /// <param name="convertedResource">The converted resource</param>
    /// <param name="errors">A code/messages mapping of the errors that have occured during conversion, if any</param>
    public ConversionReviewResponse(bool succeeded, IResource? convertedResource = null, IEnumerable<KeyValuePair<string, string[]>>? errors = null)
    {
        this.Succeeded = succeeded;
        this.ConvertedResource = convertedResource;
        this.Errors = errors;
    }

    /// <summary>
    /// Gets a boolean indicating whether or not the version conversion succeeded
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "succeeded", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("succeeded"), YamlMember(Order = 1, Alias = "succeeded")]
    public virtual bool Succeeded { get; set; }

    /// <summary>
    /// Gets the converted resource
    /// </summary>
    [DataMember(Order = 2, Name = "convertedResource"), JsonPropertyOrder(2), JsonPropertyName("convertedResource"), YamlMember(Order = 2, Alias = "convertedResource")]
    public virtual IResource? ConvertedResource { get; set; }

    /// <summary>
    /// Gets a code/messages mapping of the errors that have occured during conversion, if any
    /// </summary>
    [DataMember(Order = 3, Name = "errors"), JsonPropertyOrder(3), JsonPropertyName("errors"), YamlMember(Order = 3, Alias = "errors")]
    public virtual IEnumerable<KeyValuePair<string, string[]>>? Errors { get; set; }

}
