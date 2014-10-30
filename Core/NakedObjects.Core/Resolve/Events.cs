// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Resolve;

namespace NakedObjects.Core.Resolve {
    public static class Events {
        public static IResolveEvent DestroyEvent;
        public static IResolveEvent EndPartResolvingEvent;
        public static IResolveEvent EndPartSetupEvent;
        public static IResolveEvent EndResolvingEvent;
        public static IResolveEvent EndSerializingEvent;
        public static IResolveEvent EndSetupEvent;
        public static IResolveEvent EndUpdatingEvent;
        public static IResolveEvent InitializeAggregateEvent;
        public static IResolveEvent InitializePersistentEvent;
        public static IResolveEvent InitializeTransientEvent;
        public static IResolveEvent ResetEvent;
        public static IResolveEvent StartPartResolvingEvent;
        public static IResolveEvent StartPartSetupEvent;
        public static IResolveEvent StartResolvingEvent;
        public static IResolveEvent StartSerializingEvent;
        public static IResolveEvent StartSetupEvent;
        public static IResolveEvent StartUpdatingEvent;

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