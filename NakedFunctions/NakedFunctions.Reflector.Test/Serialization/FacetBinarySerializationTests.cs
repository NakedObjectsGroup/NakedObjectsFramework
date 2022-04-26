// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NakedFunctions.Reflector.Test.Serialization.FacetSerializationTests;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedFunctions.Reflector.Test.Serialization;

[TestClass]
public class FacetBinarySerializationTests {
    [TestMethod]
    public void TestBinarySerializeActionChoicesFacetViaFunction() => TestSerializeActionChoicesFacetViaFunction(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionDefaultsFacetViaFunction() => TestSerializeActionDefaultsFacetViaFunction(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionInvocationFacetViaStaticMethod() => TestSerializeActionInvocationFacetViaStaticMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionParameterValidationViaFunctionFacet() => TestSerializeActionParameterValidationViaFunctionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionValidationViaFunctionFacet() => TestSerializeActionValidationViaFunctionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAutoCompleteViaFunctionFacet() => TestSerializeAutoCompleteViaFunctionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisableForContextViaFunctionFacet() => TestSerializeDisableForContextViaFunctionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeHideForContextViaFunctionFacet() => TestSerializeHideForContextViaFunctionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeInjectedFacet() => TestSerializeInjectedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeInjectedIContextParameterFacet() => TestSerializeInjectedIContextParameterFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeStaticFunctionFacet() => TestSerializeStaticFunctionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTitleFacetViaTitleFunction() => TestSerializeTitleFacetViaTitleFunction(BinaryRoundTrip);
}