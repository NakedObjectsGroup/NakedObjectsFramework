// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NakedObjects.Reflector.Test.Serialization.FacetSerializationTests;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedObjects.Reflector.Test.Serialization;

[TestClass]
public class FacetBinarySerializationTests {
    [TestMethod]
    public void TestBinarySerializeAuditActionInvocationFacet() => TestSerializeAuditActionInvocationFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAuditPersistedFacet() => TestSerializeAuditPersistedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAuditUpdatedFacet() => TestSerializeAuditUpdatedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionChoicesFacetViaMethod() => TestSerializeActionChoicesFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionDefaultsFacetViaMethod() => TestSerializeActionDefaultsFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionInvocationFacetViaMethod() => TestSerializeActionInvocationFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionParameterValidation() => TestSerializeActionParameterValidation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionValidationFacet() => TestSerializeActionValidationFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAutoCompleteFacet() => TestSerializeAutoCompleteFacet(BinaryRoundTrip);
}