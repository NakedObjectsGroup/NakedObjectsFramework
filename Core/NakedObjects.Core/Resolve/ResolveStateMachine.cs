// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Resolve;

[assembly: InternalsVisibleTo("NakedObjects.Core.Test")]

namespace NakedObjects.Core.Resolve {
    public class ResolveStateMachine : IResolveStateMachine {
        #region Delegates

        public delegate IResolveState EventHandler(INakedObject no, IResolveStateMachine rsm, ISession s);

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
            public DestroyedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Destroyed"; }
            }

            public override string Code {
                get { return "D"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndResolvingEvent] = (no, rsm, s) => this;
            }
        }

        #endregion

        #region Nested type: GhostState

        internal class GhostState : ResolveState, IResolveState {
            public GhostState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Ghost"; }
            }

            public override string Code {
                get { return "PG"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.DestroyEvent] = (no, rsm, s) => States.DestroyedState;
                EventMap[Events.StartPartResolvingEvent] = (no, rsm, s) => States.ResolvingPartState;
                EventMap[Events.StartResolvingEvent] = (no, rsm, s) => {
                    Loading(no, rsm, s);
                    return States.ResolvingState;
                };
                EventMap[Events.StartUpdatingEvent] = (no, rsm, s) => States.UpdatingState;
                EventMap[Events.StartSerializingEvent] = (no, rsm, s) => States.SerializingGhostState;
                EventMap[Events.StartSetupEvent] = (no, rsm, s) => States.ResolvingState;
                EventMap[Events.StartPartSetupEvent] = (no, rsm, s) => States.ResolvingPartState;
            }
        }

        #endregion

        #region Nested type: NewState

        internal class NewState : ResolveState, IResolveState {
            public NewState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "New"; }
            }

            public override string Code {
                get { return "N"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.InitializeTransientEvent] = (no, rsm, s) => States.TransientState;
                EventMap[Events.InitializePersistentEvent] = (no, rsm, s) => States.GhostState;
                EventMap[Events.InitializeAggregateEvent] = (no, rsm, s) => {
                    Loading(no, rsm, s);
                    Loaded(no, rsm, s);
                    return States.ResolvedState;
                };
            }
        }

        #endregion

        #region Nested type: PartResolvedState

        internal class PartResolvedState : ResolveState, IResolveState {
            public PartResolvedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Part Resolved"; }
            }

            public override string Code {
                get { return "Pr"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.DestroyEvent] = (no, rsm, s) => States.DestroyedState;
                EventMap[Events.StartPartResolvingEvent] = (no, rsm, s) => States.ResolvingPartState;
                EventMap[Events.StartResolvingEvent] = (no, rsm, s) => States.ResolvingState;
                EventMap[Events.StartUpdatingEvent] = (no, rsm, s) => States.UpdatingState;
                EventMap[Events.StartSerializingEvent] = (no, rsm, s) => States.SerializingPartResolvedState;
                EventMap[Events.StartSetupEvent] = (no, rsm, s) => States.ResolvingState;
                EventMap[Events.StartPartSetupEvent] = (no, rsm, s) => States.ResolvingPartState;
            }
        }

        #endregion

        #region Nested type: ResolveState

        internal abstract class ResolveState {
            private readonly IDictionary<IResolveEvent, EventHandler> eventMap;

            protected ResolveState() {
                eventMap = new Dictionary<IResolveEvent, EventHandler>();
            }

            public abstract string Name { get; }
            public abstract string Code { get; }

            protected IDictionary<IResolveEvent, EventHandler> EventMap {
                get { return eventMap; }
            }

            protected virtual void Loading(INakedObject no, IResolveStateMachine rsm, ISession s) {
                no.Loading();
                rsm.AddHistoryNote("Loading");
            }

            protected virtual void Loaded(INakedObject no, IResolveStateMachine rsm, ISession s) {
                no.Loaded();
                rsm.AddHistoryNote("Loaded");
            }

            public override string ToString() {
                return string.Format("ResolveState [name={0},code={1}]", Name, Code);
            }

            public IResolveState Handle(IResolveEvent rEvent, INakedObject owner, IResolveStateMachine rsm, ISession s) {
                if (EventMap.ContainsKey(rEvent)) {
                    return EventMap[rEvent](owner, rsm, s);
                }
                throw new ResolveException(string.Format("Unknown event {0} in state {1}", rEvent, this));
            }
        }

        #endregion

        #region Nested type: ResolvedState

        internal class ResolvedState : ResolveState, IResolveState {
            public ResolvedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Resolved"; }
            }

            public override string Code {
                get { return "PR"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.DestroyEvent] = (no, rsm, s) => States.DestroyedState;
                EventMap[Events.StartUpdatingEvent] = (no, rsm, s) => States.UpdatingState;
                EventMap[Events.StartSerializingEvent] = (no, rsm, s) => States.SerializingResolvedState;
                EventMap[Events.ResetEvent] = (no, rsm, s) => States.GhostState;
                EventMap[Events.StartSetupEvent] = (no, rsm, s) => States.UpdatingState;
                EventMap[Events.StartPartSetupEvent] = (no, rsm, s) => States.UpdatingState;
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
                EventMap[Events.EndPartResolvingEvent] = (no, rsm, s) => States.PartResolvedState;
                EventMap[Events.EndResolvingEvent] = (no, rsm, s) => States.ResolvedState;
                EventMap[Events.EndSetupEvent] = (no, rsm, s) => States.PartResolvedState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm, s) => States.PartResolvedState;
            }
        }

        #endregion

        #region Nested type: ResolvingState

        internal class ResolvingState : ResolveState, IResolveState {
            public ResolvingState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Resolving"; }
            }

            public override string Code {
                get { return "P~R"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndResolvingEvent] = (no, rsm, s) => {
                    Loaded(no, rsm, s);
                    return States.ResolvedState;
                };
                EventMap[Events.EndSetupEvent] = (no, rsm, s) => States.ResolvedState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm, s) => States.ResolvedState;
                EventMap[Events.DestroyEvent] = (no, rsm, s) => States.DestroyedState;
            }
        }

        #endregion

        #region Nested type: SerializingGhostState

        internal class SerializingGhostState : ResolveState, IResolveState {
            public SerializingGhostState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Serializing Resolved"; } // not sure this is right for compatibility with old code 
            }

            public override string Code {
                get { return "SG"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm, s) => States.GhostState;
            }
        }

        #endregion

        #region Nested type: SerializingPartResolvedState

        internal class SerializingPartResolvedState : ResolveState, IResolveState {
            public SerializingPartResolvedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Serializing Part Resolved"; }
            }

            public override string Code {
                get { return "Sr"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm, s) => States.PartResolvedState;
            }
        }

        #endregion

        #region Nested type: SerializingResolvedState

        internal class SerializingResolvedState : ResolveState, IResolveState {
            public SerializingResolvedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Serializing Resolved"; }
            }

            public override string Code {
                get { return "SR"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm, s) => States.ResolvedState;
            }
        }

        #endregion

        #region Nested type: SerializingTransientState

        internal class SerializingTransientState : ResolveState, IResolveState {
            public SerializingTransientState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Serializing Transient"; }
            }

            public override string Code {
                get { return "ST"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm, s) => States.TransientState;
                EventMap[Events.EndSetupEvent] = (no, rsm, s) => States.TransientState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm, s) => States.TransientState;
            }
        }

        #endregion

        #region Nested type: TransientState

        internal class TransientState : ResolveState, IResolveState {
            public TransientState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Transient"; }
            }

            public override string Code {
                get { return "T"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.StartResolvingEvent] = (no, rsm, s) => States.ResolvingState;
                EventMap[Events.StartSerializingEvent] = (no, rsm, s) => States.SerializingTransientState;
                EventMap[Events.StartSetupEvent] = (no, rsm, s) => States.SerializingTransientState;
                EventMap[Events.StartPartSetupEvent] = (no, rsm, s) => States.SerializingTransientState;
            }
        }

        #endregion

        #region Nested type: UpdatingState

        internal class UpdatingState : ResolveState, IResolveState {
            public UpdatingState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name {
                get { return "Updating"; }
            }

            public override string Code {
                get { return "PU"; }
            }

            #endregion

            protected void InitialiseEventMap() {
                EventMap[Events.EndUpdatingEvent] = (no, rsm, s) => States.ResolvedState;
                EventMap[Events.EndSetupEvent] = (no, rsm, s) => States.ResolvedState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm, s) => States.ResolvedState;
            }
        }

        #endregion

        #endregion

        private readonly List<HistoryEvent> history = new List<HistoryEvent>();

        public ResolveStateMachine(INakedObject owner, ISession session) {
            CurrentState = States.NewState;
            Owner = owner;
            Session = session;
        }

        private ISession Session { get; set; }

        private INakedObject Owner { get; set; }
        public bool FullTrace { get; set; }

        #region IResolveStateMachine Members

        public IResolveState CurrentState { get; private set; }

        public void Handle(IResolveEvent rEvent) {
            IResolveState newState = CurrentState.Handle(rEvent, Owner, this, Session);
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