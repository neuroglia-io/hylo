namespace Hylo.Api.Core.Infrastructure;

/// <summary>
/// Enumerates all <see cref="ApiServerMessage"/> types
/// </summary>
public static class ApiServerMessageType
{

    /// <summary>
    /// Indicates a <see cref="ApiServerMessage"/> used to communicate a <see cref="V1ResourceEvent"/>
    /// </summary>
    public const string ResourceEvent = "resource-event";

}