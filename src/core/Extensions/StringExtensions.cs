﻿using System.Text.RegularExpressions;
using YamlDotNet.Serialization.NamingConventions;

namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="string"/>s
/// </summary>
public static partial class StringExtensions
{

    private static readonly Regex MatchCurlyBracedWords = MatchCurlyBracedWordsRegex();

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

    /// <summary>
    /// Converts the specified input into a pascal-cased string
    /// </summary>
    /// <param name="input">The string to convert to pascal-case</param>
    /// <returns>The pascal-cased input</returns>
    public static string ToPascalCase(this string input) => PascalCaseNamingConvention.Instance.Apply(input);

    /// <summary>
    /// Determines whether or not the specified input only contains letters
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <returns>A boolean indicating whether or not the specified input only contains letters</returns>
    public static bool IsAlphabetic(this string input) => input.All(char.IsLetter);

    /// <summary>
    /// Determines whether or not the specified input only contains digits
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <returns>A boolean indicating whether or not the specified input only contains digits</returns>
    public static bool IsNumeric(this string input) => input.All(char.IsDigit);

    /// <summary>
    /// Determines whether or not the specified input only contains letters or digits
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <param name="exceptions">An array containing all exceptions allowed</param>
    /// <returns>A boolean indicating whether or not the specified input only contains letters or digits</returns>
    public static bool IsAlphanumeric(this string input, params char[] exceptions) => input.All(c => char.IsLetterOrDigit(c) || (exceptions != null && exceptions.Contains(c)));

    /// <summary>
    /// Determines whether or not the specified input is lowercased
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <returns>A boolean indicating whether or not the specified input is lowercased</returns>
    public static bool IsLowercased(this string input) => input.Where(char.IsLetter).All(char.IsLower);

    /// <summary>
    /// Determines whether or not the specified input is uppercased
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <returns>A boolean indicating whether or not the specified input is uppercased</returns>
    public static bool IsUppercased(this string input) => input.All(char.IsLower);

    /// <summary>
    /// Formats the string
    /// </summary>
    /// <param name="text">The string to format</param>
    /// <param name="args">The arguments to format the string with</param>
    /// <remarks>Accepts named arguments, which will be replaced in sequence by the specified values</remarks>
    /// <returns>The resulting string</returns>
    public static string Format(this string text, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        var formattedText = text;
        var matches = MatchCurlyBracedWords.Matches(text)
            .Select(m => m.Value)
            .Distinct()
            .ToList();
        for (int i = 0; i < matches.Count && i < args.Length; i++)
        {
            formattedText = formattedText.Replace(matches[i], args[i].ToString());
        }
        return formattedText;
    }

    /// <summary>
    /// Joins the values of the <see cref="IEnumerable{T}"/> with the specified character
    /// </summary>
    /// <param name="values">The values to join</param>
    /// <param name="separator">The separator char</param>
    /// <returns>A new string that consists of the joined values, separated by the specified char</returns>
    public static string Join(this IEnumerable<string> values, char separator) => string.Join(separator, values);

    /// <summary>
    /// Joins the values of the <see cref="IEnumerable{T}"/> with the specified string
    /// </summary>
    /// <param name="values">The values to join</param>
    /// <param name="separator">The separator string</param>
    /// <returns>A new string that consists of the joined values, separated by the specified string</returns>
    public static string Join(this IEnumerable<string> values, string separator) => string.Join(separator, values);

    /// <summary>
    /// Determines whether or not the string is a runtime expression
    /// </summary>
    /// <param name="input">The string to check</param>
    /// <returns>A boolean indicating whether or not the string is a runtime expression</returns>
    public static bool IsRuntimeExpression(this string input) => input.TrimStart().StartsWith("${") && input.TrimEnd().EndsWith("}");
    [GeneratedRegex("\\{([^}]+)\\}", RegexOptions.Compiled)]
    private static partial Regex MatchCurlyBracedWordsRegex();
}
