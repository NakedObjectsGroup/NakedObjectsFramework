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

[TestClass, Ignore]
public class FacetNsJsonSerializationTests {
    [TestMethod]
    public void TestNsJsonSerializeActionChoicesFacetNone() => TestSerializeActionChoicesFacetNone(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeActionDefaultsFacetAnnotation() => TestSerializeActionDefaultsFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeActionDefaultsFacetNone() => TestSerializeActionDefaultsFacetNone(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeActionDefaultsFacetViaProperty() => TestSerializeActionDefaultsFacetViaProperty(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeAggregatedFacetAlways() => TestSerializeAggregatedFacetAlways(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeArrayFacet() => TestSerializeArrayFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeAuthorizationDisableForSessionFacetAnnotation() => TestSerializeAuthorizationDisableForSessionFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeAuthorizationDisableForSessionFacet() => TestSerializeAuthorizationDisableForSessionFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeAuthorizationHideForSessionFacetAnnotation() => TestSerializeAuthorizationHideForSessionFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeAuthorizationHideForSessionFacet() => TestSerializeAuthorizationHideForSessionFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeBoundedFacet() => TestSerializeBoundedFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeCollectionFacet() => TestSerializeCollectionFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeComplexTypeFacet() => TestSerializeComplexTypeFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeConcurrencyCheckFacet() => TestSerializeConcurrencyCheckFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeContributedActionFacet() => TestSerializeContributedActionFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeCreatedCallbackFacetNull() => TestSerializeCreatedCallbackFacetNull(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeCreateNewFacet() => TestSerializeCreateNewFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDataTypeFacet() => TestSerializeDataTypeFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDateOnlyFacet() => TestSerializeDateOnlyFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDefaultedFacetUsingDefaultsProvider() => TestSerializeDefaultedFacetUsingDefaultsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDescribedAsFacetAnnotation() => TestSerializeDescribedAsFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDescribedAsFacetNone() => TestSerializeDescribedAsFacetNone(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDisabledFacetAnnotation() => TestSerializeDisabledFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDisabledFacetAlways() => TestSerializeDisabledFacetAlways(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDisableForSessionFacetNone() => TestSerializeDisableForSessionFacetNone(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDisplayAsPropertyFacet() => TestSerializeDisplayAsPropertyFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeEagerlyFacet() => TestSerializeEagerlyFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeEditPropertiesFacet() => TestSerializeEditPropertiesFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeElementTypeFacet() => TestSerializeElementTypeFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeEnumFacet() => TestSerializeEnumFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeFinderActionFacet() => TestSerializeFinderActionFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeFindMenuFacet() => TestSerializeFindMenuFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeFromStreamFacetUsingFromStream() => TestSerializeFromStreamFacetUsingFromStream(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeGenericCollectionFacet() => TestSerializeGenericCollectionFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeGenericCollectionSetFacet() => TestSerializeGenericCollectionSetFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeGenericIEnumerableFacet() => TestSerializeGenericIEnumerableFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeGenericIEnumerableSetFacet() => TestSerializeGenericIEnumerableSetFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeGenericIQueryableFacet() => TestSerializeGenericIQueryableFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeGenericIQueryableSetFacet() => TestSerializeGenericIQueryableSetFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeHiddenFacet() => TestSerializeHiddenFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeHideForSessionFacetNone() => TestSerializeHideForSessionFacetNone(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeIdempotentFacet() => TestSerializeIdempotentFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeImmutableFacetAlways() => TestSerializeImmutableFacetAlways(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeImmutableFacetAnnotation() => TestSerializeImmutableFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeImmutableFacetNever() => TestSerializeImmutableFacetNever(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeIsASetFacet() => TestSerializeIsASetFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMandatoryFacet() => TestSerializeMandatoryFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMandatoryFacetDefault() => TestSerializeMandatoryFacetDefault(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMaskFacet() => TestSerializeMaskFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMaxLengthFacetAnnotation() => TestSerializeMaxLengthFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMaxLengthFacetZero() => TestSerializeMaxLengthFacetZero(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMemberNamedFacetAnnotation() => TestSerializeMemberNamedFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMemberNamedFacetInferred() => TestSerializeMemberNamedFacetInferred(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMemberOrderFacet() => TestSerializeMemberOrderFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMenuFacetDefault() => TestSerializeMenuFacetDefault(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMenuFacetViaMethod() => TestSerializeMenuFacetViaMethod(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMultiLineFacetAnnotation() => TestSerializeMultiLineFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMultiLineFacetNone() => TestSerializeMultiLineFacetNone(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeNamedFacetAnnotation() => TestSerializeNamedFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeNamedFacetInferred() => TestSerializeNamedFacetInferred(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeNotCountedFacet() => TestSerializeNotCountedFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeNotNavigableFacet() => TestSerializeNotNavigableFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeNotPersistedFacet() => TestSerializeNotPersistedFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeNullableFacetAlways() => TestSerializeNullableFacetAlways(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeOnPersistingErrorCallbackFacetNull() => TestSerializeOnPersistingErrorCallbackFacetNull(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeOnUpdatingErrorCallbackFacetNull() => TestSerializeOnUpdatingErrorCallbackFacetNull(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeOptionalFacet() => TestSerializeOptionalFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeOptionalFacetDefault() => TestSerializeOptionalFacetDefault(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePageSizeFacetAnnotation() => TestSerializePageSizeFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePageSizeFacetDefault() => TestSerializePageSizeFacetDefault(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeParseableFacetUsingParser() => TestSerializeParseableFacetUsingParser(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePasswordFacet() => TestSerializePasswordFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePluralFacetAnnotation() => TestSerializePluralFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePluralFacetInferred() => TestSerializePluralFacetInferred(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePresentationHintFacet() => TestSerializePresentationHintFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePropertyAccessorFacet() => TestSerializePropertyAccessorFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePropertyDefaultFacetAnnotation() => TestSerializePropertyDefaultFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePropertyDefaultFacetNone() => TestSerializePropertyDefaultFacetNone(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePropertyInitializationFacet() => TestSerializePropertyInitializationFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePropertySetterFacetViaSetterMethod() => TestSerializePropertySetterFacetViaSetterMethod(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePropertyValidateFacetDefault() => TestSerializePropertyValidateFacetDefault(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializePropertyValidateFacetNone() => TestSerializePropertyValidateFacetNone(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeRangeFacet() => TestSerializeRangeFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeRedirectedFacet() => TestSerializeRedirectedFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeRegExFacet() => TestSerializeRegExFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeRestExtensionFacet() => TestSerializeRestExtensionFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeSaveFacet() => TestSerializeSaveFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTableViewFacet() => TestSerializeTableViewFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTitleFacetNone() => TestSerializeTitleFacetNone(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTitleFacetUsingParser() => TestSerializeTitleFacetUsingParser(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTitleFacetViaToStringMethodNull() => TestSerializeTitleFacetViaToStringMethod(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTitleFacetViaToStringMethod() => TestSerializeTitleFacetViaMaskedToStringMethod(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTypeFacet() => TestSerializeTypeFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTypeIsAbstractFacet() => TestSerializeTypeIsAbstractFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTypeIsInterfaceFacet() => TestSerializeTypeIsInterfaceFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTypeIsSealedFacet() => TestSerializeTypeIsSealedFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTypeIsStaticFacet() => TestSerializeTypeIsStaticFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTypeIsVoidFacet() => TestSerializeTypeIsVoidFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTypeOfFacetDefaultToObject() => TestSerializeTypeOfFacetDefaultToObject(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTypeOfFacetInferredFromArray() => TestSerializeTypeOfFacetInferredFromArray(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTypeOfFacetInferredFromGenerics() => TestSerializeTypeOfFacetInferredFromGenerics(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeValidateObjectFacet() => TestSerializeValidateObjectFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeValidateObjectFacetNull() => TestSerializeValidateObjectFacetNull(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeValidateProgrammaticUpdatesFacetAnnotation() => TestSerializeValidateProgrammaticUpdatesFacetAnnotation(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeValueFacet() => TestSerializeValueFacet(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeValueFacetFromSemanticProvider() => TestSerializeValueFacetFromSemanticProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDescribedAsFacetI18N() => TestSerializeDescribedAsFacetI18N(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeMemberNamedFacetI18N() => TestSerializeMemberNamedFacetI18N(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeNamedFacetI18N() => TestSerializeNamedFacetI18N(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeArrayValueSemanticsProvider() => TestSerializeArrayValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeBooleanValueSemanticsProvider() => TestSerializeBooleanValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeByteValueSemanticsProvider() => TestSerializeByteValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeCharValueSemanticsProvider() => TestSerializeCharValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeColorValueSemanticsProvider() => TestSerializeColorValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDateTimeValueSemanticsProvider() => TestSerializeDateTimeValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDecimalValueSemanticsProvider() => TestSerializeDecimalValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeDoubleValueSemanticsProvider() => TestSerializeDoubleValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeEnumValueSemanticsProvider() => TestSerializeEnumValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeFileAttachmentValueSemanticsProvider() => TestSerializeFileAttachmentValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeFloatValueSemanticsProvider() => TestSerializeFloatValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeGuidValueSemanticsProvider() => TestSerializeGuidValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeImageValueSemanticsProvider() => TestSerializeImageValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeIntValueSemanticsProvider() => TestSerializeIntValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeLongValueSemanticsProvider() => TestSerializeLongValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeSbyteValueSemanticsProvider() => TestSerializeSbyteValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeShortValueSemanticsProvider() => TestSerializeShortValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeStringValueSemanticsProvider() => TestSerializeStringValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeTimeValueSemanticsProvider() => TestSerializeTimeValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeUIntValueSemanticsProvider() => TestSerializeUIntValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeULongValueSemanticsProvider() => TestSerializeULongValueSemanticsProvider(NsJsonRoundTrip);

    [TestMethod]
    public void TestNsJsonSerializeUShortValueSemanticsProvider() => TestSerializeUShortValueSemanticsProvider(NsJsonRoundTrip);
}