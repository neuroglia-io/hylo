namespace Hylo.Api.Core.Data.Models
{
    /// <summary>
    /// Represents a collection of <see cref="V1ClusterRole"/>s
    /// </summary>
    [DataContract]
    public class V1ClusterRoleList
        : V1Resource<V1ClusterRoleListSpec>
    {

        /// <summary>
        /// Gets the <see cref="V1RoleList"/> resource API version
        /// </summary>
        public const string ResourceApiVersion = V1CoreApiDefaults.Resources.ApiVersion;
        /// <summary>
        /// Gets the <see cref="V1RoleList"/> resource kind
        /// </summary>
        public const string ResourceKind = "ClusterRoleList";

        /// <summary>
        /// Initializes a new <see cref="V1ClusterRoleList"/>
        /// </summary>
        public V1ClusterRoleList() { }

        /// <summary>
        /// Initializes a new <see cref="V1ClusterRoleList"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1ClusterRoleList"/>'s <see cref="V1ClusterRoleListSpec"/></param>
        public V1ClusterRoleList(V1ResourceMetadata metadata, V1ClusterRoleListSpec spec)
            : this()
        {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
        }

    }

}
