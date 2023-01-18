using System.Runtime.Serialization;

namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Represents the object used to configure a <see cref="ResourceCommand"/>
/// </summary>
[DataContract]
public class ResourceCommandOptions
{

    /// <summary>
    /// Gets/sets a boolean indicating whether or not to persist changes consequent to the <see cref="ResourceCommand"/>'s execution
    /// </summary>
    [DataMember(Name = "dryRun", Order = 1), JsonPropertyName("dryRun")]
    public virtual bool DryRun { get; set; }

}
