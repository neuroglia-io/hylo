namespace Hylo.Api.Core.Data.Models
{
    /// <summary>
    /// Represents a role that applies cluster-wide
    /// </summary>
    [DataContract]
    public class V1ClusterRole
        : V1Resource<V1RoleSpec>
    {

        /// <summary>
        /// Gets the <see cref="V1ClusterRole"/> resource API version
        /// </summary>
        public const string ResourceApiVersion = V1CoreApiDefaults.Resources.ApiVersion;
        /// <summary>
        /// Gets the <see cref="V1ClusterRole"/> resource kind
        /// </summary>
        public const string ResourceKind = "ClusterRole";

        /// <summary>
        /// Initializes a new <see cref="V1ClusterRole"/>
        /// </summary>
        public V1ClusterRole() { }

        /// <summary>
        /// Initializes a new <see cref="V1ClusterRole"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1ClusterRole"/>'s <see cref="V1RoleSpec"/></param>
        public V1ClusterRole(V1ResourceMetadata metadata, V1RoleSpec spec)
            : this()
        {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
        }

    }

}
