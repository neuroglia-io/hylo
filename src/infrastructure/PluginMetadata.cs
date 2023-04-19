using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Hylo.Infrastructure;

/// <summary>
/// Represents an object used to describe a plugin
/// </summary>
[DataContract]
public class PluginMetadata
{

    /// <summary>
    /// Gets the plugin's name
    /// </summary>
    [Required, MinLength(1)]
    [DataMember(Order = 1, Name = "name"), JsonPropertyOrder(1), JsonPropertyName("name"), YamlMember(Order = 1, Alias = "name")]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Gets the plugin's description
    /// </summary>
    [DataMember(Order = 2, Name = "description"), JsonPropertyOrder(2), JsonPropertyName("description"), YamlMember(Order = 2, Alias = "description")]
    public virtual string? Description { get; set; }

    /// <summary>
    /// Gets the plugin's authors
    /// </summary>
    [DataMember(Order = 3, Name = "authors"), JsonPropertyOrder(3), JsonPropertyName("authors"), YamlMember(Order = 3, Alias = "authors")]
    public virtual string? Authors { get; set; }

    /// <summary>
    /// Gets the plugin's copyright
    /// </summary>
    [DataMember(Order = 4, Name = "copyright"), JsonPropertyOrder(4), JsonPropertyName("copyright"), YamlMember(Order = 4, Alias = "copyright")]
    public virtual string? Copyright { get; set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> containing the plugin's tags
    /// </summary>
    [DataMember(Order = 5, Name = "tags"), JsonPropertyOrder(5), JsonPropertyName("tags"), YamlMember(Order = 5, Alias = "tags")]
    public virtual List<string>? Tags { get; set; }

    /// <summary>
    /// Gets the plugin's license file <see cref="Uri"/>
    /// </summary>
    [DataMember(Order = 6, Name = "licenseUri"), JsonPropertyOrder(6), JsonPropertyName("licenseUri"), YamlMember(Order = 6, Alias = "licenseUri")]
    public virtual Uri? LicenseUri { get; set; }

    /// <summary>
    /// Gets the plugin's readme file <see cref="Uri"/>
    /// </summary>
    [DataMember(Order = 7, Name = "readmeUri"), JsonPropertyOrder(7), JsonPropertyName("readmeUri"), YamlMember(Order = 7, Alias = "readmeUri")]
    public virtual Uri? ReadmeUri { get; set; }

    /// <summary>
    /// Gets the plugin's website <see cref="Uri"/>
    /// </summary>
    [DataMember(Order = 8, Name = "websiteUri"), JsonPropertyOrder(8), JsonPropertyName("websiteUri"), YamlMember(Order = 8, Alias = "websiteUri")]
    public virtual Uri? WebsiteUri { get; set; }

    /// <summary>
    /// Gets the plugin's repository <see cref="Uri"/>
    /// </summary>
    [DataMember(Order = 9, Name = "repositoryUri"), JsonPropertyOrder(9), JsonPropertyName("repositoryUri"), YamlMember(Order = 9, Alias = "repositoryUri")]
    public virtual Uri? RepositoryUri { get; set; }

    /// <summary>
    /// Gets the plugin's <see cref="Assembly"/> file name
    /// </summary>
    [Required, MinLength(1)]
    [DataMember(Order = 10, Name = "assemblyFile"), JsonPropertyOrder(10), JsonPropertyName("assemblyFile"), YamlMember(Order = 10, Alias = "assemblyFile")]
    public virtual string AssemblyFileName { get; set; } = null!;

    /// <inheritdoc/>
    public override string ToString() =>  this.Name;

}
