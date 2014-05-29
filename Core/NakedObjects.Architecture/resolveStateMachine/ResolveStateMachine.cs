// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Util;

namespace NakedObjects.Architecture.Resolve {
    public class ResolveStateMachine : IResolveStateMachine {
        #region Delegates

        public delegate IResolveState EventHandler(INakedObject no, IResolveStateMachine rsm);

        #endregion

        #region Events

        #region Nested type: DestroyEvent

        internal class DestroyEvent : IResolveEvent {}

        #endregion

        #region Nested type: EndPartResolvingEvent

        internal class EndPartResolvingEvent : IResolveEvent {}

        #endregion

        #region Nested type: EndPartSetupEvent

        internal class EndPartSetupEvent : IResolveEvent {}

        #endregion

        #region Nested type: EndResolvingEvent

        internal class EndResolvingEvent : IResolveEvent {}

        #endregion

        #region Nested type: EndSerializingEvent

        internal class EndSerializingEvent : IResolveEvent {}

        #endregion

        #region Nested type: EndSetupEvent

        internal class EndSetupEvent : IResolveEvent {}

        #endregion

        #region Nested type: EndUpdatingEvent

        internal class EndUpdatingEvent : IResolveEvent {}

        #endregion

        #region Nested type: InitializeAggregateEvent

        internal class InitializeAggregateEvent : IResolveEvent {}

        #endregion

        #region Nested type: InitializePersistentEvent

        internal class InitializePersistentEvent : IResolveEvent {}

        #endregion

        #region Nested type: InitializeTransientEvent

        internal class InitializeTransientEvent : IResolveEvent {}

        #endregion

        #region Nested type: ResetEvent

        internal class ResetEvent : IResolveEvent {}

        #endregion

        #region Nested type: StartPartResolvingEvent

        internal class StartPartResolvingEvent : IResolveEvent {}

        #endregion

        #region Nested type: StartPartSetupEvent

        internal class StartPartSetupEvent : IResolveEvent {}

        #endregion

        #region Nested type: StartResolvingEvent

        internal class StartResolvingEvent : IResolveEvent {}

        #endregion

        #region Nested type: StartSerializingEvent

        internal class StartSerializingEvent : IResolveEvent {}

        #endregion

        #region Nested type: StartSetupEvent

        internal class StartSetupEvent : IResolveEvent {}

        #endregion

        #region Nested type: StartUpdatingEvent

        internal class StartUpdatingEvent : IResolveEvent {}

        #endregion

        #endregion

        #region States

        #region Nested type: AggregatedState

        internal class AggregatedState : ResolveState, IResolveState {
            #region IResolveState Members

            public override string Name {
                get { return "Aggregated"; }
            }

            public override string Code {
                get { return "A"; }
            }

            #endregion
        }

        #endregion

        #region Nested type: DestroyedState

        internal class DestroyedState : ResolveState, IResolveState {
            #region IResolveState Members

            public DestroyedState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Destroyed"; }
            }

            public override string Code {
                get { return "D"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndResolvingEvent] = (no, rsm) => this;
            }
        }

        #endregion

        #region Nested type: GhostState

        internal class GhostState : ResolveState, IResolveState {
            #region IResolveState Members

            public GhostState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Ghost"; }
            }

            public override string Code {
                get { return "PG"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.DestroyEvent] = (no, rsm) => States.DestroyedState;
                EventMap[Events.StartPartResolvingEvent] = (no, rsm) => States.ResolvingPartState;
                EventMap[Events.StartResolvingEvent] = (no, rsm) => {
                    Loading(no, rsm);
                    return States.ResolvingState;
                };
                EventMap[Events.StartUpdatingEvent] = (no, rsm) => States.UpdatingState;
                EventMap[Events.StartSerializingEvent] = (no, rsm) => States.SerializingGhostState;
                EventMap[Events.StartSetupEvent] = (no, rsm) => States.ResolvingState;
                EventMap[Events.StartPartSetupEvent] = (no, rsm) => States.ResolvingPartState;
            }
        }

        #endregion

        #region Nested type: NewState

        internal class NewState : ResolveState, IResolveState {
            #region IResolveState Members

            public NewState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "New"; }
            }

            public override string Code {
                get { return "N"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.InitializeTransientEvent] = (no, rsm) => States.TransientState;
                EventMap[Events.InitializePersistentEvent] = (no, rsm) => States.GhostState;
                EventMap[Events.InitializeAggregateEvent] = (no, rsm) => {
                    Loading(no, rsm);
                    Loaded(no, rsm);
                    return States.ResolvedState;
                };
            }
        }

        #endregion

        #region Nested type: PartResolvedState

        internal class PartResolvedState : ResolveState, IResolveState {
            #region IResolveState Members

            public PartResolvedState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Part Resolved"; }
            }

            public override string Code {
                get { return "Pr"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.DestroyEvent] = (no, rsm) => States.DestroyedState;
                EventMap[Events.StartPartResolvingEvent] = (no, rsm) => States.ResolvingPartState;
                EventMap[Events.StartResolvingEvent] = (no, rsm) => States.ResolvingState;
                EventMap[Events.StartUpdatingEvent] = (no, rsm) => States.UpdatingState;
                EventMap[Events.StartSerializingEvent] = (no, rsm) => States.SerializingPartResolvedState;
                EventMap[Events.StartSetupEvent] = (no, rsm) => States.ResolvingState;
                EventMap[Events.StartPartSetupEvent] = (no, rsm) => States.ResolvingPartState;
            }
        }

        #endregion

        #region Nested type: ResolvedState

        internal class ResolvedState : ResolveState, IResolveState {
            #region IResolveState Members

            public ResolvedState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Resolved"; }
            }

            public override string Code {
                get { return "PR"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.DestroyEvent] = (no, rsm) => States.DestroyedState;
                EventMap[Events.StartUpdatingEvent] = (no, rsm) => States.UpdatingState;
                EventMap[Events.StartSerializingEvent] = (no, rsm) => States.SerializingResolvedState;
                EventMap[Events.ResetEvent] = (no, rsm) => States.GhostState;
                EventMap[Events.StartSetupEvent] = (no, rsm) => States.UpdatingState;
                EventMap[Events.StartPartSetupEvent] = (no, rsm) => States.UpdatingState;
            }
        }

        #endregion

        #region Nested type: ResolveState

        internal abstract class ResolveState {
            protected readonly IDictionary<IResolveEvent, EventHandler> eventMap;

            protected ResolveState() {
                eventMap = new Dictionary<IResolveEvent, EventHandler>();
            }

            public abstract string Name { get; }
            public abstract string Code { get; }

            protected IDictionary<IResolveEvent, EventHandler> EventMap {
                get { return eventMap; }
            }

            protected virtual void Loading(INakedObject no, IResolveStateMachine rsm) {
                no.Loading();
                rsm.AddHistoryNote("Loading");
            }

            protected virtual void Loaded(INakedObject no, IResolveStateMachine rsm) {
                no.Loaded();
                rsm.AddHistoryNote("Loaded");
            }

            public override string ToString() {
                return string.Format("ResolveState [name={0},code={1}]", Name, Code);
            }

            public IResolveState Handle(IResolveEvent rEvent, INakedObject owner, IResolveStateMachine rsm) {
                if (EventMap.ContainsKey(rEvent)) {
                    return EventMap[rEvent](owner, rsm);
                }
                throw new ResolveException(string.Format("Unknown event {0} in state {1}", rEvent, this));
            }
        }

        #endregion

        #region Nested type: ResolvingPartState

        internal class ResolvingPartState : ResolveState, IResolveState {
            public ResolvingPartState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Resolving Part"; }
            }

            public override string Code {
                get { return "P~r"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndPartResolvingEvent] = (no, rsm) => States.PartResolvedState;
                EventMap[Events.EndResolvingEvent] = (no, rsm) => States.ResolvedState;
                EventMap[Events.EndSetupEvent] = (no, rsm) => States.PartResolvedState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm) => States.PartResolvedState;
            }
        }

        #endregion

        #region Nested type: ResolvingState

        internal class ResolvingState : ResolveState, IResolveState {
            #region IResolveState Members

            public ResolvingState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Resolving"; }
            }

            public override string Code {
                get { return "P~R"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndResolvingEvent] = (no, rsm) => {
                    Loaded(no, rsm);
                    return States.ResolvedState;
                };
                EventMap[Events.EndSetupEvent] = (no, rsm) => States.ResolvedState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm) => States.ResolvedState;
                EventMap[Events.DestroyEvent] = (no, rsm) => States.DestroyedState;
            }
        }

        #endregion

        #region Nested type: SerializingGhostState

        internal class SerializingGhostState : ResolveState, IResolveState {
            #region IResolveState Members

            public SerializingGhostState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Serializing Resolved"; } // not sure this is right for compatibility with old code 
            }

            public override string Code {
                get { return "SG"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm) => States.GhostState;
            }
        }

        #endregion

        #region Nested type: SerializingPartResolvedState

        internal class SerializingPartResolvedState : ResolveState, IResolveState {
            #region IResolveState Members

            public SerializingPartResolvedState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Serializing Part Resolved"; }
            }

            public override string Code {
                get { return "Sr"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm) => States.PartResolvedState;
            }
        }

        #endregion

        #region Nested type: SerializingResolvedState

        internal class SerializingResolvedState : ResolveState, IResolveState {
            #region IResolveState Members

            public SerializingResolvedState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Serializing Resolved"; }
            }

            public override string Code {
                get { return "SR"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm) => States.ResolvedState;
            }
        }

        #endregion

        #region Nested type: SerializingTransientState

        internal class SerializingTransientState : ResolveState, IResolveState {
            #region IResolveState Members

            public SerializingTransientState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Serializing Transient"; }
            }

            public override string Code {
                get { return "ST"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm) => States.TransientState;
                EventMap[Events.EndSetupEvent] = (no, rsm) => States.TransientState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm) => States.TransientState;
            }
        }

        #endregion

        #region Nested type: TransientState

        internal class TransientState : ResolveState, IResolveState {
            #region IResolveState Members

            public TransientState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Transient"; }
            }

            public override string Code {
                get { return "T"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.StartResolvingEvent] = (no, rsm) => States.ResolvingState;
                EventMap[Events.StartSerializingEvent] = (no, rsm) => States.SerializingTransientState;
                EventMap[Events.StartSetupEvent] = (no, rsm) => States.SerializingTransientState;
                EventMap[Events.StartPartSetupEvent] = (no, rsm) => States.SerializingTransientState;
            }
        }

        #endregion

        #region Nested type: UpdatingState

        internal class UpdatingState : ResolveState, IResolveState {
            #region IResolveState Members

            public UpdatingState() {
                InitialiseEventMap();
            }

            public override string Name {
                get { return "Updating"; }
            }

            public override string Code {
                get { return "PU"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndUpdatingEvent] = (no, rsm) => States.ResolvedState;
                EventMap[Events.EndSetupEvent] = (no, rsm) => States.ResolvedState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm) => States.ResolvedState;
            }
        }

        #endregion

        #endregion

        private readonly List<HistoryEvent> history = new List<HistoryEvent>();

        public ResolveStateMachine(INakedObject owner) {
            CurrentState = States.NewState;
            Owner = owner;
        }

        private INakedObject Owner { get; set; }
        public bool FullTrace { get; set; }

        #region IResolveStateMachine Members

        public IResolveState CurrentState { get; private set; }

        public void Handle(IResolveEvent rEvent) {
            IResolveState newState = CurrentState.Handle(rEvent, Owner, this);
            history.Add(new HistoryEvent(CurrentState, newState, rEvent, FullTrace));
            CurrentState = newState;
        }

        public void AddHistoryNote(string note) {
            HistoryEvent lastEvent = history.LastOrDefault();
            if (lastEvent != null) {
                lastEvent.AddNote(note);
            }
        }

        #endregion

        public override string ToString() {
            return CurrentState.ToString();
        }

        #region Nested type: HistoryEvent

        private class HistoryEvent {
            public HistoryEvent(IResolveState startState, IResolveState endState, IResolveEvent rEvent, bool fullTrace) {
                StartState = startState;
                EndState = endState;
                Event = rEvent;
                TimeStamp = DateTime.Now;
                Notes = new List<string>();
                if (fullTrace) {
                    Trace = new StackTrace(2, true);
                }
            }

            private IList<string> Notes { get; set; }

            private IResolveState StartState { get; set; }
            private IResolveState EndState { get; set; }
            private IResolveEvent Event { get; set; }
            private DateTime TimeStamp { get; set; }
            private StackTrace Trace { get; set; }

            public void AddNote(string note) {
                Notes.Add(note);
            }

            public override string ToString() {
                string notes = Notes.Aggregate("", (s1, s2) => s1 + ":" + s2);
                return string.Format("Transition from: {0} to: {1} by: {2} at: {3} with: {4}", StartState, EndState, Event, TimeStamp, notes);
            }
        }

        #endregion
    }
}