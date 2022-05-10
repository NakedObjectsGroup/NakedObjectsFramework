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

    [TestMethod]
    public void TestBinarySerializeCreatedCallbackFacetViaMethod() => TestSerializeCreatedCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDeletedCallbackFacetNull() => TestSerializeDeletedCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDeletedCallbackFacetViaMethod() => TestSerializeDeletedCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDeletingCallbackFacetNull() => TestSerializeDeletingCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDeletingCallbackFacetViaMethod() => TestSerializeDeletingCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisableForContextFacet() => TestSerializeDisableForContextFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeHideForContextFacet() => TestSerializeHideForContextFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeLoadedCallbackFacetNull() => TestSerializeLoadedCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeLoadedCallbackFacetViaMethod() => TestSerializeLoadedCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeLoadingCallbackFacetNull() => TestSerializeLoadingCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeLoadingCallbackFacetViaMethod() => TestSerializeLoadingCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOnPersistingErrorCallbackFacetViaMethod() => TestSerializeOnPersistingErrorCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOnUpdatingErrorCallbackFacetViaMethod() => TestSerializeOnUpdatingErrorCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePersistedCallbackFacetNull() => TestSerializePersistedCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePersistedCallbackFacetViaMethod() => TestSerializePersistedCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePersistingCallbackFacetNull() => TestSerializePersistingCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePersistingCallbackFacetViaMethod() => TestSerializePersistingCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyAccessorFacetViaContributedAction() => TestSerializePropertyAccessorFacetViaContributedAction(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyAccessorFacetViaMethod() => TestSerializePropertyAccessorFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyChoicesFacet() => TestSerializePropertyChoicesFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyDefaultFacetViaMethod() => TestSerializePropertyDefaultFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertySetterFacetViaModifyMethod() => TestSerializePropertySetterFacetViaModifyMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyValidateFacetViaMethod() => TestSerializePropertyValidateFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTitleFacetViaProperty() => TestSerializeTitleFacetViaProperty(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTitleFacetViaTitleMethod() => TestSerializeTitleFacetViaTitleMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeUpdatedCallbackFacetNull() => TestSerializeUpdatedCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeUpdatedCallbackFacetViaMethod() => TestSerializeUpdatedCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeUpdatingCallbackFacetNull() => TestSerializeUpdatingCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeUpdatingCallbackFacetViaMethod() => TestSerializeUpdatingCallbackFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeViewModelEditFacetConvention() => TestSerializeViewModelEditFacetConvention(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeViewModelFacetConvention() => TestSerializeViewModelFacetConvention(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeViewModelSwitchableFacetConvention() => TestSerializeViewModelSwitchableFacetConvention(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeProfileActionInvocationFacet() => TestSerializeProfileActionInvocationFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeProfileCallbackFacet() => TestSerializeProfileCallbackFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeProfilePropertySetterFacet() => TestSerializeProfilePropertySetterFacet(BinaryRoundTrip);
}