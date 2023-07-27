using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Hylo.Infrastructure;

/// <summary>
/// Represents an object used to describe a Nuget package
/// </summary>
[DataContract]
public record NuGetPackageIdentity
{

    /// <summary>
    /// Gets/sets the id of the described Nuget package
    /// </summary>
    [DataMember(Order = 1, Name = "id", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("id"), YamlMember(Order = 1, Alias = "id")]
    public virtual required string Id { get; set; }

    /// <summary>
    /// Gets/sets the version of the described Nuget package
    /// </summary>
    [DataMember(Order = 2, Name = "version", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("version"), YamlMember(Order = 2, Alias = "version")]
    public virtual required string Version { get; set; }

    /// <inheritdoc/>
    public override string ToString() => $"{this.Id}:{this.Version}";

}