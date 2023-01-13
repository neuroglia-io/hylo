namespace Hylo;

/// <summary>
/// Represents an <see cref="Attribute"/> used to describe an <see cref="IResource{TMetadata}"/> implementation
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ResourceAttribute
    : Attribute
{

    /// <summary>
    /// Initializes a new <see cref="ResourceAttribute"/>
    /// </summary>
    /// <param name="group">The API group the resource belongs to</param>
    /// <param name="apiVersion">The version of the API the resource belongs to</param>
    /// <param name="kind">The resource's kind</param>
    /// <param name="pluralName">The resource's plural name</param>
    public ResourceAttribute(string group, string apiVersion, string kind, string pluralName)
    {
        this.Group = group;
        this.ApiVersion = apiVersion;
        this.Kind = kind;
        this.PluralName = pluralName;
    }

    /// <summary>
    /// Gets the API group the resource belongs to
    /// </summary>
    public  virtual string Group { get; }

    /// <summary>
    /// Gets the version of the API the resource belongs to
    /// </summary>
    public virtual string ApiVersion { get; }

    /// <summary>
    /// Gets the resource's kind
    /// </summary>
    public virtual string Kind { get; }

    /// <summary>
    /// Gets the resource's plural name
    /// </summary>
    public virtual string PluralName { get; }

}
