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

namespace NakedFunctions.Reflector.Test.Facet {
    [TestClass]
    public class ActionValidationViaFunctionFacetTest {
        private static readonly string TestValue = "fail";

        [TestMethod]
        public void TestValidate() {
            var method = typeof(TestClass).GetMethod(nameof(TestClass.Validate));
            var testFacet = new ActionValidationViaFunctionFacet(method, null);

            var result = testFacet.InvalidReason(null, null, Array.Empty<INakedObjectAdapter>());

            Assert.AreEqual(TestValue, result);
        }

        public static class TestClass {
            public static string Validate() => TestValue;
        }
    }
}