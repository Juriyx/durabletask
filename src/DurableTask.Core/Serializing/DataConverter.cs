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

    /// <summary>
    /// Abstract class for serializing and deserializing data
    /// </summary>
    public abstract class DataConverter
    {
        /// <summary>
        /// Serialize a value of type <typeparamref name="T"/> to a string with default formatting
        /// </summary>
        /// <typeparam name="T">The desired type to serialize.</typeparam>
        /// <param name="value">Value to serialize</param>
        /// <returns>Object serialized to a string</returns>
        public virtual string Serialize<T>(T value)
            => Serialize(value, typeof(T));

        /// <summary>
        /// Serialize a value of type <typeparamref name="T"/> to a string with supplied formatting
        /// </summary>
        /// <typeparam name="T">The desired type to serialize.</typeparam>
        /// <param name="value">Value to serialize</param>
        /// <param name="formatted">Boolean indicating whether to format the results or not</param>
        /// <returns>Object serialized to a string</returns>
        public virtual string Serialize<T>(T value, bool formatted)
            => Serialize(value, typeof(T), formatted);

        /// <summary>
        /// Serialize an object to a string with default formatting using the specified type
        /// </summary>
        /// <param name="value">Object to serialize</param>
        /// <param name="type">The type used when serializing.</param>
        /// <returns>Object serialized to a string</returns>
        public abstract string Serialize(object value, Type type);

        /// <summary>
        /// Serialize an object to a string with supplied formatting using the specified type
        /// </summary>
        /// <param name="value">Object to serialize</param>
        /// <param name="type">The type used when serializing.</param>
        /// <param name="formatted">Boolean indicating whether to format the results or not</param>
        /// <returns>Object serialized to a string</returns>
        public abstract string Serialize(object value, Type type, bool formatted);

        /// <summary>
        /// Deserialize a string to an Object of supplied type
        /// </summary>
        /// <param name="data">String data of the Object to deserialize</param>
        /// <param name="objectType">Type to deserialize to</param>
        /// <returns>Deserialized Object</returns>
        public abstract object Deserialize(string data, Type objectType);

        /// <summary>
        /// Deserialize a string to an Object of supplied type
        /// </summary>
        /// <param name="data">String data of the Object to deserialize</param>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <returns>Deserialized Object</returns>
        public virtual T Deserialize<T>(string data)
        {
            object result = this.Deserialize(data, typeof(T));
            return result == null ? default : (T)result;
        }
    }
}