namespace Hylo;

/// <summary>
/// Enumerates all default types of resource-related operations
/// </summary>
[TypeConverter(typeof(StringEnumTypeConverter))]
[JsonConverter(typeof(JsonStringEnumConverterFactory))]
public enum ResourceOperation
{
    /// <summary>
    /// Indicates the operation to create a new resource
    /// </summary>
    [EnumMember(Value = "create")]
    Create = 1,
    /// <summary>
    /// Indicates the operation to update an existing resource
    /// </summary>
    [EnumMember(Value = "update")]
    Update = 2,
    /// <summary>
    /// Indicates the operation to delete an existing resource
    /// </summary>
    [EnumMember(Value = "delete")]
    Delete = 4
}