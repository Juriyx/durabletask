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
    using System.Text.Json;
    using DurableTask.Core.Common;

    /// <summary>
    /// Class for serializing and deserializing data to and from json
    /// </summary>
    public class JsonDataConverter : DataConverter
    {
        /// <summary>
        /// Default JsonDataConverter
        /// </summary>
        public static readonly JsonDataConverter Default = new JsonDataConverter();

        readonly JsonSerializerOptions _options;
        readonly JsonSerializerOptions _indentedOptions;

        /// <summary>
        /// Creates a new instance of the <see cref="JsonDataConverter"/> with default settings
        /// </summary>
        public JsonDataConverter()
            : this(Utils.InternalSerializerOptions)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="JsonDataConverter"/> with supplied settings
        /// </summary>
        /// <param name="options">Options for the json serializer</param>
        public JsonDataConverter(JsonSerializerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = new JsonSerializerOptions(options) { WriteIndented = false };
            _indentedOptions = new JsonSerializerOptions(options) { WriteIndented = true };
        }

        /// <summary>
        /// Serialize an object to a string with default formatting using the specified type
        /// </summary>
        /// <param name="value">Object to serialize</param>
        /// <param name="type">The type used when serializing.</param>
        /// <returns>Object serialized to a string</returns>
        public override string Serialize(object value, Type type)
            => Serialize(value, type, false);

        /// <summary>
        /// Serialize an object to a string with supplied formatting using the specified type
        /// </summary>
        /// <param name="value">Object to serialize</param>
        /// <param name="type">The type used when serializing.</param>
        /// <param name="formatted">Boolean indicating whether to format the results or not</param>
        /// <returns>Object serialized to a string</returns>
        public override string Serialize(object value, Type type, bool formatted)
        {
            if (value == null)
            {
                // This avoids serializing null into "null"
                return null;
            }

            return JsonSerializer.Serialize(value, type, formatted ? _indentedOptions : _options);
        }

        /// <summary>
        /// Deserialize a string to an Object of supplied type
        /// </summary>
        /// <param name="data">String data of the Object to deserialize</param>
        /// <param name="objectType">Type to deserialize to</param>
        /// <returns>Deserialized Object</returns>
        public override object Deserialize(string data, Type objectType)
        {
            if (data == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize(data, objectType);
        }
    }
}