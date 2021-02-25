// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Resolve;

namespace NakedFramework.Core.Resolve {
    internal static class States {
        internal static readonly IResolveState AggregatedState;
        internal static readonly IResolveState DestroyedState;
        internal static readonly IResolveState GhostState;
        internal static readonly IResolveState NewState;
        internal static readonly IResolveState PartResolvedState;
        internal static readonly IResolveState ResolvedState;
        internal static readonly IResolveState ResolvingPartState;
        internal static readonly IResolveState ResolvingState;
        internal static readonly IResolveState SerializingGhostState;
        internal static readonly IResolveState SerializingPartResolvedState;
        internal static readonly IResolveState SerializingResolvedState;
        internal static readonly IResolveState SerializingTransientState;
        internal static readonly IResolveState TransientState;
        internal static readonly IResolveState UpdatingState;

        static States() {
            AggregatedState = new ResolveStateMachine.AggregatedState();
            DestroyedState = new ResolveStateMachine.DestroyedState();
            GhostState = new ResolveStateMachine.GhostState();
            NewState = new ResolveStateMachine.NewState();
            PartResolvedState = new ResolveStateMachine.PartResolvedState();
            ResolvedState = new ResolveStateMachine.ResolvedState();
            ResolvingPartState = new ResolveStateMachine.ResolvingPartState();
            ResolvingState = new ResolveStateMachine.ResolvingState();
            SerializingGhostState = new ResolveStateMachine.SerializingGhostState();
            SerializingPartResolvedState = new ResolveStateMachine.SerializingPartResolvedState();
            SerializingResolvedState = new ResolveStateMachine.SerializingResolvedState();
            SerializingTransientState = new ResolveStateMachine.SerializingTransientState();
            TransientState = new ResolveStateMachine.TransientState();
            UpdatingState = new ResolveStateMachine.UpdatingState();
        }
    }
}