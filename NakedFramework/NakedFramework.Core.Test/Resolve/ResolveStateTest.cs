// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Resolve;
using NakedFramework.Architecture.Spec;
using NakedObjects.Architecture.Facet;
using NakedObjects.Core.Resolve;
using NUnit.Framework;

namespace NakedObjects.Core.Test.Resolve {
    [TestFixture]
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

        private static IResolveStateMachine NewSm() {
            var mockAdapter = new Mock<INakedObjectAdapter>();
            var testAdapter = mockAdapter.Object;

            var mockSpecification = new Mock<IObjectSpec>();
            var testSpecification = mockSpecification.Object;

            var mockFacet = new Mock<ITestCallbackFacet>();
            var testFacet = mockFacet.Object;

            mockFacet.Setup(f => f.Invoke(null, null));

            mockAdapter.Setup(a => a.Spec).Returns(testSpecification);

            mockSpecification.Setup(s => s.GetFacet(null)).Returns(testFacet);
            mockSpecification.Setup(s => s.GetFacet<ILoadingCallbackFacet>()).Returns(testFacet);
            mockSpecification.Setup(s => s.GetFacet<ILoadedCallbackFacet>()).Returns(testFacet);

            return new ResolveStateMachine(testAdapter, null);
        }

        private static IResolveStateMachine GhostSM() {
            var sm = NewSm();
            sm.Handle(Events.InitializePersistentEvent);
            return sm;
        }

        private static IResolveStateMachine TransientSm() {
            var sm = NewSm();
            sm.Handle(Events.InitializeTransientEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvingPartSm() {
            var sm = GhostSM();
            sm.Handle(Events.StartPartResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvedPartSm() {
            var sm = ResolvingPartSm();
            sm.Handle(Events.EndPartResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvingSm() {
            var sm = GhostSM();
            sm.Handle(Events.StartResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine ResolvedSm() {
            var sm = ResolvingSm();
            sm.Handle(Events.EndResolvingEvent);
            return sm;
        }

        private static IResolveStateMachine UpdatingSm() {
            var sm = ResolvedSm();
            sm.Handle(Events.StartUpdatingEvent);
            return sm;
        }

        private static IResolveStateMachine SerializingPartResolvedSm() {
            var sm = ResolvedPartSm();
            sm.Handle(Events.StartSerializingEvent);
            return sm;
        }

        private static IResolveStateMachine SerializingTransientSm() {
            var sm = TransientSm();
            sm.Handle(Events.StartSerializingEvent);
            return sm;
        }

        private static IResolveStateMachine SerializingResolvedSm() {
            var sm = ResolvedSm();
            sm.Handle(Events.StartSerializingEvent);
            return sm;
        }

        [Test]
        public void InvalidChangesFromGhost() {
            ExpectException(() => GhostSM().Handle(Events.InitializePersistentEvent));
            ExpectException(() => GhostSM().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => GhostSM().Handle(Events.EndResolvingEvent));
            ExpectException(() => GhostSM().Handle(Events.InitializeTransientEvent));
            ExpectException(() => GhostSM().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void InvalidChangesFromNew() {
            ExpectException(() => NewSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => NewSm().Handle(Events.EndResolvingEvent));
            ExpectException(() => NewSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => NewSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => NewSm().Handle(Events.DestroyEvent));
            ExpectException(() => NewSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => NewSm().Handle(Events.StartSerializingEvent));
        }

        [Test]
        public void InvalidChangesFromPartResolved() {
            ExpectException(() => ResolvedPartSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => ResolvedPartSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => ResolvedPartSm().Handle(Events.EndResolvingEvent));
            ExpectException(() => ResolvedPartSm().Handle(Events.InitializeTransientEvent));
        }

        [Test]
        public void InvalidChangesFromResolved() {
            ExpectException(() => ResolvedSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.EndResolvingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => ResolvedSm().Handle(Events.InitializeTransientEvent));
        }

        [Test]
        public void InvalidChangesFromResolving() {
            ExpectException(() => ResolvingSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => ResolvingSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartResolvingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.StartSerializingEvent));
            ExpectException(() => ResolvingSm().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void InvalidChangesFromTransient() {
            ExpectException(() => TransientSm().Handle(Events.InitializePersistentEvent));
            ExpectException(() => TransientSm().Handle(Events.EndPartResolvingEvent));
            ExpectException(() => TransientSm().Handle(Events.StartPartResolvingEvent));
            ExpectException(() => TransientSm().Handle(Events.InitializeTransientEvent));
            ExpectException(() => TransientSm().Handle(Events.DestroyEvent));
            ExpectException(() => TransientSm().Handle(Events.StartUpdatingEvent));
            ExpectException(() => TransientSm().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
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

        [Test]
        public void ValidChangesFromGhost() {
            ExpectNoException(() => GhostSM().Handle(Events.StartResolvingEvent));
            ExpectNoException(() => GhostSM().Handle(Events.StartPartResolvingEvent));
            ExpectNoException(() => GhostSM().Handle(Events.DestroyEvent));
            ExpectNoException(() => GhostSM().Handle(Events.StartUpdatingEvent));
            ExpectNoException(() => GhostSM().Handle(Events.StartSerializingEvent));
        }

        [Test]
        public void ValidChangesFromNew() {
            ExpectNoException(() => NewSm().Handle(Events.InitializePersistentEvent));
            ExpectNoException(() => NewSm().Handle(Events.InitializeTransientEvent));
            ExpectNoException(() => NewSm().Handle(Events.InitializeAggregateEvent));
        }

        [Test]
        public void ValidChangesFromPartResolved() {
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartResolvingEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartPartResolvingEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.DestroyEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartUpdatingEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartSerializingEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartSetupEvent));
            ExpectNoException(() => ResolvedPartSm().Handle(Events.StartPartSetupEvent));
        }

        [Test]
        public void ValidChangesFromResolved() {
            ExpectNoException(() => ResolvedSm().Handle(Events.ResetEvent));
            ExpectNoException(() => ResolvedSm().Handle(Events.DestroyEvent));
            ExpectNoException(() => ResolvedSm().Handle(Events.StartUpdatingEvent));
            ExpectNoException(() => ResolvedSm().Handle(Events.StartSerializingEvent));
            ExpectNoException(() => ResolvedSm().Handle(Events.StartSetupEvent));
            ExpectNoException(() => ResolvedSm().Handle(Events.StartPartSetupEvent));
        }

        [Test]
        public void ValidChangesFromResolving() {
            ExpectNoException(() => ResolvingSm().Handle(Events.EndResolvingEvent));
            ExpectNoException(() => ResolvingSm().Handle(Events.DestroyEvent));
        }

        [Test]
        public void ValidChangesFromResolvingPart() {
            ExpectNoException(() => ResolvingPartSm().Handle(Events.EndPartResolvingEvent));
            ExpectNoException(() => ResolvingPartSm().Handle(Events.EndResolvingEvent));
        }

        [Test]
        public void ValidChangesFromSerializingPartResolved() {
            ExpectNoException(() => SerializingPartResolvedSm().Handle(Events.EndSerializingEvent));
        }

        [Test]
        public void ValidChangesFromSerializingResolved() {
            ExpectNoException(() => SerializingResolvedSm().Handle(Events.EndSerializingEvent));
        }

        [Test]
        public void ValidChangesFromSerializingTransient() {
            ExpectNoException(() => SerializingTransientSm().Handle(Events.EndSerializingEvent));
        }

        [Test]
        public void ValidChangesFromTransient() {
            ExpectNoException(() => TransientSm().Handle(Events.StartResolvingEvent));
            ExpectNoException(() => TransientSm().Handle(Events.StartSerializingEvent));
            ExpectNoException(() => TransientSm().Handle(Events.StartSetupEvent));
            ExpectNoException(() => TransientSm().Handle(Events.StartPartSetupEvent));
        }

        [Test]
        public void ValidChangesFromUpdating() {
            ExpectNoException(() => UpdatingSm().Handle(Events.EndUpdatingEvent));
        }

        #region Nested type: ITestCallbackFacet

        public interface ITestCallbackFacet : ILoadingCallbackFacet, ILoadedCallbackFacet { }

        #endregion
    }
}