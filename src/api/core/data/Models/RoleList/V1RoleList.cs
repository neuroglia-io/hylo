namespace Hylo.Api.Core.Data.Models
{
    /// <summary>
    /// Represents a collection of <see cref="V1Role"/>s
    /// </summary>
    [DataContract]
    public class V1RoleList
        : V1Resource<V1RoleListSpec>
    {

        /// <summary>
        /// Gets the <see cref="V1RoleList"/> resource API version
        /// </summary>
        public const string ResourceApiVersion = V1CoreApiDefaults.Resources.ApiVersion;
        /// <summary>
        /// Gets the <see cref="V1RoleList"/> resource kind
        /// </summary>
        public const string ResourceKind = "RoleList";

        /// <summary>
        /// Initializes a new <see cref="V1RoleList"/>
        /// </summary>
        public V1RoleList() { }

        /// <summary>
        /// Initializes a new <see cref="V1RoleList"/>
        /// </summary>
        /// <param name="spec">The <see cref="V1RoleList"/>'s <see cref="V1RoleListSpec"/></param>
        public V1RoleList(V1ResourceMetadata metadata, V1RoleListSpec spec)
            : this()
        {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
        }

    }

}
