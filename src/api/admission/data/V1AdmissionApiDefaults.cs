namespace Hylo.Api;

/// <summary>
/// Exposes constants about the Hylo v1 Admission API
/// </summary>
public static class V1AdmissionApiDefaults
{

    /// <summary>
    /// Exposes constants about paging on the Hylo v1 Admission API
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
    /// Exposes constants about Hylo v1 Admission API resources
    /// </summary>
    public static class Resources
    {

        /// <summary>
        /// Gets the API group all Hylo v1 Admission resources belongs to
        /// </summary>
        public const string Group = "admission.hylo.cloud";
        /// <summary>
        /// Gets the version of the API all Hylo v1 Admission resources belongs to
        /// </summary>
        public const string Version = "v1";
        /// <summary>
        /// Gets the version of the API all Hylo v1 Admission resources belongs to
        /// </summary>
        public const string ApiVersion = Group + "/" + Version;

        /// <summary>
        /// Exposes built-in <see cref="V1ResourceDefinition"/>s
        /// </summary>
        public static class BuiltInDefinitions
        {

            /// <summary>
            /// Gets the definitions of <see cref="V1ResourceDefinition"/>s
            /// </summary>
            public static readonly V1ResourceDefinition MutatingWebhook = LoadDefinition(nameof(MutatingWebhook));
            /// <summary>
            /// Gets the definitions of <see cref="V1Namespace"/>s
            /// </summary>
            public static readonly V1ResourceDefinition ValidatingWebhook = LoadDefinition(nameof(ValidatingWebhook));

            /// <summary>
            /// Gets an <see cref="IEnumerable{T}"/> containing all built-in <see cref="V1ResourceDefinition"/>s
            /// </summary>
            /// <returns></returns>
            public static IEnumerable<V1ResourceDefinition> AsEnumerable()
            {
                yield return MutatingWebhook;
                yield return ValidatingWebhook;
            }

            static V1ResourceDefinition LoadDefinition(string name)
            {
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
                name = YamlDotNet.Serialization.NamingConventions.HyphenatedNamingConvention.Instance.Apply(name);
                using var stream = typeof(V1CoreApiDefaults).Assembly.GetManifestResourceStream($"{typeof(V1CoreApiDefaults).Namespace}.Admission.Data.Assets.Resources.Definitions.{name}.yaml");
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
