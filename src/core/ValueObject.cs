namespace Hylo;

/// <summary>
/// Represents the base class for all <see cref="ValueObject{T}"/>s
/// </summary>
/// <typeparam name="T">The type of the <see cref="ValueObject{T}"/></typeparam>
public abstract class ValueObject<T>
    : IEquatable<ValueObject<T>>
    where T : ValueObject<T>
{

    /// <inheritdoc/>
    public bool Equals(ValueObject<T>? other)
    {
#pragma warning disable IDE0041 // Use 'is null' check
        if (ReferenceEquals(this, null) ^ ReferenceEquals(other, null)) return false;
        return ReferenceEquals(this, other) || this.Equals(other);
#pragma warning restore IDE0041 // Use 'is null' check
    }

    /// <inheritdoc/>
    public override bool Equals(object? other)
    {
        if (other is ValueObject<T> valueObject) return this.Equals(valueObject);
        else return false;
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the components use to generate the <see cref="ValueObject{T}"/>'s hashcode
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing the components use to generate the <see cref="ValueObject{T}"/>'s hashcode</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <inheritdoc/>
    public override int GetHashCode() => GetEqualityComponents().Select(x => x != null ? x.GetHashCode() : 0).Aggregate((x, y) => x ^ y);

    /// <summary>
    /// Determines whether the two specified <see cref="ValueObject{T}"/>s are equal
    /// </summary>
    /// <param name="type1">The first <see cref="ValueObject{T}"/></param>
    /// <param name="type2">The second <see cref="ValueObject{T}"/></param>
    /// <returns>A boolean indicating whether or not the two specified <see cref="ValueObject{T}"/>s are equal</returns>
    public static bool operator ==(ValueObject<T>? type1, ValueObject<T>? type2) => type1?.Equals(type2) == true;

    /// <summary>
    /// Determines whether the two specified <see cref="ValueObject{T}"/>s are not equal
    /// </summary>
    /// <param name="type1">The first <see cref="ValueObject{T}"/></param>
    /// <param name="type2">The second <see cref="ValueObject{T}"/></param>
    /// <returns>A boolean indicating whether or not the two specified <see cref="ValueObject{T}"/>s are not equal</returns>
    public static bool operator !=(ValueObject<T>? type1, ValueObject<T>? type2) => type1?.Equals(type2) == false;

}