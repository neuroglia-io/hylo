namespace Hylo.Api.Core.Data.Models
{

    /// <summary>
    /// Represents a role that applies in a specific namespace
    /// </summary>
    [DataContract]
    public class V1Role
        : V1Resource<V1RoleSpec>
    {

        /// <summary>
        /// Gets the <see cref="V1Role"/> resource API version
        /// </summary>
        public const string ResourceApiVersion = V1CoreApiDefaults.Resources.ApiVersion;
        /// <summary>
        /// Gets the <see cref="V1Role"/> resource kind
        /// </summary>
        public const string ResourceKind = "Role";

        /// <summary>
        /// Initializes a new <see cref="V1Role"/>
        /// </summary>
        public V1Role() { }

        /// <summary>
        /// Initializes a new <see cref="V1Role"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1Role"/>'s <see cref="V1RoleSpec"/></param>
        public V1Role(V1ResourceMetadata metadata, V1RoleSpec spec)
            : this()
        {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
        }

    }

}
