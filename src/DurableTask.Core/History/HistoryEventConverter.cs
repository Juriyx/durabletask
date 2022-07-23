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
namespace DurableTask.Core.History
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using DurableTask.Core.Serializing;

    internal sealed class HistoryEventConverter : JsonCreationConverter<HistoryEvent>
    {
        protected override Type GetObjectType(JsonObject node, JsonSerializerOptions options)
        {
            if (node.TryGetPropertyValue(nameof(HistoryEvent.EventType), out JsonNode? property))
                return GetObjectType(property.Deserialize<EventType>(options));

            throw new JsonException($"Cannot find '{nameof(HistoryEvent.EventType)}' property.");
        }

        protected override Type GetObjectType(HistoryEvent value)
            => GetObjectType(value.EventType);

        private static Type GetObjectType(EventType eventType)
            => eventType switch
            {
                EventType.ContinueAsNew => typeof(ContinueAsNewEvent),
                EventType.EventRaised => typeof(EventRaisedEvent),
                EventType.EventSent => typeof(EventSentEvent),
                EventType.ExecutionCompleted => typeof(ExecutionCompletedEvent),
                // EventType.ExecutionFailed is unused
                EventType.ExecutionStarted => typeof(ExecutionStartedEvent),
                EventType.ExecutionTerminated => typeof(ExecutionTerminatedEvent),
                EventType.GenericEvent => typeof(GenericEvent),
                EventType.HistoryState => typeof(HistoryStateEvent),
                EventType.OrchestratorCompleted => typeof(OrchestratorCompletedEvent),
                EventType.OrchestratorStarted => typeof(OrchestratorStartedEvent),
                EventType.SubOrchestrationInstanceCompleted => typeof(SubOrchestrationInstanceCompletedEvent),
                EventType.SubOrchestrationInstanceCreated => typeof(SubOrchestrationInstanceCreatedEvent),
                EventType.SubOrchestrationInstanceFailed => typeof(SubOrchestrationInstanceFailedEvent),
                EventType.TaskCompleted => typeof(TaskCompletedEvent),
                EventType.TaskFailed => typeof(TaskFailedEvent),
                EventType.TaskScheduled => typeof(TaskScheduledEvent),
                EventType.TimerCreated => typeof(TimerCreatedEvent),
                EventType.TimerFired => typeof(TimerFiredEvent),
                _ => throw new JsonException($"Unrecognized EventType '{eventType}'."),
            };
    }
}
