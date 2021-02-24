// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFunctions.Meta.Facet;

namespace NakedFunctions.Reflector.Test.Facet {
    [TestClass]
    public class CallBackFacetsTest {
        private static readonly object TestValuePersisting = "Persisting";
        private static readonly object TestValuePersisted = "Persisted";
        private static readonly object TestValueUpdating = "Updating";
        private static readonly object TestValueUpdated = "Updated";

        [TestMethod]
        public void TestCallBackPersisting() {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.Persisting));
            var testFacet = new PersistingCallbackFacetViaFunction(method, null);

            var result = testFacet.InvokeAndReturn(null, null);

            Assert.AreEqual(TestValuePersisting, result);
        }

        [TestMethod]
        public void TestCallBackPersisted() {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.Persisted));
            var testFacet = new PersistedCallbackFacetViaFunction(method, null);

            var result = testFacet.InvokeAndReturn(null, null);

            Assert.AreEqual(TestValuePersisted, result);
        }

        [TestMethod]
        public void TestCallBackUpdating() {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.Updating));
            var testFacet = new UpdatingCallbackFacetViaFunction(method, null);

            var result = testFacet.InvokeAndReturn(null, null);

            Assert.AreEqual(TestValueUpdating, result);
        }

        [TestMethod]
        public void TestCallBackUpdated() {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.Updated));
            var testFacet = new UpdatedCallbackFacetViaFunction(method, null);

            var result = testFacet.InvokeAndReturn(null, null);

            Assert.AreEqual(TestValueUpdated, result);
        }

        public static class TestClass {
            public static object Persisting() => TestValuePersisting;
            public static object Persisted() => TestValuePersisted;
            public static object Updating() => TestValueUpdating;
            public static object Updated() => TestValueUpdated;
        }
    }
}