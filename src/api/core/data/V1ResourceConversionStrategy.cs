using Hylo.Api.Core.Data.Models;

namespace Hylo.Api.Data.Core;

/// <summary>
/// Exposes the default <see cref="V1ResourceConversion"/> strategies 
/// </summary>
public static class V1ResourceConversionStrategy
{

    /// <summary>
    /// Gets the 'none' strategy, which does not perform any conversion
    /// </summary>
    public const string None = "none";

    /// <summary>
    /// Gets the 'webhook' strategy, which relies on calls to external services to convert <see cref="V1Resource"/>s
    /// </summary>
    public const string Webhook = "webhook";

}
