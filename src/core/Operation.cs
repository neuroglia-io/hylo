namespace Hylo;

/// <summary>
/// Enumerates all default types of resource-related operations
/// </summary>
[TypeConverter(typeof(StringEnumTypeConverter))]
[JsonConverter(typeof(JsonStringEnumConverterFactory))]
public enum Operation
{
    /// <summary>
    /// Indicates the operation to create a new resource
    /// </summary>
    [EnumMember(Value = "create")]
    Create = 1,
    /// <summary>
    /// Indicates the operation to replace an existing resource
    /// </summary>
    [EnumMember(Value = "replace")]
    Replace = 2,
    /// <summary>
    /// Indicates the operation to patch an existing resource
    /// </summary>
    [EnumMember(Value = "patch")]
    Patch = 4,
    /// <summary>
    /// Indicates the operation to delete an existing resource
    /// </summary>
    [EnumMember(Value = "delete")]
    Delete = 8
}