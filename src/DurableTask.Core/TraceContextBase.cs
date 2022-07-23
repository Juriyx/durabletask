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

namespace DurableTask.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// TraceContext keep the correlation value.
    /// </summary>
    [JsonConverter(typeof(TraceContextConverter))]
    public abstract class TraceContextBase
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        protected TraceContextBase()
        {
            OrchestrationTraceContexts = new Stack<TraceContextBase>();
        }

        /// <summary>
        /// The type for the derived <see cref="TraceContextBase"/> class.
        /// </summary>
        [JsonInclude]
        internal abstract TraceContextType Type { get; }

        /// <summary>
        /// Start time of this telemetry
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Type of this telemetry.
        /// Request Telemetry or Dependency Telemetry.
        /// Use
        /// <see cref="TelemetryType"/> 
        /// </summary>
        public TelemetryType TelemetryType { get; set; }

        /// <summary>
        /// OrchestrationState save the state of the 
        /// </summary>
        public Stack<TraceContextBase> OrchestrationTraceContexts { get; set; }

        /// <summary>
        /// Keep OperationName in case, don't have an Activity in this context
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// Current Activity only managed by this concrete class.
        /// This property is not serialized.
        /// </summary>
        [JsonIgnore]
        internal Activity CurrentActivity { get; set; }

        /// <summary>
        /// Return if the orchestration is on replay
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public bool IsReplay { get; set; } = false;

        /// <summary>
        /// Duration of this context. Valid after call Stop() method.
        /// </summary>
        [JsonIgnore]
        public abstract TimeSpan Duration { get; }

        [JsonIgnore]
        static JsonSerializerOptions CustomJsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            ReferenceHandler = ReferenceHandler.Preserve,
        };

        /// <summary>
        /// Serializable Json string of TraceContext
        /// </summary>
        [JsonIgnore]
        public string SerializableTraceContext => 
            JsonSerializer.Serialize(this, CustomJsonSerializerOptions);

        /// <summary>
        /// Telemetry.Id Used for sending telemetry. refer this URL
        /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.applicationinsights.extensibility.implementation.operationtelemetry?view=azure-dotnet
        /// </summary>
        [JsonIgnore]
        public abstract string TelemetryId { get; }

        /// <summary>
        /// Telemetry.Context.Operation.Id Used for sending telemetry refer this URL
        /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.applicationinsights.extensibility.implementation.operationtelemetry?view=azure-dotnet
        /// </summary>
        [JsonIgnore]
        public abstract string TelemetryContextOperationId { get; }

        /// <summary>
        /// Get RequestTraceContext of Current Orchestration
        /// </summary>
        /// <returns></returns>
        public TraceContextBase GetCurrentOrchestrationRequestTraceContext()
        {
            foreach(TraceContextBase element in OrchestrationTraceContexts)
            {
                if (TelemetryType.Request == element.TelemetryType) return element;
            }

            throw new InvalidOperationException("Can not find RequestTraceContext");
        }

        /// <summary>
        /// Telemetry.Context.Operation.ParentId Used for sending telemetry refer this URL
        /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.applicationinsights.extensibility.implementation.operationtelemetry?view=azure-dotnet
        /// </summary>
        [JsonIgnore]
        public abstract string TelemetryContextOperationParentId { get; }

        /// <summary>
        /// Set Parent TraceContext and Start the context
        /// </summary>
        /// <param name="parentTraceContext"> Parent Trace</param>
        public abstract void SetParentAndStart(TraceContextBase parentTraceContext);

        /// <summary>
        /// Start TraceContext as new
        /// </summary>
        public abstract void StartAsNew();

        /// <summary>
        /// Stop TraceContext
        /// </summary>
        public void Stop() => CurrentActivity?.Stop();

        /// <summary>
        /// Set Activity.Current to CurrentActivity
        /// </summary>
        public void SetActivityToCurrent()
        {
            Activity.Current = CurrentActivity;
        }

        /// <summary>
        /// Restore TraceContext sub class
        /// </summary>
        /// <param name="json">Serialized json of TraceContext sub classes</param>
        /// <returns></returns>
        public static TraceContextBase Restore(string json)
        {
            // If the JSON is empty, we assume to have an empty context
            if (string.IsNullOrEmpty(json))
            {
                return TraceContextFactory.Empty;
            }

            TraceContextBase restored = JsonSerializer.Deserialize<TraceContextBase>(json, CustomJsonSerializerOptions);
            restored.OrchestrationTraceContexts = new Stack<TraceContextBase>(restored.OrchestrationTraceContexts);

            return restored;
        }
    }

    /// <summary>
    /// Telemetry Type
    /// </summary>
    public enum TelemetryType
    {
        /// <summary>
        /// Request Telemetry
        /// </summary>
        Request,

        /// <summary>
        /// Dependency Telemetry
        /// </summary>
        Dependency,
    }
}
