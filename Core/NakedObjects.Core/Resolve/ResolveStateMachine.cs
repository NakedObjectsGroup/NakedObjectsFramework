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
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Core.Util;

[assembly: InternalsVisibleTo("NakedObjects.Core.Test")]

namespace NakedObjects.Core.Resolve {
    public sealed class ResolveStateMachine : IResolveStateMachine {
        #region Delegates

        public delegate IResolveState EventHandler(INakedObjectAdapter no, IResolveStateMachine rsm, ISession s);

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(ResolveStateMachine));

        private readonly List<HistoryEvent> history = new List<HistoryEvent>();

        public ResolveStateMachine(INakedObjectAdapter owner, ISession session) {
            CurrentState = States.NewState;
            Owner = owner;
            Session = session;
        }

        private ISession Session { get; }
        private INakedObjectAdapter Owner { get; }
        public bool FullTrace { get; set; }

        #region IResolveStateMachine Members

        public IResolveState CurrentState { get; private set; }

        public void Handle(IResolveEvent rEvent) {
            var newState = CurrentState.Handle(rEvent, Owner, this, Session);
            history.Add(new HistoryEvent(CurrentState, newState, rEvent, FullTrace));
            CurrentState = newState;
        }

        public void AddHistoryNote(string note) {
            var lastEvent = history.LastOrDefault();
            lastEvent?.AddNote(note);
        }

        #endregion

        public override string ToString() => CurrentState.ToString();

        #region Nested type: HistoryEvent

        private class HistoryEvent {
            // ReSharper disable once NotAccessedField.Local
            // for viewing via debugger
            private StackTrace trace;

            public HistoryEvent(IResolveState startState, IResolveState endState, IResolveEvent rEvent, bool fullTrace) {
                StartState = startState;
                EndState = endState;
                Event = rEvent;
                TimeStamp = DateTime.Now;
                Notes = new List<string>();
                if (fullTrace) {
                    trace = new StackTrace(2, true);
                }
            }

            private IList<string> Notes { get; }
            private IResolveState StartState { get; }
            private IResolveState EndState { get; }
            private IResolveEvent Event { get; }
            private DateTime TimeStamp { get; }

            public void AddNote(string note) {
                Notes.Add(note);
            }

            public override string ToString() {
                var notes = Notes.Aggregate("", (s1, s2) => s1 + ":" + s2);
                return $"Transition from: {StartState} to: {EndState} by: {Event} at: {TimeStamp} with: {notes}";
            }
        }

        #endregion

        #region Events

        #region Nested type: DestroyEvent

        public sealed class DestroyEvent : IResolveEvent { }

        #endregion

        #region Nested type: EndPartResolvingEvent

        public sealed class EndPartResolvingEvent : IResolveEvent { }

        #endregion

        #region Nested type: EndPartSetupEvent

        public sealed class EndPartSetupEvent : IResolveEvent { }

        #endregion

        #region Nested type: EndResolvingEvent

        public sealed class EndResolvingEvent : IResolveEvent { }

        #endregion

        #region Nested type: EndSerializingEvent

        public sealed class EndSerializingEvent : IResolveEvent { }

        #endregion

        #region Nested type: EndSetupEvent

        public sealed class EndSetupEvent : IResolveEvent { }

        #endregion

        #region Nested type: EndUpdatingEvent

        public sealed class EndUpdatingEvent : IResolveEvent { }

        #endregion

        #region Nested type: InitializeAggregateEvent

        public sealed class InitializeAggregateEvent : IResolveEvent { }

        #endregion

        #region Nested type: InitializePersistentEvent

        public sealed class InitializePersistentEvent : IResolveEvent { }

        #endregion

        #region Nested type: InitializeTransientEvent

        public sealed class InitializeTransientEvent : IResolveEvent { }

        #endregion

        #region Nested type: ResetEvent

        public sealed class ResetEvent : IResolveEvent { }

        #endregion

        #region Nested type: StartPartResolvingEvent

        public sealed class StartPartResolvingEvent : IResolveEvent { }

        #endregion

        #region Nested type: StartPartSetupEvent

        public sealed class StartPartSetupEvent : IResolveEvent { }

        #endregion

        #region Nested type: StartResolvingEvent

        public sealed class StartResolvingEvent : IResolveEvent { }

        #endregion

        #region Nested type: StartSerializingEvent

        public sealed class StartSerializingEvent : IResolveEvent { }

        #endregion

        #region Nested type: StartSetupEvent

        public sealed class StartSetupEvent : IResolveEvent { }

        #endregion

        #region Nested type: StartUpdatingEvent

        public sealed class StartUpdatingEvent : IResolveEvent { }

        #endregion

        #endregion

        #region States

        #region Nested type: AggregatedState

        public sealed class AggregatedState : ResolveState, IResolveState {
            #region IResolveState Members

            public override string Name => "Aggregated";

            public override string Code => "A";

            #endregion
        }

        #endregion

        #region Nested type: DestroyedState

        public sealed class DestroyedState : ResolveState, IResolveState {
            public DestroyedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Destroyed";

            public override string Code => "D";

            #endregion

            private void InitialiseEventMap() {
                EventMap[Events.EndResolvingEvent] = (no, rsm, s) => this;
            }
        }

        #endregion

        #region Nested type: GhostState

        public sealed class GhostState : ResolveState, IResolveState {
            public GhostState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Ghost";

            public override string Code => "PG";

            #endregion

            private void InitialiseEventMap() {
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

        public sealed class NewState : ResolveState, IResolveState {
            public NewState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "New";

            public override string Code => "N";

            #endregion

            private void InitialiseEventMap() {
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

        public sealed class PartResolvedState : ResolveState, IResolveState {
            public PartResolvedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Part Resolved";

            public override string Code => "Pr";

            #endregion

            private void InitialiseEventMap() {
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

        public abstract class ResolveState {
            protected ResolveState() => EventMap = new Dictionary<IResolveEvent, EventHandler>();

            public abstract string Name { get; }
            public abstract string Code { get; }

            protected IDictionary<IResolveEvent, EventHandler> EventMap { get; }

            protected virtual void Loading(INakedObjectAdapter no, IResolveStateMachine rsm, ISession s) {
                no.Loading();
                rsm.AddHistoryNote("Loading");
            }

            protected virtual void Loaded(INakedObjectAdapter no, IResolveStateMachine rsm, ISession s) {
                no.Loaded();
                rsm.AddHistoryNote("Loaded");
            }

            public override string ToString() => $"ResolveState [name={Name},code={Code}]";

            public IResolveState Handle(IResolveEvent rEvent, INakedObjectAdapter owner, IResolveStateMachine rsm, ISession s) {
                if (EventMap.ContainsKey(rEvent)) {
                    return EventMap[rEvent](owner, rsm, s);
                }

                throw new ResolveException(Log.LogAndReturn($"Unknown event {rEvent} in state {this}"));
            }
        }

        #endregion

        #region Nested type: ResolvedState

        public sealed class ResolvedState : ResolveState, IResolveState {
            public ResolvedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Resolved";

            public override string Code => "PR";

            #endregion

            private void InitialiseEventMap() {
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

        public sealed class ResolvingPartState : ResolveState, IResolveState {
            public ResolvingPartState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Resolving Part";

            public override string Code => "P~r";

            #endregion

            private void InitialiseEventMap() {
                EventMap[Events.EndPartResolvingEvent] = (no, rsm, s) => States.PartResolvedState;
                EventMap[Events.EndResolvingEvent] = (no, rsm, s) => States.ResolvedState;
                EventMap[Events.EndSetupEvent] = (no, rsm, s) => States.PartResolvedState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm, s) => States.PartResolvedState;
            }
        }

        #endregion

        #region Nested type: ResolvingState

        public sealed class ResolvingState : ResolveState, IResolveState {
            public ResolvingState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Resolving";

            public override string Code => "P~R";

            #endregion

            private void InitialiseEventMap() {
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

        public sealed class SerializingGhostState : ResolveState, IResolveState {
            public SerializingGhostState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Serializing Resolved";

            public override string Code => "SG";

            #endregion

            private void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm, s) => States.GhostState;
            }
        }

        #endregion

        #region Nested type: SerializingPartResolvedState

        public sealed class SerializingPartResolvedState : ResolveState, IResolveState {
            public SerializingPartResolvedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Serializing Part Resolved";

            public override string Code => "Sr";

            #endregion

            private void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm, s) => States.PartResolvedState;
            }
        }

        #endregion

        #region Nested type: SerializingResolvedState

        public sealed class SerializingResolvedState : ResolveState, IResolveState {
            public SerializingResolvedState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Serializing Resolved";

            public override string Code => "SR";

            #endregion

            private void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm, s) => States.ResolvedState;
            }
        }

        #endregion

        #region Nested type: SerializingTransientState

        public sealed class SerializingTransientState : ResolveState, IResolveState {
            public SerializingTransientState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Serializing Transient";

            public override string Code => "ST";

            #endregion

            private void InitialiseEventMap() {
                EventMap[Events.EndSerializingEvent] = (no, rsm, s) => States.TransientState;
                EventMap[Events.EndSetupEvent] = (no, rsm, s) => States.TransientState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm, s) => States.TransientState;
            }
        }

        #endregion

        #region Nested type: TransientState

        public sealed class TransientState : ResolveState, IResolveState {
            public TransientState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Transient";

            public override string Code => "T";

            #endregion

            private void InitialiseEventMap() {
                EventMap[Events.StartResolvingEvent] = (no, rsm, s) => States.ResolvingState;
                EventMap[Events.StartSerializingEvent] = (no, rsm, s) => States.SerializingTransientState;
                EventMap[Events.StartSetupEvent] = (no, rsm, s) => States.SerializingTransientState;
                EventMap[Events.StartPartSetupEvent] = (no, rsm, s) => States.SerializingTransientState;
            }
        }

        #endregion

        #region Nested type: UpdatingState

        public sealed class UpdatingState : ResolveState, IResolveState {
            public UpdatingState() {
                InitialiseEventMap();
            }

            #region IResolveState Members

            public override string Name => "Updating";

            public override string Code => "PU";

            #endregion

            private void InitialiseEventMap() {
                EventMap[Events.EndUpdatingEvent] = (no, rsm, s) => States.ResolvedState;
                EventMap[Events.EndSetupEvent] = (no, rsm, s) => States.ResolvedState;
                EventMap[Events.EndPartSetupEvent] = (no, rsm, s) => States.ResolvedState;
            }
        }

        #endregion

        #endregion
    }
}