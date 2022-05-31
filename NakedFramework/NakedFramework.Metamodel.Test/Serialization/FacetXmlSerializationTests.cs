// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NakedFramework.Metamodel.Test.Serialization.FacetSerializationTests;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedFramework.Metamodel.Test.Serialization;

[TestClass]
public class FacetXmlSerializationTests {
    [TestMethod]
    public void TestXmlSerializeActionChoicesFacetNone() => TestSerializeActionChoicesFacetNone(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeActionDefaultsFacetAnnotation() => TestSerializeActionDefaultsFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeActionDefaultsFacetNone() => TestSerializeActionDefaultsFacetNone(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeActionDefaultsFacetViaProperty() => TestSerializeActionDefaultsFacetViaProperty(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeAggregatedFacetAlways() => TestSerializeAggregatedFacetAlways(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeArrayFacet() => TestSerializeArrayFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeAuthorizationDisableForSessionFacetAnnotation() => TestSerializeAuthorizationDisableForSessionFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeAuthorizationDisableForSessionFacet() => TestSerializeAuthorizationDisableForSessionFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeAuthorizationHideForSessionFacetAnnotation() => TestSerializeAuthorizationHideForSessionFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeAuthorizationHideForSessionFacet() => TestSerializeAuthorizationHideForSessionFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeBoundedFacet() => TestSerializeBoundedFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeCollectionFacet() => TestSerializeCollectionFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeComplexTypeFacet() => TestSerializeComplexTypeFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeConcurrencyCheckFacet() => TestSerializeConcurrencyCheckFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeContributedActionFacet() => TestSerializeContributedActionFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeCreatedCallbackFacetNull() => TestSerializeCreatedCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeCreateNewFacet() => TestSerializeCreateNewFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDataTypeFacet() => TestSerializeDataTypeFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDateOnlyFacet() => TestSerializeDateOnlyFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDefaultedFacetUsingDefaultsProvider() => TestSerializeDefaultedFacetUsingDefaultsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDescribedAsFacetAnnotation() => TestSerializeDescribedAsFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDescribedAsFacetNone() => TestSerializeDescribedAsFacetNone(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDisabledFacetAnnotation() => TestSerializeDisabledFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDisabledFacetAlways() => TestSerializeDisabledFacetAlways(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDisableForSessionFacetNone() => TestSerializeDisableForSessionFacetNone(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDisplayAsPropertyFacet() => TestSerializeDisplayAsPropertyFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeEagerlyFacet() => TestSerializeEagerlyFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeEditPropertiesFacet() => TestSerializeEditPropertiesFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeElementTypeFacet() => TestSerializeElementTypeFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeEnumFacet() => TestSerializeEnumFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeFinderActionFacet() => TestSerializeFinderActionFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeFindMenuFacet() => TestSerializeFindMenuFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeFromStreamFacetUsingFromStream() => TestSerializeFromStreamFacetUsingFromStream(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeGenericCollectionFacet() => TestSerializeGenericCollectionFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeGenericCollectionSetFacet() => TestSerializeGenericCollectionSetFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeGenericIEnumerableFacet() => TestSerializeGenericIEnumerableFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeGenericIEnumerableSetFacet() => TestSerializeGenericIEnumerableSetFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeGenericIQueryableFacet() => TestSerializeGenericIQueryableFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeGenericIQueryableSetFacet() => TestSerializeGenericIQueryableSetFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeHiddenFacet() => TestSerializeHiddenFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeHideForSessionFacetNone() => TestSerializeHideForSessionFacetNone(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeIdempotentFacet() => TestSerializeIdempotentFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeImmutableFacetAlways() => TestSerializeImmutableFacetAlways(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeImmutableFacetAnnotation() => TestSerializeImmutableFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeImmutableFacetNever() => TestSerializeImmutableFacetNever(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeIsASetFacet() => TestSerializeIsASetFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMandatoryFacet() => TestSerializeMandatoryFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMandatoryFacetDefault() => TestSerializeMandatoryFacetDefault(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMaskFacet() => TestSerializeMaskFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMaxLengthFacetAnnotation() => TestSerializeMaxLengthFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMaxLengthFacetZero() => TestSerializeMaxLengthFacetZero(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMemberNamedFacetAnnotation() => TestSerializeMemberNamedFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMemberNamedFacetInferred() => TestSerializeMemberNamedFacetInferred(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMemberOrderFacet() => TestSerializeMemberOrderFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMenuFacetDefault() => TestSerializeMenuFacetDefault(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMenuFacetViaMethod() => TestSerializeMenuFacetViaMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMultiLineFacetAnnotation() => TestSerializeMultiLineFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMultiLineFacetNone() => TestSerializeMultiLineFacetNone(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeNamedFacetAnnotation() => TestSerializeNamedFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeNamedFacetInferred() => TestSerializeNamedFacetInferred(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeNotCountedFacet() => TestSerializeNotCountedFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeNotNavigableFacet() => TestSerializeNotNavigableFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeNotPersistedFacet() => TestSerializeNotPersistedFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeNullableFacetAlways() => TestSerializeNullableFacetAlways(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeOnPersistingErrorCallbackFacetNull() => TestSerializeOnPersistingErrorCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeOnUpdatingErrorCallbackFacetNull() => TestSerializeOnUpdatingErrorCallbackFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeOptionalFacet() => TestSerializeOptionalFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeOptionalFacetDefault() => TestSerializeOptionalFacetDefault(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePageSizeFacetAnnotation() => TestSerializePageSizeFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePageSizeFacetDefault() => TestSerializePageSizeFacetDefault(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeParseableFacetUsingParser() => TestSerializeParseableFacetUsingParser(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePasswordFacet() => TestSerializePasswordFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePluralFacetAnnotation() => TestSerializePluralFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePluralFacetInferred() => TestSerializePluralFacetInferred(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePresentationHintFacet() => TestSerializePresentationHintFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyAccessorFacet() => TestSerializePropertyAccessorFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyDefaultFacetAnnotation() => TestSerializePropertyDefaultFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyDefaultFacetNone() => TestSerializePropertyDefaultFacetNone(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyInitializationFacet() => TestSerializePropertyInitializationFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertySetterFacetViaSetterMethod() => TestSerializePropertySetterFacetViaSetterMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyValidateFacetDefault() => TestSerializePropertyValidateFacetDefault(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializePropertyValidateFacetNone() => TestSerializePropertyValidateFacetNone(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeRangeFacet() => TestSerializeRangeFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeRedirectedFacet() => TestSerializeRedirectedFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeRegExFacet() => TestSerializeRegExFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeRestExtensionFacet() => TestSerializeRestExtensionFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeSaveFacet() => TestSerializeSaveFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTableViewFacet() => TestSerializeTableViewFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTitleFacetNone() => TestSerializeTitleFacetNone(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTitleFacetUsingParser() => TestSerializeTitleFacetUsingParser(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTitleFacetViaToStringMethodNull() => TestSerializeTitleFacetViaToStringMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTitleFacetViaToStringMethod() => TestSerializeTitleFacetViaMaskedToStringMethod(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTypeFacet() => TestSerializeTypeFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTypeIsAbstractFacet() => TestSerializeTypeIsAbstractFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTypeIsInterfaceFacet() => TestSerializeTypeIsInterfaceFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTypeIsSealedFacet() => TestSerializeTypeIsSealedFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTypeIsStaticFacet() => TestSerializeTypeIsStaticFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTypeIsVoidFacet() => TestSerializeTypeIsVoidFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTypeOfFacetDefaultToObject() => TestSerializeTypeOfFacetDefaultToObject(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTypeOfFacetInferredFromArray() => TestSerializeTypeOfFacetInferredFromArray(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTypeOfFacetInferredFromGenerics() => TestSerializeTypeOfFacetInferredFromGenerics(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeValidateObjectFacet() => TestSerializeValidateObjectFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeValidateObjectFacetNull() => TestSerializeValidateObjectFacetNull(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeValidateProgrammaticUpdatesFacetAnnotation() => TestSerializeValidateProgrammaticUpdatesFacetAnnotation(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeValueFacet() => TestSerializeValueFacet(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeValueFacetFromSemanticProvider() => TestSerializeValueFacetFromSemanticProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDescribedAsFacetI18N() => TestSerializeDescribedAsFacetI18N(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeMemberNamedFacetI18N() => TestSerializeMemberNamedFacetI18N(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeNamedFacetI18N() => TestSerializeNamedFacetI18N(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeArrayValueSemanticsProvider() => TestSerializeArrayValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeBooleanValueSemanticsProvider() => TestSerializeBooleanValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeByteValueSemanticsProvider() => TestSerializeByteValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeCharValueSemanticsProvider() => TestSerializeCharValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeColorValueSemanticsProvider() => TestSerializeColorValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDateTimeValueSemanticsProvider() => TestSerializeDateTimeValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDecimalValueSemanticsProvider() => TestSerializeDecimalValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeDoubleValueSemanticsProvider() => TestSerializeDoubleValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeEnumValueSemanticsProvider() => TestSerializeEnumValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeFileAttachmentValueSemanticsProvider() => TestSerializeFileAttachmentValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeFloatValueSemanticsProvider() => TestSerializeFloatValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeGuidValueSemanticsProvider() => TestSerializeGuidValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeImageValueSemanticsProvider() => TestSerializeImageValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeIntValueSemanticsProvider() => TestSerializeIntValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeLongValueSemanticsProvider() => TestSerializeLongValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeSbyteValueSemanticsProvider() => TestSerializeSbyteValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeShortValueSemanticsProvider() => TestSerializeShortValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeStringValueSemanticsProvider() => TestSerializeStringValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeTimeValueSemanticsProvider() => TestSerializeTimeValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeUIntValueSemanticsProvider() => TestSerializeUIntValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeULongValueSemanticsProvider() => TestSerializeULongValueSemanticsProvider(XmlRoundTrip);

    [TestMethod]
    public void TestXmlSerializeUShortValueSemanticsProvider() => TestSerializeUShortValueSemanticsProvider(XmlRoundTrip);
}