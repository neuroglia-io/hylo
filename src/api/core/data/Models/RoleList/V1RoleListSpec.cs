namespace Hylo.Api.Core.Data.Models
{
    /// <summary>
    /// Represents an object used to configure a <see cref="V1RoleList"/>
    /// </summary>
    [DataContract]
    public class V1RoleListSpec
    {

        /// <summary>
        /// Gets/sets a list containing the roles that belong to the configured role list
        /// </summary>
        [DataMember(Name = "roles", Order = 1, IsRequired = true), JsonPropertyName("roles"), Required, MinLength(1)]
        public virtual List<V1Role> Roles { get; set; } = null!;

    }

}
