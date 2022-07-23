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
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using DurableTask.Core.Serializing;

    internal sealed class TraceContextConverter : JsonCreationConverter<TraceContextBase>
    {
        protected override Type GetObjectType(JsonObject obj, JsonSerializerOptions options)
{
            if (obj.TryGetPropertyValue(nameof(TraceContextBase.Type), out JsonNode property))
                return GetObjectType(JsonSerializer.Deserialize<TraceContextType>(property, options));
            else if (obj.TryGetPropertyValue("$type", out property))
                return GetDeprecatedObjectType(Type.GetType(property.AsValue().GetValue<string>()));

            throw new JsonException("TraceContext 'Type' property not provided.");
        }

        protected override Type GetObjectType(TraceContextBase value)
            => GetObjectType(value.Type);

        private static Type GetObjectType(TraceContextType value)
            => value switch
            {
                TraceContextType.HttpCorrelationProtocol => typeof(HttpCorrelationProtocolTraceContext),
                TraceContextType.NullObject => typeof(NullObjectTraceContext),
                TraceContextType.W3C => typeof(W3CTraceContext),
                _ => throw new JsonException($"Invalid TraceContextType '{value}'."),
            };

        private static Type GetDeprecatedObjectType(Type type)
        {
            if (type == typeof(HttpCorrelationProtocolTraceContext) ||
                type == typeof(NullObjectTraceContext) ||
                type == typeof(W3CTraceContext))
                return type;
            else
                throw new JsonException($"Invalid TraceContextType '{type}'.");
        }
    }
}
