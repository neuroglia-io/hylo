namespace Hylo;

/// <summary>
/// Exposes helpers to handle Hylo embedded resources
/// </summary>
public static class EmbeddedResources
{

    static readonly string Prefix = $"{typeof(EmbeddedResources).Namespace}.";

    /// <summary>
    /// Reads to end the stream of the specified embedded resource
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource to read to end</param>
    /// <returns>The specified embedded resource's content</returns>
    public static string ReadToEnd(string resourceName)
    {
        if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentNullException(nameof(resourceName));
        using var stream = typeof(EmbeddedResources).Assembly.GetManifestResourceStream(resourceName)!;
        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }

    /// <summary>
    /// Exposes Hylo embedded assets
    /// </summary>
    public static class Assets
    {

        static readonly string Prefix = $"{EmbeddedResources.Prefix}Assets.";

        /// <summary>
        /// Exposes Hylo embedded resource definitions
        /// </summary>
        public static class Definitions
        {

            static readonly string Prefix = $"{Assets.Prefix}Definitions.";

            /// <summary>
            /// Gets the definition of resource definitions
            /// </summary>
            public static readonly string ResourceDefinition = $"{Prefix}resource-definition.yaml";
            /// <summary>
            /// Gets the definition of mutating admission webhooks
            /// </summary>
            public static readonly string MutatingWebhook = $"{Prefix}mutating-webhook.yaml";
            /// <summary>
            /// Gets the definition of validating admission webhooks
            /// </summary>
            public static readonly string ValidatingWebhook = $"{Prefix}validating-webhook.yaml";

        }

    }

}