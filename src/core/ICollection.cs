namespace Hylo;

/// <summary>
/// Defines the fundamentals of a collection
/// </summary>
public interface ICollection
    : IObject, IMetadata<CollectionMetadata>
{

    /// <summary>
    /// Gets the items the collection is made out of
    /// </summary>
    [Required]
    [DataMember(Order = 1, Name = "items", IsRequired = true), JsonPropertyOrder(1), JsonPropertyName("items"), YamlMember(Order = 1, Alias = "items")]
    EquatableList<object>? Items { get; }

}

/// <summary>
/// Defines the fundamentals of a collection
/// </summary>
/// <typeparam name="TObject">The expected type of object</typeparam>
public interface ICollection<TObject>
    : ICollection, IObject
    where TObject : class, IObject, new()
{

    /// <summary>
    /// Gets the items the collection is made out of
    /// </summary>
    new EquatableList<TObject>? Items { get; }

}
