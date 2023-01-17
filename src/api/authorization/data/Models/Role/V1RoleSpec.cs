namespace Hylo.Api.Authorization.Data.Models
{
    /// <summary>
    /// Represents an object used to configure a <see cref="V1Role"/>
    /// </summary>
    [DataContract]
    public class V1RoleSpec
    {

        /// <summary>
        /// Initializes a new <see cref="V1RoleSpec"/>
        /// </summary>
        public V1RoleSpec() { }

        /// <summary>
        /// Initializes a new <see cref="V1RoleSpec"/>
        /// </summary>
        /// <param name="rules">A list containing the authorization rules that apply to the configured role</param>
        public V1RoleSpec(List<V1AuthorizationRule>? rules)
        {
            this.Rules = rules;
        }

        /// <summary>
        /// Gets/sets a list containing the authorization rules that apply to the configured role
        /// </summary>
        [DataMember(Name = "rules", Order = 1), JsonPropertyName("rules")]
        public virtual List<V1AuthorizationRule>? Rules { get; set; }

    }

}
