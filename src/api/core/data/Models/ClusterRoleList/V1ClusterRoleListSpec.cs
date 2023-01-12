namespace Hylo.Api.Core.Data.Models
{

    /// <summary>
    /// Represents an object used to configure a <see cref="V1ClusterRoleList"/>
    /// </summary>
    [DataContract]
    public class V1ClusterRoleListSpec
    {

        /// <summary>
        /// Gets/sets a list containing the cluster roles that belong to the configured role list
        /// </summary>
        [DataMember(Name = "roles", Order = 1, IsRequired = true), JsonPropertyName("roles"), Required, MinLength(1)]
        public virtual List<V1ClusterRole> Roles { get; set; } = null!;

    }

}
