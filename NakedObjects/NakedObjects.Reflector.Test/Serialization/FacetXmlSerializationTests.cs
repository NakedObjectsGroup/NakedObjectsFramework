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
public class FacetXmlSerializationTests {
    [TestMethod]
    public void TestXmlSerializeAuditActionInvocationFacet() => TestSerializeAuditActionInvocationFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeAuditPersistedFacet() => TestSerializeAuditPersistedFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeAuditUpdatedFacet() => TestSerializeAuditUpdatedFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeActionChoicesFacetViaMethod() => TestSerializeActionChoicesFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeActionDefaultsFacetViaMethod() => TestSerializeActionDefaultsFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeActionInvocationFacetViaMethod() => TestSerializeActionInvocationFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeActionParameterValidation() => TestSerializeActionParameterValidation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeActionValidationFacet() => TestSerializeActionValidationFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeAutoCompleteFacet() => TestSerializeAutoCompleteFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeCreatedCallbackFacetViaMethod() => TestSerializeCreatedCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDeletedCallbackFacetNull() => TestSerializeDeletedCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDeletedCallbackFacetViaMethod() => TestSerializeDeletedCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDeletingCallbackFacetNull() => TestSerializeDeletingCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDeletingCallbackFacetViaMethod() => TestSerializeDeletingCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDisableForContextFacet() => TestSerializeDisableForContextFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeHideForContextFacet() => TestSerializeHideForContextFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeLoadedCallbackFacetNull() => TestSerializeLoadedCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeLoadedCallbackFacetViaMethod() => TestSerializeLoadedCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeLoadingCallbackFacetNull() => TestSerializeLoadingCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeLoadingCallbackFacetViaMethod() => TestSerializeLoadingCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeOnPersistingErrorCallbackFacetViaMethod() => TestSerializeOnPersistingErrorCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeOnUpdatingErrorCallbackFacetViaMethod() => TestSerializeOnUpdatingErrorCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePersistedCallbackFacetNull() => TestSerializePersistedCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePersistedCallbackFacetViaMethod() => TestSerializePersistedCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePersistingCallbackFacetNull() => TestSerializePersistingCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePersistingCallbackFacetViaMethod() => TestSerializePersistingCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyAccessorFacetViaContributedAction() => TestSerializePropertyAccessorFacetViaContributedAction(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyAccessorFacetViaMethod() => TestSerializePropertyAccessorFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyChoicesFacet() => TestSerializePropertyChoicesFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyDefaultFacetViaMethod() => TestSerializePropertyDefaultFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertySetterFacetViaModifyMethod() => TestSerializePropertySetterFacetViaModifyMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyValidateFacetViaMethod() => TestSerializePropertyValidateFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTitleFacetViaProperty() => TestSerializeTitleFacetViaProperty(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTitleFacetViaTitleMethod() => TestSerializeTitleFacetViaTitleMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeUpdatedCallbackFacetNull() => TestSerializeUpdatedCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeUpdatedCallbackFacetViaMethod() => TestSerializeUpdatedCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeUpdatingCallbackFacetNull() => TestSerializeUpdatingCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeUpdatingCallbackFacetViaMethod() => TestSerializeUpdatingCallbackFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeViewModelEditFacetConvention() => TestSerializeViewModelEditFacetConvention(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeViewModelFacetConvention() => TestSerializeViewModelFacetConvention(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeViewModelSwitchableFacetConvention() => TestSerializeViewModelSwitchableFacetConvention(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeProfileActionInvocationFacet() => TestSerializeProfileActionInvocationFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeProfileCallbackFacet() => TestSerializeProfileCallbackFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeProfilePropertySetterFacet() => TestSerializeProfilePropertySetterFacet(XmlRoundTrip);
}