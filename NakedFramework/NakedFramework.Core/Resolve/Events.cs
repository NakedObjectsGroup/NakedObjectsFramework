// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Resolve;

namespace NakedFramework.Core.Resolve {
    public static class Events {
        public static readonly IResolveEvent DestroyEvent;
        public static readonly IResolveEvent EndPartResolvingEvent;
        public static readonly IResolveEvent EndPartSetupEvent;
        public static readonly IResolveEvent EndResolvingEvent;
        public static readonly IResolveEvent EndSerializingEvent;
        public static readonly IResolveEvent EndSetupEvent;
        public static readonly IResolveEvent EndUpdatingEvent;
        public static readonly IResolveEvent InitializeAggregateEvent;
        public static readonly IResolveEvent InitializePersistentEvent;
        public static readonly IResolveEvent InitializeTransientEvent;
        public static readonly IResolveEvent ResetEvent;
        public static readonly IResolveEvent StartPartResolvingEvent;
        public static readonly IResolveEvent StartPartSetupEvent;
        public static readonly IResolveEvent StartResolvingEvent;
        public static readonly IResolveEvent StartSerializingEvent;
        public static readonly IResolveEvent StartSetupEvent;
        public static readonly IResolveEvent StartUpdatingEvent;

        static Events() {
            DestroyEvent = new ResolveStateMachine.DestroyEvent();
            EndPartResolvingEvent = new ResolveStateMachine.EndPartResolvingEvent();
            EndResolvingEvent = new ResolveStateMachine.EndResolvingEvent();
            EndSerializingEvent = new ResolveStateMachine.EndSerializingEvent();
            EndUpdatingEvent = new ResolveStateMachine.EndUpdatingEvent();
            InitializeAggregateEvent = new ResolveStateMachine.InitializeAggregateEvent();
            InitializePersistentEvent = new ResolveStateMachine.InitializePersistentEvent();
            InitializeTransientEvent = new ResolveStateMachine.InitializeTransientEvent();
            ResetEvent = new ResolveStateMachine.ResetEvent();
            StartPartResolvingEvent = new ResolveStateMachine.StartPartResolvingEvent();
            StartResolvingEvent = new ResolveStateMachine.StartResolvingEvent();
            StartSerializingEvent = new ResolveStateMachine.StartSerializingEvent();
            StartUpdatingEvent = new ResolveStateMachine.StartUpdatingEvent();
            StartSetupEvent = new ResolveStateMachine.StartSetupEvent();
            StartPartSetupEvent = new ResolveStateMachine.StartPartSetupEvent();
            EndSetupEvent = new ResolveStateMachine.EndSetupEvent();
            EndPartSetupEvent = new ResolveStateMachine.EndPartSetupEvent();
        }
    }
}