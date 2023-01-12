namespace Hylo.Api.Core.Data.Models
{
    /// <summary>
    /// Represents an authorization rule
    /// </summary>
    [DataContract]
    public class V1AuthorizationRule
    {

        /// <summary>
        /// Gets/sets a list containing all the API groups the role grants access to
        /// </summary>
        [DataMember(Name = "apiGroups", Order = 1), JsonPropertyName("apiGroups")]
        public virtual List<string>? ApiGroups { get; set; }

        /// <summary>
        /// Gets/sets a list containing all the resources the role grants access to. You can use a single '*' string to authorize the role for all resources
        /// </summary>
        [DataMember(Name = "resources", Order = 2), JsonPropertyName("resources")]
        public virtual List<string>? Resources { get; set; }

        /// <summary>
        /// Gets/sets a list containing all the resources the role grants access to. You can use a single '*' string to authorize the role for all verbs
        /// </summary>
        [DataMember(Name = "verbs", Order = 3), JsonPropertyName("verbs")]
        public virtual List<string>? Verbs { get; set; }

    }

}
