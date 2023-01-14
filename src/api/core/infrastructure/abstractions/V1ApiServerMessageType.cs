namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Enumerates all <see cref="V1ApiServerMessage"/> types
/// </summary>
public static class V1ApiServerMessageType
{

    /// <summary>
    /// Indicates a <see cref="V1ApiServerMessage"/> used to communicate a <see cref="V1ResourceEvent"/>
    /// </summary>
    public const string ResourceEvent = "resource-event";

}