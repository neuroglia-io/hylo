﻿using Hylo.Serialization.Yaml;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Hylo;

/// <summary>
/// Provides functionality to serialize/deserialize objects to/from JSON and YAML
/// </summary>
public static partial class Serializer
{

    /// <summary>
    /// Provides functionality to serialize/deserialize objects to/from YAML
    /// </summary>
    public static class Yaml
    {

        static readonly ISerializer Serializer;
        static readonly IDeserializer Deserializer;

        static Yaml()
        {
            Serializer = new SerializerBuilder()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new JsonNodeTypeConverter())
                .WithTypeConverter(new JsonSchemaTypeConverter())
                .WithTypeConverter(new JsonPointerTypeConverter())
                .WithTypeConverter(new StringEnumSerializer())
                .WithTypeConverter(new UriTypeSerializer())
                .Build();
            Deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithNodeDeserializer(
                    inner => new StringEnumDeserializer(inner),
                    syntax => syntax.InsteadOf<ScalarNodeDeserializer>())
                .WithNodeTypeResolver(new InferTypeResolver())
                .WithNodeDeserializer(
                    inner => new JsonObjectDeserializer(inner),
                    syntax => syntax.InsteadOf<DictionaryNodeDeserializer>())
                .WithNodeDeserializer(
                    inner => new JsonSchemaDeserializer(inner),
                    syntax => syntax.InsteadOf<JsonObjectDeserializer>())
                .Build();
        }

        /// <summary>
        /// Serializes the specified object to YAML
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="graph">The object to serialized</param>
        /// <returns>The YAML of the serialized object</returns>
        public static string Serialize<T>(T graph) => Serializer.Serialize(graph!);

        /// <summary>
        /// Serializes the specified object to YAML
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="writer">The <see cref="TextWriter"/> to the YAML to</param>
        /// <param name="graph">The object to serialized</param>
        /// <returns>The YAML of the serialized object</returns>
        public static void Serialize<T>(TextWriter writer, T graph) => Serializer.Serialize(writer, graph!);

        /// <summary>
        /// Deserializes the specified YAML input
        /// </summary>
        /// <param name="yaml">The YAML input to deserialize</param>
        /// <param name="type">The type to deserialize the specified YAML into</param>
        /// <returns>The deserialized value</returns>
        public static object? Deserialize(string yaml, Type type) => Deserializer.Deserialize(yaml, type);

        /// <summary>
        /// Deserializes the specified YAML input
        /// </summary>
        /// <typeparam name="T">The type to deserialize the specified YAML into</typeparam>
        /// <param name="yaml">The YAML input to deserialize</param>
        /// <returns>The deserialized value</returns>
        public static T? Deserialize<T>(string yaml) => Deserializer.Deserialize<T>(yaml);

        /// <summary>
        /// Deserializes the specified YAML input
        /// </summary>
        /// <typeparam name="T">The type to deserialize the specified YAML into</typeparam>
        /// <param name="reader">The <see cref="TextReader"/> to read the YAML to deserialize</param>
        /// <returns>The deserialized value</returns>
        public static T? Deserialize<T>(TextReader reader) => Deserializer.Deserialize<T>(reader);

        /// <summary>
        /// Deserializes the specified YAML input
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> to read the YAML to deserialize</param>
        /// <param name="type">The type to deserialize the specified YAML into</param>
        /// <returns>The deserialized value</returns>
        public static object? Deserialize(TextReader reader, Type type) => Deserializer.Deserialize(reader, type);

        /// <summary>
        /// Converts the specified JSON input into YAML
        /// </summary>
        /// <param name="json">The JSON input to convert</param>
        /// <returns>The JSON input converted into YAML</returns>
        public static string ConvertFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null!;
            var graph = Json.Deserialize<object>(json);
            return Serialize(graph);
        }

        /// <summary>
        /// Converts the specified YAML input into JSON
        /// </summary>
        /// <param name="yaml">The YAML input to convert</param>
        /// <param name="indented">A boolean indicating whether or not to indent the output</param>
        /// <returns>The YAML input converted into JSON</returns>
        public static string ConvertToJson(string yaml, bool indented = false)
        {
            if (string.IsNullOrWhiteSpace(yaml)) return null!;
            var graph = Deserialize<object>(yaml);
            return Json.Serialize(graph, indented);
        }

    }

}