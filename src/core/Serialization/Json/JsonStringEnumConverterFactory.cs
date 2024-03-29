﻿using System.Collections.Concurrent;

namespace Hylo.Serialization.Json;

/// <summary>
/// Represents the <see cref="JsonConverterFactory"/> used to create <see cref="JsonStringEnumConverter{TEnum}"/>
/// </summary>
public class JsonStringEnumConverterFactory
    : JsonConverterFactory
{

    /// <summary>
    /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing the mappings of types to their respective <see cref="JsonConverter"/>
    /// </summary>
    private static readonly ConcurrentDictionary<Type, JsonConverter> Converters = new();

    /// <inheritdoc/>
	public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum || (typeToConvert.IsGenericType && typeToConvert.IsNullable() && typeToConvert.GetGenericArguments().First().IsEnum);

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var enumType = typeToConvert;
        if (enumType.IsGenericType && enumType.IsNullable() && enumType.GetGenericArguments().First().IsEnum) enumType = enumType.GetGenericArguments().First();
        if (!Converters.TryGetValue(typeToConvert, out var converter) || converter == null)
        {
            var converterType = typeof(JsonStringEnumConverter<>).MakeGenericType(typeToConvert);
            converter = (JsonConverter)Activator.CreateInstance(converterType, enumType)!;
            Converters.TryAdd(typeToConvert, converter);
        }
        return converter;
    }

}
