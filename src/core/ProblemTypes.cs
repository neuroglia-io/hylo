namespace Hylo;

/// <summary>
/// Exposes default Hylo problem types
/// </summary>
public static class ProblemTypes
{

    static readonly Uri BaseUri = new("https://hylo.io/docs/problems/");

    /// <summary>
    /// Exposes Hylo problem types related to resources
    /// </summary>
    public static class Resources
    {

        static readonly Uri BaseUri = new(ProblemTypes.BaseUri, "resources/");

        /// <summary>
        /// Gets the <see cref="Uri"/> that references a problem due to refusal of an operation on a specific resource
        /// </summary>
        public static readonly Uri AdmissionFailed = new(BaseUri, "admission-failed");
        /// <summary>
        /// Gets the <see cref="Uri"/> that references a failure to convert a resource to a specific version
        /// </summary>
        public static readonly Uri ConversionFailed = new(BaseUri, "conversion-failed");
        /// <summary>
        /// Gets the <see cref="Uri"/> that references a problem due to failing to validate a resource against its schema
        /// </summary>
        public static readonly Uri SchemaValidationFailed = new(BaseUri, "schema-validation-failed");
        /// <summary>
        /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a resource
        /// </summary>
        public static readonly Uri NotFound = new(BaseUri, "not-found");
        /// <summary>
        /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a resource definition
        /// </summary>
        public static readonly Uri DefinitionNotFound = new(BaseUri, "definition-not-found");
        /// <summary>
        /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a specific version of a resource definition
        /// </summary>
        public static readonly Uri VersionNotFound = new(BaseUri, "version-not-found");
        /// <summary>
        /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a the storage version of a resource definition
        /// </summary>
        public static readonly Uri StorageVersionNotFound = new(BaseUri, "storage-version-not-found");
    }

}