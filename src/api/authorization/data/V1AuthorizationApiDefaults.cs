using Hylo.Api.Authorization.Data.Models;

namespace Hylo.Api;

/// <summary>
/// Exposes constants about the Hylo v1 Authorization API
/// </summary>
public static class V1AuthorizationApiDefaults
{

    /// <summary>
    /// Exposes constants about paging on the Hylo v1 Authorization API
    /// </summary>
    public static class Paging
    {

        /// <summary>
        /// Indicates the minimum amount of results per page
        /// </summary>
        public const int MinResultsPerPage = 1;
        /// <summary>
        /// Indicates the maximum amount of result per page
        /// </summary>
        public const int MaxResultsPerPage = 100;

    }

    /// <summary>
    /// Exposes constants about Hylo v1 Authorization API resources
    /// </summary>
    public static class Resources
    {

        /// <summary>
        /// Gets the API group all Hylo v1 Authorization resources belongs to
        /// </summary>
        public const string Group = "authorization.hylo.cloud";
        /// <summary>
        /// Gets the version of the API all Hylo v1 Authorization resources belongs to
        /// </summary>
        public const string Version = "v1";
        /// <summary>
        /// Gets the version of the API all Hylo v1 Authorization resources belongs to
        /// </summary>
        public const string ApiVersion = Group + "/" + Version;

        /// <summary>
        /// Exposes built-in <see cref="V1ResourceDefinition"/>s
        /// </summary>
        public static class BuiltInDefinitions
        {

            /// <summary>
            /// Gets the definition of <see cref="V1ResourceDefinition"/>s
            /// </summary>
            public static readonly V1ResourceDefinition ClusterRole = LoadDefinition(nameof(ClusterRole));
            /// <summary>
            /// Gets the definition of <see cref="V1ClusterRoleBinding"/>s
            /// </summary>
            public static readonly V1ResourceDefinition ClusterRoleBinding = LoadDefinition(nameof(ClusterRoleBinding));
            /// <summary>
            /// Gets the definition of <see cref="V1Role"/>s
            /// </summary>
            public static readonly V1ResourceDefinition Role = LoadDefinition(nameof(Role));
            /// <summary>
            /// Gets the definition of <see cref="V1RoleBinding"/>s
            /// </summary>
            public static readonly V1ResourceDefinition RoleBinding = LoadDefinition(nameof(RoleBinding));
            /// <summary>
            /// Gets the definition of <see cref="V1UserAccount"/>s 
            /// </summary>
            public static readonly V1ResourceDefinition UserAccount = LoadDefinition(nameof(UserAccount));

            /// <summary>
            /// Gets an <see cref="IEnumerable{T}"/> containing all built-in <see cref="V1ResourceDefinition"/>s
            /// </summary>
            /// <returns></returns>
            public static IEnumerable<V1ResourceDefinition> AsEnumerable()
            {
                yield return ClusterRole;
                yield return ClusterRoleBinding;
                yield return Role;
                yield return RoleBinding;
                yield return UserAccount;
            }

            static V1ResourceDefinition LoadDefinition(string name)
            {
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
                name = YamlDotNet.Serialization.NamingConventions.HyphenatedNamingConvention.Instance.Apply(name);
                using var stream = typeof(V1CoreApiDefaults).Assembly.GetManifestResourceStream($"{typeof(V1CoreApiDefaults).Namespace}.Authorization.Data.Assets.Resources.Definitions.{name}.yaml");
                if (stream == null) throw new NullReferenceException();
                using var streamReader = new StreamReader(stream);
                var yaml = streamReader.ReadToEnd();
                var definition = Serializer.Yaml.Deserialize<V1ResourceDefinition>(yaml);
                if (definition == null) throw new NullReferenceException();
                return definition;
            }

        }

    }

}

