namespace Hylo;

/// <summary>
/// Represents an <see cref="IEqualityComparer{T}"/> used to compare objects and determine if they are equal based on JSON patches
/// </summary>
public class PatchBasedObjectComparer
    : IEqualityComparer<object>
{

    /// <summary>
    /// Gets the current <see cref="PatchBasedObjectComparer"/> instance
    /// </summary>
    public static readonly PatchBasedObjectComparer Instance = new();

    /// <inheritdoc/>
    public new bool Equals(object? x, object? y) => !JsonPatchHelper.CreateJsonPatchFromDiff(x!, y!).Operations.Any();

    /// <inheritdoc/>
    public int GetHashCode(object obj) => Serializer.Json.Serialize(obj).GetHashCode();

}