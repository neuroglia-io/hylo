﻿using System.Runtime.Serialization;
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
    /// Initializes a new <see cref="PluginMetadata"/>
    /// </summary>
    public PluginMetadata() { }

    /// <summary>
    /// Initializes a new <see cref="PluginMetadata"/>
    /// </summary>
    /// <param name="assemblyFilePath">The path to the plugin assembly file</param>
    /// <param name="typeName">The full name of the plugin type</param>
    /// <param name="contractTypeName">The full name of the plugin's contract type. If not set, should be resolved by reflection</param>
    /// <param name="bootstrapperTypeName">The name of the plugin's bootstrapper type, if any</param>
    /// <param name="sharedAssemblies">A list containing the assemblies shared between the configured plugin and its host</param>
    /// <param name="nugetPackage">A reference to the plugin's nuget package, if any. If set, the plugin manager should download and extract the plugin's package to the plugins directory before loading it</param>
    public PluginMetadata(string assemblyFilePath, string typeName, string? contractTypeName = null, string? bootstrapperTypeName = null, IEnumerable<string>? sharedAssemblies = null, string? nugetPackage = null)
    {
        if (string.IsNullOrWhiteSpace(assemblyFilePath)) throw new ArgumentNullException(nameof(assemblyFilePath));
        if (string.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException(nameof(typeName));
        this.AssemblyFilePath = assemblyFilePath;
        this.TypeName = typeName;
        this.ContractTypeName = contractTypeName;
        this.BootstrapperTypeName = bootstrapperTypeName;
        this.SharedAssemblies = sharedAssemblies?.ToList();
        this.NugetPackage = nugetPackage;
    }

    /// <summary>
    /// Gets the path to the plugin assembly file
    /// </summary>
    [DataMember(Order = 1, Name = "assemblyFilePath", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("assemblyFilePath"), YamlMember(Order = 1, Alias = "assemblyFilePath")]
    public virtual string AssemblyFilePath { get; set; } = null!;

    /// <summary>
    /// Gets the full name of the plugin type
    /// </summary>
    [DataMember(Order = 2, Name = "typeName", IsRequired = true), JsonPropertyOrder(2), JsonPropertyName("typeName"), YamlMember(Order = 2, Alias = "typeName")]
    public virtual string TypeName { get; set; } = null!;

    /// <summary>
    /// Gets the full name of the plugin's contract type
    /// </summary>
    [DataMember(Order = 3, Name = "contractTypeName"), JsonPropertyOrder(3), JsonPropertyName("contractTypeName"), YamlMember(Order = 3, Alias = "contractTypeName")]
    public virtual string? ContractTypeName { get; set; }

    /// <summary>
    /// Gets the name of the plugin's bootstrapper type, if any
    /// </summary>
    [DataMember(Order = 4, Name = "bootstrapperTypeName"), JsonPropertyOrder(4), JsonPropertyName("bootstrapperTypeName"), YamlMember(Order = 4, Alias = "bootstrapperTypeName")]
    public virtual string? BootstrapperTypeName { get; set; }

    /// <summary>
    /// Gets/sets a list containing the assemblies shared between the configured plugin and its host
    /// </summary>
    [DataMember(Order = 5, Name = "sharedAssemblies"), JsonPropertyOrder(5), JsonPropertyName("sharedAssemblies"), YamlMember(Order = 5, Alias = "sharedAssemblies")]
    public virtual List<string>? SharedAssemblies { get; set; }

    /// <summary>
    /// Gets/sets a reference to the plugin's nuget package, if any. If set, the plugin manager should download and extract the plugin's package to the plugins directory before loading it
    /// </summary>
    [DataMember(Order = 6, Name = "nugetPackage"), JsonPropertyOrder(6), JsonPropertyName("nugetPackage"), YamlMember(Order = 6, Alias = "nugetPackage")]
    public virtual string? NugetPackage { get; set; }

    /// <summary>
    /// Gets/sets a key/value mapping of the plugin metadata's extensions, if any
    /// </summary>
    [DataMember(Order = 99, Name = "extensionData"), JsonExtensionData]
    public virtual IDictionary<string, object>? ExtensionData { get; set; }

    /// <summary>
    /// Creates <see cref="PluginMetadata"/> from the specified type
    /// </summary>
    /// <param name="pluginType">The non-generic, non-abstract plugin class type</param>
    /// <returns>A new <see cref="PluginMetadata"/> used to describe the specified plugin type</returns>
    public static PluginMetadata FromType(Type pluginType)
    {
        if (pluginType == null) throw new ArgumentNullException(nameof(pluginType));
        if (!pluginType.IsClass || pluginType.IsInterface || pluginType.IsAbstract || pluginType.IsGenericType) throw new ArgumentException("The plugin type must be a non-abstract, non-generic class", nameof(pluginType));
        var pluginAttribute = pluginType.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType.FullName == typeof(PluginAttribute).FullName);
        var contractType = ((Type?)pluginAttribute?.ConstructorArguments[0].Value)?.FullName;
        var bootstrapperType = pluginAttribute?.ConstructorArguments.Count > 1 ? ((Type?)pluginAttribute?.ConstructorArguments[1].Value)?.FullName : null;
        return new(pluginType.Assembly.Location, pluginType.FullName!, contractType, bootstrapperType);
    }

}
