// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Resolve {
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