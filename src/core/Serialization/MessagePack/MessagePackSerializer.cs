using MessagePack;

namespace Hylo;

public static partial class Serializer
{

    /// <summary>
    /// Provides functionality to serialize/deserialize objects to/from MessagePack
    /// </summary>
    public static class MessagePack
    {

        /// <summary>
        /// Gets/sets an <see cref="Action{T}"/> used to configure the <see cref="MessagePackSerializerOptions"/> used by default
        /// </summary>
        public static Action<MessagePackSerializerOptions>? DefaultOptionsConfiguration { get; set; } = (options) =>
        {

        };

        static MessagePackSerializerOptions? _DefaultOptions;
        /// <summary>
        /// Gets/sets the default <see cref="MessagePackSerializerOptions"/>
        /// </summary>
        public static MessagePackSerializerOptions DefaultOptions
        {
            get
            {
                if (_DefaultOptions != null) return _DefaultOptions;
                _DefaultOptions = MessagePackSerializerOptions.Standard;
                if (DefaultOptionsConfiguration != null) DefaultOptionsConfiguration(_DefaultOptions);
                return _DefaultOptions;
            }
        }

        /// <summary>
        /// Serializes the specified object
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="schema">The object to serialize</param>
        /// <returns>The serialized object</returns>
        public static byte[] Serialize<T>(T schema) => MessagePackSerializer.Serialize(schema, DefaultOptions);

        /// <summary>
        /// Serializes the specified object to JSON
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="schema">The object to serialize</param>
        /// <returns>The serialized object, in JSON format</returns>
        public static string SerializeToJson<T>(T schema) => MessagePackSerializer.SerializeToJson(schema, DefaultOptions);

        /// <summary>
        /// Deserializes the specified byte array into a new object
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize the byte array into</typeparam>
        /// <param name="bytes">The byte array that contains the MessagePack image of the object to deserialize</param>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(byte[] bytes) => MessagePackSerializer.Deserialize<T>(bytes, DefaultOptions);

    }

}
