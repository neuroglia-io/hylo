namespace Hylo.Api.Core.Data.Models
{

    /// <summary>
    /// Represents a role that applies in a specific namespace
    /// </summary>
    [Resource(HyloGroup, HyloApiVersion, HyloKind, HyloPluralName), DataContract]
    public class V1Role
        : V1Resource<V1RoleSpec>
    {

        /// <summary>
        /// Gets the resource API group
        /// </summary>
        public const string HyloGroup = V1CoreApiDefaults.Resources.ApiVersion;
        /// <summary>
        /// Gets the resource API version
        /// </summary>
        public const string HyloApiVersion = V1CoreApiDefaults.Resources.ApiVersion;
        /// <summary>
        /// Gets the resource kind
        /// </summary>
        public const string HyloKind = "Role";
        /// <summary>
        /// Gets the resource plural name
        /// </summary>
        public const string HyloPluralName = "roles";

        /// <summary>
        /// Initializes a new <see cref="V1Role"/>
        /// </summary>
        public V1Role() : base(HyloGroup, HyloApiVersion, HyloKind) { }

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
