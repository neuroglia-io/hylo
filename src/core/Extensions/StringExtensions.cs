using YamlDotNet.Serialization.NamingConventions;

namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="string"/>s
/// </summary>
public static class StringExtensions
{

    /// <summary>
    /// Converts the specified input into a camel-cased string
    /// </summary>
    /// <param name="input">The string to convert to camel-case</param>
    /// <returns>The camel-cased input</returns>
    public static string ToCamelCase(this string input) => CamelCaseNamingConvention.Instance.Apply(input);

    /// <summary>
    /// Converts the specified input into a hyphen-cased string
    /// </summary>
    /// <param name="input">The string to convert to hyphen-case</param>
    /// <returns>The hyphen-cased input</returns>
    public static string ToHyphenCase(this string input) => HyphenatedNamingConvention.Instance.Apply(input);

}
