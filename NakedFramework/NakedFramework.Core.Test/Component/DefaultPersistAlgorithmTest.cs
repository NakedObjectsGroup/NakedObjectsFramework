// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Component;
using NakedObjects.Core.Resolve;
using NUnit.Framework;

namespace NakedObjects.Core.Test.Component {
    /// <summary>
    ///     Summary description for DefaultPersistAlgorithmTest
    /// </summary>
    [TestFixture]
    public class DefaultPersistAlgorithmTest {
        private RecursivePersistAlgorithm algorithm;
        private INakedObjectManager manager;
        private Mock<INakedObjectManager> mockManager;
        private Mock<IObjectPersistor> mockPersistor;
        private IObjectPersistor persistor;

        #region Setup/Teardown

        [SetUp]
        public void SetUp() {
            mockPersistor = new Mock<IObjectPersistor>();
            persistor = mockPersistor.Object;

            mockManager = new Mock<INakedObjectManager>();
            manager = mockManager.Object;
            var mockLogger = new Mock<ILogger<RecursivePersistAlgorithm>>();

            algorithm = new RecursivePersistAlgorithm(persistor, manager, mockLogger.Object);
        }

        #endregion

        [Test]
        public void TestMakePersistent() {
            var mockAdapter = new Mock<INakedObjectAdapter>();
            var testAdapter = mockAdapter.Object;

            var mockSpec = new Mock<IObjectSpec>();
            var mockState = new Mock<IResolveStateMachine>();

            var mockASpec = new Mock<IAssociationSpec>();
            var mockCallBack = new Mock<IPersistingCallbackFacet>();

            mockState.Setup(s => s.CurrentState).Returns(new ResolveStateMachine.TransientState());

            mockSpec.Setup(s => s.Properties).Returns(new[] {mockASpec.Object});
            mockSpec.Setup(s => s.GetFacet<IPersistingCallbackFacet>()).Returns(mockCallBack.Object);

            mockASpec.Setup(s => s.IsPersisted).Returns(true);

            mockAdapter.Setup(a => a.Spec).Returns(mockSpec.Object);
            mockAdapter.Setup(a => a.ResolveState).Returns(mockState.Object);

            algorithm.MakePersistent(testAdapter);

            mockPersistor.Verify(p => p.AddPersistedObject(testAdapter));
            mockASpec.Verify(s => s.GetNakedObject(testAdapter));
        }

        [Test]
        public void TestMakePersistentFailsIfObjectAlreadyPersistent() {
            var mockAdapter = new Mock<INakedObjectAdapter>();
            var testAdapter = mockAdapter.Object;

            var mockSpec = new Mock<IObjectSpec>();
            var mockState = new Mock<IResolveStateMachine>();

            mockState.Setup(s => s.CurrentState).Returns(new ResolveStateMachine.ResolvedState());

            mockAdapter.Setup(a => a.Spec).Returns(mockSpec.Object);
            mockAdapter.Setup(a => a.ResolveState).Returns(mockState.Object);

            try {
                algorithm.MakePersistent(testAdapter);
                Assert.Fail("Expect exception");
            }
            catch (NotPersistableException /*expected*/) { }
        }

        [Test]
        public void TestMakePersistentFailsIfObjectMustBeTransient() {
            var mockAdapter = new Mock<INakedObjectAdapter>();
            var testAdapter = mockAdapter.Object;

            var mockSpec = new Mock<IObjectSpec>();
            var mockState = new Mock<IResolveStateMachine>();

            mockState.Setup(s => s.CurrentState).Returns(new ResolveStateMachine.TransientState());
            mockSpec.Setup(s => s.Persistable).Returns(PersistableType.Transient);

            mockAdapter.Setup(a => a.Spec).Returns(mockSpec.Object);
            mockAdapter.Setup(a => a.ResolveState).Returns(mockState.Object);

            try {
                algorithm.MakePersistent(testAdapter);
                Assert.Fail("Expect exception");
            }
            catch (NotPersistableException /*expected*/) { }
        }

        [Test]
        public void TestMakePersistentSkipsAggregatedObjects() {
            var mockAdapter = new Mock<INakedObjectAdapter>();
            var testAdapter = mockAdapter.Object;

            var mockSpec = new Mock<IObjectSpec>();
            var mockState = new Mock<IResolveStateMachine>();

            var mockASpec = new Mock<IAssociationSpec>();
            var mockCallBack = new Mock<IPersistingCallbackFacet>();

            mockState.Setup(s => s.CurrentState).Returns(new ResolveStateMachine.AggregatedState());

            mockSpec.Setup(s => s.Properties).Returns(new[] {mockASpec.Object});
            mockSpec.Setup(s => s.GetFacet<IPersistingCallbackFacet>()).Returns(mockCallBack.Object);

            mockASpec.Setup(s => s.IsPersisted).Returns(true);

            mockAdapter.Setup(a => a.Spec).Returns(mockSpec.Object);
            mockAdapter.Setup(a => a.ResolveState).Returns(mockState.Object);

            algorithm.MakePersistent(testAdapter);

            mockPersistor.Verify(p => p.AddPersistedObject(testAdapter));
            mockASpec.Verify(s => s.GetNakedObject(testAdapter));
        }

        [Test]
        public void TestMakePersistentSkipsAlreadyPersistedObjects() {
            var mockAdapter = new Mock<INakedObjectAdapter>();
            var testAdapter = mockAdapter.Object;

            var mockSpec = new Mock<IObjectSpec>();
            var mockState = new Mock<IResolveStateMachine>();

            var mockASpec = new Mock<IAssociationSpec>();
            var mockCallBack = new Mock<IPersistingCallbackFacet>();

            mockState.Setup(s => s.CurrentState).Returns(new ResolveStateMachine.AggregatedState());

            mockSpec.Setup(s => s.Properties).Returns(new[] {mockASpec.Object});
            mockSpec.Setup(s => s.GetFacet<IPersistingCallbackFacet>()).Returns(mockCallBack.Object);

            mockASpec.Setup(s => s.IsPersisted).Returns(false);

            mockAdapter.Setup(a => a.Spec).Returns(mockSpec.Object);
            mockAdapter.Setup(a => a.ResolveState).Returns(mockState.Object);

            algorithm.MakePersistent(testAdapter);

            mockPersistor.Verify(p => p.AddPersistedObject(testAdapter));
            mockASpec.Verify(s => s.GetNakedObject(testAdapter), Times.Never);
        }
    }
}