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

namespace DurableTask.Core.Command
{
    using System;
    using System.Text.Json;
    using DurableTask.Core.Serializing;

    internal class OrchestrationActionConverter : JsonCreationConverter<OrchestratorAction>
    {
        protected override Type GetObjectType(JsonElement element, JsonSerializerOptions options)
        {
            if (element.TryGetProperty(nameof(OrchestratorAction.OrchestratorActionType), out JsonElement property))
            {
                return GetObjectType(property.Deserialize<OrchestratorActionType>(options));
            }

            throw new JsonException("Action Type not provided.");
        }

        protected override Type GetObjectType(OrchestratorAction value)
            => GetObjectType(value.OrchestratorActionType);

        private static Type GetObjectType(OrchestratorActionType actionType)
            => actionType switch
            {
                OrchestratorActionType.CreateTimer => typeof(CreateTimerOrchestratorAction),
                OrchestratorActionType.OrchestrationComplete => typeof(OrchestrationCompleteOrchestratorAction),
                OrchestratorActionType.ScheduleOrchestrator => typeof(ScheduleTaskOrchestratorAction),
                OrchestratorActionType.CreateSubOrchestration => typeof(CreateSubOrchestrationAction),
                _ => throw new JsonException("Unrecognized action type."),
            };
    }
}