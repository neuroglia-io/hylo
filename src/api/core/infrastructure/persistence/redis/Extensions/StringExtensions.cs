namespace Hylo.Api.Core.Infrastructure.Services;

/// <summary>
/// Defines extensions for <see cref="string"/>s
/// </summary>
public static class StringExtensions
{

    /// <summary>
    /// Computes the score of the specified input
    /// </summary>
    /// <param name="input">The input to get the score of</param>
    /// <returns>The score of the specified input</returns>
    public static double ComputeSortedSetScore(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return double.MaxValue;
        var score = 0;
        foreach (var c in input)
        {
            score += (int)c;
        }
        return score;
    }

}