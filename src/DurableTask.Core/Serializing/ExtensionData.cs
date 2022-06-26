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

#nullable enable
namespace DurableTask.Core.Serializing
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.Json;

    /// <summary>
    /// Represents additional encoded data that was not explicitly represented by a type.
    /// </summary>
    public sealed class ExtensionData
    {
        /// <summary>
        /// Gets or sets additional data encoded in JSON that was not explicitly represented by the type.
        /// </summary>
        /// <value>A dictionary of the extra JSON properties, if any; otherwise, <see langword="null"/>.</value>
        /// <exception cref="InvalidOperationException">The instance already contains XML data.</exception>
        public Dictionary<string, JsonElement>? Json
        {
            get
            {
                if (_data is ExtensionDataObject)
                    throw new InvalidOperationException("Extension data was deserialized from XML.");

                return _data as Dictionary<string, JsonElement>;
            }
            set
            {
                if (_data is ExtensionDataObject)
                    throw new InvalidOperationException("XML data already exists.");

                _data = value;
            }
        }

        /// <summary>
        /// Gets or sets additional data encoded in XML that was not explicitly represented by the type.
        /// </summary>
        /// <value>
        /// An instance of <see cref="ExtensionDataObject"/> if there was any additional data;
        /// otherwise, <see langword="null"/>.
        /// </value>
        /// <exception cref="InvalidOperationException">The instance already contains JSON data.</exception>
        public ExtensionDataObject? Xml
        {
            get
            {
                if (_data is Dictionary<string, JsonElement>)
                    throw new InvalidOperationException("Extension data was deserialized from JSON.");

                return _data as ExtensionDataObject;
            }
            set
            {
                if (_data is Dictionary<string, JsonElement>)
                    throw new InvalidOperationException("JSON data already exists.");

                _data = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionData"/> based on additional JSON data.
        /// </summary>
        /// <param name="jsonData">Optional dictionary of additional JSON properties.</param>
        public ExtensionData(Dictionary<string, JsonElement>? jsonData)
            => _data = jsonData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionData"/> based on additional XML data.
        /// </summary>
        /// <param name="xmlData">Optional XML extension data.</param>
        public ExtensionData(ExtensionDataObject? xmlData)
            => _data = xmlData;

        private object? _data;
    }
}
