//  ----------------------------------------------------------------------------------
//  Copyright Microsoft Corporation
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------

namespace DurableTask.Core.Serializing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization;

    /// <summary>
    ///     Helper class for supporting deserialization from JSON into a custom class hierarchy
    /// </summary>
    internal abstract class JsonCreationConverter<T> : JsonConverter<T> where T : class
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        // Note: These methods only work as long as the converter is not used for the "object type" directly

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                return null;
            }

            JsonNode node = JsonNode.Parse(ref reader);

            // Create target object based on JsonNode 
            Type targetType = GetObjectType(node.AsObject(), options);

            // For old data serialized with $type, we need to remove it before performing
            // the member for finally deserializing the object in case preserve mode is enabled
            node = RemoveTypeProperties(node.AsObject());

            return (T)node.Deserialize(targetType, options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, GetObjectType(value), options);

        /// <summary>
        ///     Gets the objectType based on properties in the JSON object
        /// </summary>
        protected abstract Type GetObjectType(JsonObject node, JsonSerializerOptions options);

        /// <summary>
        ///     Gets the objectType based on the value.
        /// </summary>
        protected abstract Type GetObjectType(T value);

        private static JsonNode RemoveTypeProperties(JsonObject obj)
        {
            // Remove if present
            if (obj.Remove("$type"))
            {
                // In JSON.NET, arrays are serialized as objects and require some finesse to unwrap the array
                if (!obj.ContainsKey("$id") && obj.TryGetPropertyValue("$value", out JsonNode arrayValue))
                {
                    return arrayValue;
                }
            }

            // Recursively search each of the nested objects
            List<string> candidates = obj.Where(x => x.Value is JsonObject).Select(x => x.Key).ToList();
            foreach (string propertyName in candidates)
            {
                obj[propertyName] = RemoveTypeProperties(obj[propertyName].AsObject());
            }

            return obj;
        }
    }
}