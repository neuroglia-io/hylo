using System.Text.RegularExpressions;

namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="Guid"/>s
/// </summary>
public static class GuidExtensions
{

    /// <summary>
    /// Gets the <see cref="Guid"/>'s sanitized base-64 string representation
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> to convert to its short string representation</param>
    /// <returns>The <see cref="Guid"/>'s sanitized base-64 string representation</returns>
    public static string ToShortString(this Guid id) => Regex.Replace(Convert.ToBase64String(id.ToByteArray()), "[/+=]", string.Empty, RegexOptions.Compiled).ToLowerInvariant();

}
