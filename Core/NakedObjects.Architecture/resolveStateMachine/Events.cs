// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Resolve {
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