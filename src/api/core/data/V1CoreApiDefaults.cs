namespace Hylo.Api;

/// <summary>
/// Exposes constants about the Hylo v1 Core API
/// </summary>
public static class V1CoreApiDefaults
{

    /// <summary>
    /// Exposes constants about paging on the Hylo v1 Core API
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
    /// Exposes constants about Hylo v1 Core API resources
    /// </summary>
    public static class Resources
    {

        /// <summary>
        /// Gets the API group all Hylo v1 Core resources belongs to
        /// </summary>
        public const string Group = "core.hylo.cloud";
        /// <summary>
        /// Gets the version of the API all Hylo v1 Core resources belongs to
        /// </summary>
        public const string Version = "v1";
        /// <summary>
        /// Gets the version of the API all Hylo v1 Core resources belongs to
        /// </summary>
        public const string ApiVersion = Group + "/" + Version;

    }

}

