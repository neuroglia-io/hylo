namespace Hylo;

/// <summary>
/// Exposes the default <see cref="ConversionStrategy"/> strategies 
/// </summary>
public static class ConversionStrategy
{

    /// <summary>
    /// Gets the 'none' strategy, which does not perform any conversion
    /// </summary>
    public const string None = "none";

    /// <summary>
    /// Gets the 'webhook' strategy, which relies on calls to external services to convert <see cref="IResource"/>s
    /// </summary>
    public const string Webhook = "webhook";

}