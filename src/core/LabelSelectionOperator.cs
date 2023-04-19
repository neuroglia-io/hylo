namespace Hylo;

/// <summary>
/// Enumerates resoruce label selection operators 
/// </summary>
[TypeConverter(typeof(StringEnumTypeConverter))]
[JsonConverter(typeof(JsonStringEnumConverterFactory))]
public enum LabelSelectionOperator
{
    /// <summary>
    /// Indicates that the label value must be equal to the specified value
    /// </summary>
    [EnumMember(Value = "equals")]
    Equals,
    /// <summary>
    /// Indicates that the label value must not be equal to the specified value
    /// </summary>
    [EnumMember(Value = "not-equals")]
    NotEquals,
    /// <summary>
    /// Indicates that the resource must have the specified label. If values have been supplied, the label must also have one of the specified values
    /// </summary>
    [EnumMember(Value = "contains")]
    Contains,
    /// <summary>
    /// Indicates that the resource must not the specified label. If values have been supplied, the label must also have one of the specified values
    /// </summary>
    [EnumMember(Value = "not-contains")]
    NotContains
}
