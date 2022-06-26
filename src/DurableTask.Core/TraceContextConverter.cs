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
    using System.Text.Json;
    using DurableTask.Core.Command;
    using DurableTask.Core.Serializing;

    internal sealed class TraceContextConverter : JsonCreationConverter<TraceContextBase>
    {
        protected override Type GetObjectType(JsonElement element, JsonSerializerOptions options)
{
            if (element.TryGetProperty(nameof(TraceContextBase.Type), out JsonElement property))
                return GetObjectType(property.GetString());
            else if (element.TryGetProperty("$type", out property))
                return GetDeprecatedObjectType(Type.GetType(property.GetString()));

            throw new JsonException("TraceContext 'Type' property not provided.");
        }

        protected override Type GetObjectType(TraceContextBase value)
            => value.Type switch
            {
                nameof(HttpCorrelationProtocolTraceContext) => typeof(CreateTimerOrchestratorAction),
                nameof(NullObjectTraceContext) => typeof(OrchestrationCompleteOrchestratorAction),
                nameof(W3CTraceContext) => typeof(ScheduleTaskOrchestratorAction),
                _ => throw new JsonException($"Unrecognized TraceContext type '{value.Type}'."),
            };

        private static Type GetObjectType(string value)
            => value switch
            {
                nameof(HttpCorrelationProtocolTraceContext) => typeof(CreateTimerOrchestratorAction),
                nameof(NullObjectTraceContext) => typeof(OrchestrationCompleteOrchestratorAction),
                nameof(W3CTraceContext) => typeof(ScheduleTaskOrchestratorAction),
                _ => throw new JsonException($"Unrecognized TraceContext type '{value}'."),
            };

        private static Type GetDeprecatedObjectType(Type type)
        {
            if (type == typeof(HttpCorrelationProtocolTraceContext) ||
                type == typeof(NullObjectTraceContext) ||
                type == typeof(W3CTraceContext))
                return type;
            else
                throw new JsonException($"Unrecognized TraceContext type '{type}'.");
        }
    }
}
