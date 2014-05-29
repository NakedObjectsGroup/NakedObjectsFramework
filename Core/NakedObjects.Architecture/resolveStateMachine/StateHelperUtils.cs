// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Resolve {
    public static class StateHelperUtils {
        public static bool IsGhost(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.GhostState;
        }

        public static bool IsNew(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.NewState;
        }

        public static bool IsUpdating(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.UpdatingState;
        }

        public static bool IsPartlyResolved(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.PartResolvedState;
        }

        public static bool IsPersistent(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.GhostState ||
                   stateMachine.CurrentState is ResolveStateMachine.PartResolvedState ||
                   stateMachine.CurrentState is ResolveStateMachine.ResolvedState ||
                   stateMachine.CurrentState is ResolveStateMachine.ResolvingPartState ||
                   stateMachine.CurrentState is ResolveStateMachine.ResolvingState ||
                   stateMachine.CurrentState is ResolveStateMachine.SerializingGhostState ||
                   stateMachine.CurrentState is ResolveStateMachine.SerializingPartResolvedState ||
                   stateMachine.CurrentState is ResolveStateMachine.SerializingResolvedState ||
                   stateMachine.CurrentState is ResolveStateMachine.UpdatingState;
        }

        // not necessarily the same as IsTransient ! 
        public static bool IsNotPersistent(this IResolveStateMachine stateMachine) {
            return !stateMachine.IsPersistent();
        }

        public static bool IsDestroyed(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.DestroyedState;
        }

        public static bool IsResolved(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.ResolvedState;
        }

        public static bool IsAggregated(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.AggregatedState;
        }

        public static bool IsResolving(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.ResolvingState || stateMachine.CurrentState is ResolveStateMachine.ResolvingPartState;
        }

        public static bool IsPartResolving(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.ResolvingPartState;
        }

        public static bool IsSerializing(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.SerializingGhostState ||
                   stateMachine.CurrentState is ResolveStateMachine.SerializingPartResolvedState ||
                   stateMachine.CurrentState is ResolveStateMachine.SerializingResolvedState ||
                   stateMachine.CurrentState is ResolveStateMachine.SerializingTransientState;
        }

        public static bool IsTransient(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.TransientState ||
                   stateMachine.CurrentState is ResolveStateMachine.SerializingTransientState;
        }

        public static bool RespondToChangesInPersistentObjects(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.AggregatedState ||
                   stateMachine.CurrentState is ResolveStateMachine.DestroyedState ||
                   stateMachine.CurrentState is ResolveStateMachine.GhostState ||
                   stateMachine.CurrentState is ResolveStateMachine.PartResolvedState ||
                   stateMachine.CurrentState is ResolveStateMachine.ResolvedState;
        }

        public static bool IsResolvable(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.GhostState ||
                   stateMachine.CurrentState is ResolveStateMachine.PartResolvedState;
        }

        public static bool IsDeserializable(this IResolveStateMachine stateMachine) {
            return stateMachine.CurrentState is ResolveStateMachine.TransientState ||
                   stateMachine.CurrentState is ResolveStateMachine.GhostState ||
                   stateMachine.CurrentState is ResolveStateMachine.PartResolvedState ||
                   stateMachine.CurrentState is ResolveStateMachine.ResolvedState;
        }

        public static void CheckCanAssociate(this IResolveStateMachine stateMachine, INakedObject associate) {
            if (stateMachine.IsPersistent() && associate != null && associate.ResolveState.IsTransient()) {
                throw new TransientReferenceException(string.Format(Resources.NakedObjects.TransientErrorMessage, associate.TitleString()));
            }
        }
    }
}