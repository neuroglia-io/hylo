namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Defines the fundamentals of an <see cref="ICommand"/> used to perform an operation on a <see cref="V1Resource"/>
/// </summary>
public abstract class ResourceCommand
    : V1ApiCommand<object>
{

    /// <summary>
    /// Initializes a new <see cref="ResourceCommand"/>
    /// </summary>
    protected ResourceCommand() { }

    /// <summary>
    /// Initializes a new <see cref="ResourceCommand"/>
    /// </summary>
    /// <param name="group">The API group the <see cref="V1Resource"/> belongs to</param>
    /// <param name="version">The version of the API the <see cref="V1Resource"/> belongs to</param>
    /// <param name="plural">The plural name of the type of the <see cref="V1Resource"/> to operate on</param>
    /// <param name="resource">The state of the resource to operate on</param>
    /// <param name="commandOptions">An object used to configure the <see cref="ResourceCommand"/></param>
    protected ResourceCommand(string group, string version, string plural, object resource, ResourceCommandOptions? commandOptions = null)
    {
        if (string.IsNullOrEmpty(group)) throw new ArgumentNullException(nameof(group));
        if (string.IsNullOrEmpty(version)) throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrEmpty(plural)) throw new ArgumentNullException(nameof(plural));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        this.Group = group;
        this.Version = version;
        this.Plural = plural;
        this.Resource = resource;
        this.CommandOptions = commandOptions;
    }

    /// <summary>
    /// Gets the <see cref="V1RuleWithOperation"/> to perform
    /// </summary>
    public abstract string Operation { get; }

    /// <summary>
    /// Gets the API group the <see cref="V1Resource"/> belongs to
    /// </summary>
    [FromRoute, JsonPropertyName("group"), Required]
    public string Group { get; set; } = null!;

    /// <summary>
    /// Gets the version of the API the <see cref="V1Resource"/> belongs to
    /// </summary>
    [FromRoute, JsonPropertyName("version"), Required]
    public string Version { get; set; } = null!;

    /// <summary>
    /// Gets the plural name of the type of the <see cref="V1Resource"/> to operate on
    /// </summary>
    [FromRoute, JsonPropertyName("plural"), Required]
    public string Plural { get; set; } = null!;

    /// <summary>
    /// Gets state of the resource to operate on
    /// </summary>
    [FromBody, JsonPropertyName("resource"), Required]
    public object Resource { get; set; } = null!;

    /// <summary>
    /// Gets an object used to configure the <see cref="ResourceCommand"/>
    /// </summary>
    [FromQuery, JsonPropertyName("commandOptions")]
    public ResourceCommandOptions? CommandOptions { get; set; }

    /// <summary>
    /// Gets a <see cref="V1ResourceReference"/> to the resource to operate on
    /// </summary>
    /// <returns>A new <see cref="V1ResourceReference"/></returns>
    public abstract V1ResourceReference GetResourceReference();

}