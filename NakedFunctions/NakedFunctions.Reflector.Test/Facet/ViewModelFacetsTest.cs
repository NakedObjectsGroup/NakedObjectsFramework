// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFunctions.Reflector.Facet;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;

namespace NakedFunctions.Reflector.Test.Facet {
    [TestClass]
    public class ViewModelFacetsTest {
        private static readonly TestClass testClass = new("1", "2");

        [TestMethod]
        public void TestViewModelFacetViaFunctionsConvention() {
            var deriveMethod = typeof(TestViewModelClass).GetMethod(nameof(TestViewModelClass.Derive));
            var populateMethod = typeof(TestViewModelClass).GetMethod(nameof(TestViewModelClass.Populate));

            var testFacet = new ViewModelFacetViaFunctionsConvention(null, deriveMethod, populateMethod);

            var keys = testFacet.Derive(null, null);

            Assert.AreEqual(testClass.Keys, keys);

            var testAdapter = new TestAdapter();
            testFacet.Populate(keys, testAdapter, null);

            Assert.AreEqual(testClass, testAdapter.Object);
        }

        private class TestAdapter : INakedObjectAdapter {
            public object Object { get; private set; }
            public ITypeSpec Spec { get; }
            public IOid Oid { get; }
            public IResolveStateMachine ResolveState { get; }
            public IVersion Version { get; }
            public IVersion OptimisticLock { get; set; }
            public string IconName() => throw new NotImplementedException();
            public string TitleString() => throw new NotImplementedException();
            public string InvariantString() => throw new NotImplementedException();
            public void CheckLock(IVersion otherVersion) => throw new NotImplementedException();
            public void ReplacePoco(object poco) => Object = poco;
            public string ValidToPersist() => throw new NotImplementedException();
            public void SetATransientOid(IOid oid) => throw new NotImplementedException();
            public void LoadAnyComplexTypes() => throw new NotImplementedException();
            public void Created() => throw new NotImplementedException();
            public void Deleting() => throw new NotImplementedException();
            public void Deleted() => throw new NotImplementedException();
            public void Loading() => throw new NotImplementedException();
            public void Loaded() => throw new NotImplementedException();
            public void Persisting() => throw new NotImplementedException();
            public void Persisted() => throw new NotImplementedException();
            public void Updating() => throw new NotImplementedException();
            public void Updated() => throw new NotImplementedException();
            public object PersistingAndReturn() => throw new NotImplementedException();
            public object PersistedAndReturn() => throw new NotImplementedException();
            public object UpdatingAndReturn() => throw new NotImplementedException();
            public object UpdatedAndReturn() => throw new NotImplementedException();
        }

        private record TestClass {
            public TestClass(params string[] keys) => Keys = keys;

            public string[] Keys { get; }
        }

        public static class TestViewModelClass {
            public static object Derive() => testClass.Keys;
            public static object Populate(string[] keys) => new TestClass(keys);

            public static bool IsEditable() => false;
        }
    }
}