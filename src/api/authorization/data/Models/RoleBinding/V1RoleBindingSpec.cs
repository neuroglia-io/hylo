namespace Hylo.Api.Authorization.Data.Models;

/// <summary>
/// Represents an object used to configure a <see cref="V1RoleBinding"/>
/// </summary>
[DataContract]
public class V1RoleBindingSpec
{

    /// <summary>
    /// Initializes a new <see cref="V1RoleBindingSpec"/>
    /// </summary>
    public V1RoleBindingSpec() { }

    /// <summary>
    /// Initializes a new <see cref="V1RoleBindingSpec"/>
    /// </summary>
    /// <param name="role">A reference to the role to bind the subjects to</param>
    /// <param name="subjects"></param>
    public V1RoleBindingSpec(V1ResourceReference role, List<V1ResourceReference> subjects)
    {
        this.Role = role;
        this.Subjects = subjects;
    }

    /// <summary>
    /// Gets/sets a reference of the role to bind the subjects to
    /// </summary>
    [DataMember(Name = "role", Order = 1), JsonPropertyName("role"), Required]
    public virtual V1ResourceReference Role { get; set; } = null!;

    /// <summary>
    /// Gets/sets a list containing references of the subjects to bind to the specified role
    /// </summary>
    [DataMember(Name = "subjects", Order = 1), JsonPropertyName("subjects"), Required, MinLength(1)]
    public virtual List<V1ResourceReference> Subjects { get; set; } = null!;

}
