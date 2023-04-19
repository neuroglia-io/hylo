namespace Hylo;

/// <summary>
/// Enumerates all supported resource scopes
/// </summary>
[TypeConverter(typeof(StringEnumTypeConverter))]
[JsonConverter(typeof(JsonStringEnumConverterFactory))]
public enum ResourceScope
{
    /// <summary>
    /// Indicates a namespaced resource
    /// </summary>
    [EnumMember(Value = "Namespaced")]
    Namespaced = 1,
    /// <summary>
    /// Indicates a cluster resource
    /// </summary>
    [EnumMember(Value = "Cluster")]
    Cluster = 2
}
