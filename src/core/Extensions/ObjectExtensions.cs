﻿namespace Hylo;

/// <summary>
/// Defines extensions for <see cref="object"/>s
/// </summary>
public static class ObjectExtensions
{

    /// <summary>
    /// Clones the object
    /// </summary>
    /// <param name="obj">The object to clone</param>
    /// <returns>The clone</returns>
    public static object? Clone(this object? obj) => Serializer.Json.Deserialize<object>(Serializer.Json.Serialize(obj));

    /// <summary>
    /// Clones the object
    /// </summary>
    /// <typeparam name="T">The type of the object to clone</typeparam>
    /// <param name="obj">The object to clone</param>
    /// <returns>The clone</returns>
    public static T? Clone<T>(this T? obj) => Serializer.Json.Deserialize<T>(Serializer.Json.Serialize(obj));

    /// <summary>
    /// Converts the object into a new <see cref="Dictionary{TKey, TValue}"/>
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <returns>A new <see cref="Dictionary{TKey, TValue}"/></returns>
    public static Dictionary<string, object>? ToDictionary(this object? obj) => Serializer.Json.Deserialize<Dictionary<string, object>>(Serializer.Json.Serialize(obj));

    /// <summary>
    /// Converts the object into a new <see cref="Dictionary{TKey, TValue}"/>
    /// </summary>
    /// <param name="obj">The object to convert</param>
    /// <returns>A new <see cref="Dictionary{TKey, TValue}"/></returns>
    public static Dictionary<string, TValue>? ToDictionary<TValue>(this object? obj) => Serializer.Json.Deserialize<Dictionary<string, TValue>>(Serializer.Json.Serialize(obj));

    /// <summary>
    /// Converts the object to the specified type by using JSON serdes
    /// </summary>
    /// <typeparam name="T">The type to convert the object to</typeparam>
    /// <param name="obj">The object to convert</param>
    /// <returns>The converted object</returns>
    public static T? ConvertTo<T>(this object? obj)
    {
        if (obj == null) return default;
        return obj switch
        {
            T t => t,
            JsonElement jsonElem => Serializer.Json.Deserialize<T>(jsonElem),
            JsonNode jsonNode => Serializer.Json.Deserialize<T>(jsonNode),
            _ => Serializer.Json.Deserialize<T>(Serializer.Json.Serialize(obj))
        };
    }

}