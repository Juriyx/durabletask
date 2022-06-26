﻿//  ----------------------------------------------------------------------------------
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

namespace DurableTask.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using DurableTask.Core.Serializing;

    /// <summary>
    /// Represents the state of an orchestration instance
    /// </summary>
    [DataContract]
    public class OrchestrationInstance : IExtensibleDataObject
    {
        /// <summary>
        /// The instance id, assigned as unique to the orchestration
        /// </summary>
        [DataMember]
        public string InstanceId { get; set; }

        /// <summary>
        /// The execution id, unique to the execution of this instance
        /// </summary>
        [DataMember]
        public string ExecutionId { get; set; }

        internal OrchestrationInstance Clone()
        {
            return new OrchestrationInstance
            {
                ExecutionId = ExecutionId,
                InstanceId = InstanceId
            };
        }

        /// <summary>
        /// Serves as a hash function for an OrchestrationInstance. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return (this.InstanceId ?? string.Empty).GetHashCode() ^ (this.ExecutionId ?? string.Empty).GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the OrchestrationInstance.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"[InstanceId: {this.InstanceId}, ExecutionId: {this.ExecutionId}]";
        }

        /// <summary>
        /// Implementation for <see cref="IExtensibleDataObject.ExtensionData"/>.
        /// </summary>
        [JsonIgnore]
        [Obsolete("XML serialization has been deprecated.")]
        public ExtensionDataObject ExtensionData
        {
            get => _extensionData?.Xml;
            set
            {
                if (_extensionData == null)
                    _extensionData = new ExtensionData(value);
                else
                    _extensionData.Xml = value;
            }
        }

        /// <summary>
        /// Gets or sets additional data encoded in JSON that was not explicitly represented by the <see cref="OrchestrationInstance"/> type.
        /// </summary>
        [IgnoreDataMember]
        [JsonExtensionData]
        public Dictionary<string, JsonElement> JsonExtensionData
        {
            get => _extensionData?.Json;
            set
            {
                if (_extensionData == null)
                    _extensionData = new ExtensionData(value);
                else
                    _extensionData.Json = value;
            }
        }

        private ExtensionData _extensionData;
    }
}