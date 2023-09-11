namespace Hylo;

/// <summary>
/// Exposes constants used to describe the current application
/// </summary>
public static class ApplicationInfo
{

    /// <summary>
    /// Indicates whether or not the application is running in Docker
    /// </summary>
    public static readonly bool RunsInDocker = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));

    /// <summary>
    /// Indicates whether or not the application is running in Kubernetes
    /// </summary>
    public static readonly bool RunsInKubernetes = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST "));

}
