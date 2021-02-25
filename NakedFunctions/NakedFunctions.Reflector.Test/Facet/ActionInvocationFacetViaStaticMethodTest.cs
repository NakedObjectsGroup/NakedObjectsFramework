// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFunctions.Reflector.Facet;
using NakedObjects;

namespace NakedFunctions.Reflector.Test.Facet {
    [TestClass]
    public class ActionInvocationFacetViaStaticMethodTest {
        private static readonly object TestValue = new();
        private readonly Mock<INakedObjectsFramework> mockFramework = new();

        private readonly Mock<INakedObjectManager> mockNakedObjectManager = new();

        private readonly Mock<IObjectPersistor> mockPersistor = new();

        public ActionInvocationFacetViaStaticMethodTest() {
            mockFramework.SetupGet(p => p.NakedObjectManager).Returns(mockNakedObjectManager.Object);
            mockFramework.SetupGet(p => p.Persistor).Returns(mockPersistor.Object);
        }

        [TestMethod]
        public void TestInvoke() {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod1));
            var testFacet = new ActionInvocationFacetViaStaticMethod(method, null, null, null, null, false, null);

            var result = testFacet.Invoke(null, Array.Empty<INakedObjectAdapter>(), mockFramework.Object);
        }

        public static class TestClass {
            public static object TestMethod1() => TestValue;
        }
    }
}