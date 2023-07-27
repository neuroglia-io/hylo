using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace Hylo.Infrastructure;

/// <summary>
/// Represents an object used to describe a plugin's package
/// </summary>
[DataContract]
public class PluginPackageMetadata
{

    /// <summary>
    /// Initializes a new <see cref="PluginPackageMetadata"/>
    /// </summary>
    public PluginPackageMetadata() { }

    /// <summary>
    /// Initializes a new <see cref="PluginPackageMetadata"/>
    /// </summary>
    /// <param name="identity">The id of the described plugin package</param>
    /// <param name="dependencies">A list containng the dependencies of the described plugin package</param>
    public PluginPackageMetadata(NuGetPackageIdentity identity, IEnumerable<NuGetPackageIdentity> dependencies)
    {
        this.Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        this.Dependencies = dependencies?.ToList() ?? throw new ArgumentNullException(nameof(dependencies));
    }

    /// <summary>
    /// Gets/sets the id of the described plugin package
    /// </summary>
    [DataMember(Order = 1, Name = "identity", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("identity"), YamlMember(Order = 1, Alias = "identity")]
    public virtual NuGetPackageIdentity Identity { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containng the dependencies of the described plugin package
    /// </summary>
    [DataMember(Order = 2, Name = "dependencies", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("dependencies"), YamlMember(Order = 2, Alias = "dependencies")]
    public virtual List<NuGetPackageIdentity> Dependencies { get; set; } = null!;

    /// <summary>
    /// Gets/sets a key/value mapping of the plugin metadata's extensions, if any
    /// </summary>
    [DataMember(Order = 99, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

}
