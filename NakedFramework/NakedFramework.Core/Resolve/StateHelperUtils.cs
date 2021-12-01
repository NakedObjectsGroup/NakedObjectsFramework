// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Resolve;
using NakedFramework.Core.Error;

namespace NakedFramework.Core.Resolve; 

public static class StateHelperUtils {
    public static bool IsGhost(this IResolveStateMachine stateMachine) => stateMachine.CurrentState is ResolveStateMachine.GhostState;

    public static bool IsPersistent(this IResolveStateMachine stateMachine) =>
        stateMachine.CurrentState is ResolveStateMachine.GhostState ||
        stateMachine.CurrentState is ResolveStateMachine.PartResolvedState ||
        stateMachine.CurrentState is ResolveStateMachine.ResolvedState ||
        stateMachine.CurrentState is ResolveStateMachine.ResolvingPartState ||
        stateMachine.CurrentState is ResolveStateMachine.ResolvingState ||
        stateMachine.CurrentState is ResolveStateMachine.SerializingGhostState ||
        stateMachine.CurrentState is ResolveStateMachine.SerializingPartResolvedState ||
        stateMachine.CurrentState is ResolveStateMachine.SerializingResolvedState ||
        stateMachine.CurrentState is ResolveStateMachine.UpdatingState;

    // not necessarily the same as IsTransient ! 
    public static bool IsNotPersistent(this IResolveStateMachine stateMachine) => !stateMachine.IsPersistent();

    public static bool IsDestroyed(this IResolveStateMachine stateMachine) => stateMachine.CurrentState is ResolveStateMachine.DestroyedState;

    public static bool IsResolved(this IResolveStateMachine stateMachine) => stateMachine.CurrentState is ResolveStateMachine.ResolvedState;

    public static bool IsAggregated(this IResolveStateMachine stateMachine) => stateMachine.CurrentState is ResolveStateMachine.AggregatedState;

    public static bool IsPartResolving(this IResolveStateMachine stateMachine) => stateMachine.CurrentState is ResolveStateMachine.ResolvingPartState;

    public static bool IsTransient(this IResolveStateMachine stateMachine) =>
        stateMachine.CurrentState is ResolveStateMachine.TransientState ||
        stateMachine.CurrentState is ResolveStateMachine.SerializingTransientState;

    public static bool RespondToChangesInPersistentObjects(this IResolveStateMachine stateMachine) =>
        stateMachine.CurrentState is ResolveStateMachine.AggregatedState ||
        stateMachine.CurrentState is ResolveStateMachine.DestroyedState ||
        stateMachine.CurrentState is ResolveStateMachine.GhostState ||
        stateMachine.CurrentState is ResolveStateMachine.PartResolvedState ||
        stateMachine.CurrentState is ResolveStateMachine.ResolvedState;

    public static bool IsResolvable(this IResolveStateMachine stateMachine) =>
        stateMachine.CurrentState is ResolveStateMachine.GhostState ||
        stateMachine.CurrentState is ResolveStateMachine.PartResolvedState;

    public static void CheckCanAssociate(this IResolveStateMachine stateMachine, INakedObjectAdapter associate) {
        if (stateMachine.IsPersistent() && associate != null && associate.ResolveState.IsTransient()) {
            throw new TransientReferenceException(string.Format(NakedObjects.Resources.NakedObjects.TransientErrorMessage, associate.TitleString()));
        }
    }
}