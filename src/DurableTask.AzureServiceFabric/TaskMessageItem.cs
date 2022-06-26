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

namespace DurableTask.AzureServiceFabric
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using DurableTask.Core;
    using DurableTask.Core.Serializing;

    [DataContract]
    sealed class TaskMessageItem : IExtensibleDataObject
    {
        public TaskMessageItem(TaskMessage taskMessage)
        {
            this.TaskMessage = taskMessage ?? throw new ArgumentNullException(nameof(taskMessage));
        }

        [DataMember]
        public TaskMessage TaskMessage { get; private set; }

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
