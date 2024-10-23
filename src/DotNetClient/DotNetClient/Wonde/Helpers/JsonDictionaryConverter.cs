using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wonde.Helpers
{
    internal class JsonDictionaryConverter : JsonConverter<Dictionary<string, object>>
    {
        public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = new Dictionary<string, object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dict;
                }

                string key = reader.GetString();
                reader.Read(); // Move to value

                object value;

                // Handle different JSON value types
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    // Recursively handle nested objects
                    value = Read(ref reader, typeToConvert, options);
                }
                else if (reader.TokenType == JsonTokenType.StartArray)
                {
                    // Handle arrays
                    value = ReadArray(ref reader, options);
                }
                else
                {
                    // Read primitive values and convert them to appropriate types
                    value = ReadPrimitive(ref reader);
                }

                dict[key] = value;
            }

            return dict;
        }

        private object ReadPrimitive(ref Utf8JsonReader reader)
        {
            // Handle primitive types
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => reader.GetDouble(), // Use GetInt32 or GetInt64 as needed
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Null => null,
                _ => throw new JsonException($"Unexpected token type: {reader.TokenType}")
            };
        }

        private List<object> ReadArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var list = new List<object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return list;
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    list.Add(Read(ref reader, typeof(Dictionary<string, object>), options));
                }
                else
                {
                    list.Add(ReadPrimitive(ref reader));
                }
            }

            return list;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kvp in value)
            {
                writer.WritePropertyName(kvp.Key);
                JsonSerializer.Serialize(writer, kvp.Value, options);
            }
            writer.WriteEndObject();
        }
    }
}