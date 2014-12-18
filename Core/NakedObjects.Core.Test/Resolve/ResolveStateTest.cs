// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;

namespace NakedObjects.Core.Test.Resolve {
    [TestClass]
    public class ResolveStateTest {
        private static void ExpectException(Action x) {
            try {
                x();
                Assert.Fail();
            }
            catch (Exception) {
                Assert.IsTrue(true);
            }
        }

        private static void ExpectNoException(Action x) {
            try {
                x();
            }
            catch (Exception) {
                Assert.Fail();
            }
        }

        public interface ITestCallbackFacet : ILoadingCallbackFacet, ILoadedCallbackFacet {}


        private static IResolveStateMachine NewSm() {
            var mockAdapter = new Mock<INakedObject>();
            var testAdapter = mockAdapter.Object;

            var mockSpecification = new Mock<IObjectSpec>();
            var testSpecification = mockSpecification.Object;

            var mockFacet = new Mock<ITestCallbackFacet>();
            var testFacet = mockFacet.Object;

            mockFacet.Setup(f => f.Invoke(null, null, null, null));

            mockAdapter.Setup(a => a.Spec).Returns(testSpecification);

            mockSpecification.Setup(s => s.GetFacet(null)).Returns(testFacet);
            mockSpecification.Setup(s => s.GetFacet<ILoadingCallbackFacet>()).Returns(testFacet);
            mockSpecification.Setup(s => s.GetFacet<ILoadedCallbackFacet>()).Returns(testFacet);

            return new ResolveStateMachine(testAdapter, null);
        }

        private static IResolveStateMachine GhostSM() {
            IResolveStateMachine sm = NewSm();
            sm.Handle(Events.InitializePersistentEvent);
            return sm;
        }

        private static IResolveStateMachine TransientSm() {
            IResolveStateMachine sm = NewSm();
            sm.Handle(Events.InitializeTransientEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvingPartSm() {
            IResolveStateMachine sm = GhostSM();
            sm.Handle(Events.StartPartResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvedPartSm() {
            IResolveStateMachine sm = ResolvingPartSm();
            sm.Handle(Events.EndPartResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvingSm() {
            IResolveStateMachine sm = GhostSM();
            sm.Handle(Events.StartResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvedSm() {
            IResolveStateMachine sm = ResolvingSm();
            sm.Handle(Events.EndResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine UpdatingSm() {
            IResolveStateMachine sm = ResolvedSm();
            sm.Handle(Events.StartUpdatingEvent);
            return sm;
        }

        private static IResolveStateMachine SerializingPartResolvedSm() {
            IResolveStateMachine sm = ResolvedPartSm();
            sm.Handle(Events.StartSerializingEvent);
            return sm;
        }

        private static IResolveStateMachine SerializingTransientSm() {
            IResolveStateMachine sm = TransientSm();
            sm.Handle(Events.StartSerializingEvent);
            return sm;
        }

        private static IResolveStateMachine SerializingResolvedSm() {
            IResolveStateMachine sm = ResolvedSm();
            sm.Handle(Events.StartSerializingEvent);
            return sm;
        }

        [TestMethod]
        public void InvalidChangesFromGhost() {
            ExpectException(() => GhostSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => GhostSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => GhostSM().Handle(Events.EndResolvingEvent));
            ExpectException(() => GhostSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => GhostSM().Handle(Events.StartSerializingEvent));
            ExpectException(() => GhostSM().Handle(Events.InitializeAggregateEvent));
        }

        [TestMethod]
        public void InvalidChangesFromNew() {
            ExpectException(() => NewSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => NewSm().Handle(Events.EndResolvingEvent));
            ExpectException(() => NewSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => NewSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => NewSm().Handle(Events.DestroyEvent));
            ExpectException(() => NewSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => NewSm().Handle(Events.StartSerializingEvent));
        }

        [TestMethod]
        public void InvalidChangesFromPartResolved() {
            ExpectException(() => ResolvedPartSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => ResolvedPartSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => ResolvedPartSm().Handle(Events.EndResolvingEvent));
            ExpectException(() => ResolvedPartSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => ResolvedPartSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvedPartSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvedPartSm().Handle(Events.StartSerializingEvent));
        }

        [TestMethod]
        public void InvalidChangesFromResolved() {
            ExpectException(() => ResolvedSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.EndResolvingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => ResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.StartSerializingEvent));
        }

        [TestMethod]
        public void InvalidChangesFromResolving() {
            ExpectException(() => ResolvingSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => ResolvingSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => ResolvingSm().Handle(Events.DestroyEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.InitializeAggregateEvent));
        }

        [TestMethod]
        public void InvalidChangesFromResolvingPart() {
            ExpectException(() => ResolvingPartSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.DestroyEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingPartSm().Handle(Events.InitializeAggregateEvent));
        }

        [TestMethod]
        public void InvalidChangesFromSerializingPartResolved() {
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.EndResolvingEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.DestroyEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingPartResolvedSm().Handle(Events.InitializeAggregateEvent));
        }

        [TestMethod]
        public void InvalidChangesFromSerializingResolved() {
            ExpectException(() => SerializingResolvedSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.DestroyEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingResolvedSm().Handle(Events.InitializeAggregateEvent));
        }

        [TestMethod]
        public void InvalidChangesFromSerializingTransient() {
            ExpectException(() => SerializingTransientSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.EndResolvingEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.DestroyEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => SerializingTransientSm().Handle(Events.InitializeAggregateEvent));
        }

        [TestMethod]
        public void InvalidChangesFromTransient() {
            ExpectException(() => TransientSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => TransientSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => TransientSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => TransientSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => TransientSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => TransientSm().Handle(Events.DestroyEvent));
            ExpectException(() => TransientSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => TransientSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => TransientSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => TransientSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => TransientSm().Handle(Events.InitializeAggregateEvent));
        }

        [TestMethod]
        public void InvalidChangesFromUpdating() {
            ExpectException(() => UpdatingSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => UpdatingSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => UpdatingSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => UpdatingSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => UpdatingSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => UpdatingSm().Handle(Events.DestroyEvent));
            ExpectException(() => UpdatingSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => UpdatingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => UpdatingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => UpdatingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => UpdatingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => UpdatingSm().Handle(Events.InitializeAggregateEvent));
        }

        [TestMethod]
        public void ValidChangesFromGhost() {
            ExpectNoException(() => GhostSM().Handle(Events.StartResolvingEvent));
            ExpectNoException(() => GhostSM().Handle(Events.StartPartResolvingEvent));
            ExpectNoException(() => GhostSM().Handle(Events.DestroyEvent));
            ExpectNoException(() => GhostSM().Handle(Events.StartUpdatingEvent));
            ExpectNoException(() => GhostSM().Handle(Events.StartSerializingEvent));
        }

        [TestMethod]
        public void ValidChangesFromNew() {
            ExpectNoException(() => NewSm().Handle(Events.InitializePersistentEvent));
            ExpectNoException(() => NewSm().Handle(Events.InitializeTransientEvent));
            ExpectNoException(() => NewSm().Handle(Events.InitializeAggregateEvent));
        }

        [TestMethod]
        public void ValidChangesFromPartResolved() {
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartResolvingEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartPartResolvingEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.DestroyEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartUpdatingEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartSerializingEvent));
        }

        [TestMethod]
        public void ValidChangesFromResolved() {
            ExpectNoException(() => ResolvedSm().Handle(Events.ResetEvent));
            ExpectNoException(() => ResolvedSm().Handle(Events.DestroyEvent));
            ExpectNoException(() => ResolvedSm().Handle(Events.StartUpdatingEvent));
            ExpectNoException(() => ResolvedSm().Handle(Events.StartSerializingEvent));
        }

        [TestMethod]
        public void ValidChangesFromResolving() {
            ExpectNoException(() => ResolvingSm().Handle(Events.EndResolvingEvent));
        }

        [TestMethod]
        public void ValidChangesFromResolvingPart() {
            ExpectNoException(() => ResolvingPartSm().Handle(Events.EndPartResolvingEvent));
            ExpectNoException(() => ResolvingPartSm().Handle(Events.EndResolvingEvent));
        }

        [TestMethod]
        public void ValidChangesFromSerializingPartResolved() {
            ExpectNoException(() => SerializingPartResolvedSm().Handle(Events.EndSerializingEvent));
        }

        [TestMethod]
        public void ValidChangesFromSerializingResolved() {
            ExpectNoException(() => SerializingResolvedSm().Handle(Events.EndSerializingEvent));
        }

        [TestMethod]
        public void ValidChangesFromSerializingTransient() {
            ExpectNoException(() => SerializingTransientSm().Handle(Events.EndSerializingEvent));
        }

        [TestMethod]
        public void ValidChangesFromTransient() {
            ExpectNoException(() => TransientSm().Handle(Events.StartResolvingEvent));
            ExpectNoException(() => TransientSm().Handle(Events.StartSerializingEvent));
        }

        [TestMethod]
        public void ValidChangesFromUpdating() {
            ExpectNoException(() => UpdatingSm().Handle(Events.EndUpdatingEvent));
        }
    }
}