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
public class FacetJsonSerializationTests {
    [TestMethod]
    public void TestJsonSerializeActionChoicesFacetNone() => TestSerializeActionChoicesFacetNone(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeActionDefaultsFacetAnnotation() => TestSerializeActionDefaultsFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeActionDefaultsFacetNone() => TestSerializeActionDefaultsFacetNone(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeActionDefaultsFacetViaProperty() => TestSerializeActionDefaultsFacetViaProperty(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeAggregatedFacetAlways() => TestSerializeAggregatedFacetAlways(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeArrayFacet() => TestSerializeArrayFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeAuthorizationDisableForSessionFacetAnnotation() => TestSerializeAuthorizationDisableForSessionFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeAuthorizationDisableForSessionFacet() => TestSerializeAuthorizationDisableForSessionFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeAuthorizationHideForSessionFacetAnnotation() => TestSerializeAuthorizationHideForSessionFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeAuthorizationHideForSessionFacet() => TestSerializeAuthorizationHideForSessionFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeBoundedFacet() => TestSerializeBoundedFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeCollectionFacet() => TestSerializeCollectionFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeComplexTypeFacet() => TestSerializeComplexTypeFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeConcurrencyCheckFacet() => TestSerializeConcurrencyCheckFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeContributedActionFacet() => TestSerializeContributedActionFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeCreatedCallbackFacetNull() => TestSerializeCreatedCallbackFacetNull(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeCreateNewFacet() => TestSerializeCreateNewFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDataTypeFacet() => TestSerializeDataTypeFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDateOnlyFacet() => TestSerializeDateOnlyFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDefaultedFacetUsingDefaultsProvider() => TestSerializeDefaultedFacetUsingDefaultsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDescribedAsFacetAnnotation() => TestSerializeDescribedAsFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDescribedAsFacetNone() => TestSerializeDescribedAsFacetNone(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDisabledFacetAnnotation() => TestSerializeDisabledFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDisabledFacetAlways() => TestSerializeDisabledFacetAlways(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDisableForSessionFacetNone() => TestSerializeDisableForSessionFacetNone(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDisplayAsPropertyFacet() => TestSerializeDisplayAsPropertyFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeEagerlyFacet() => TestSerializeEagerlyFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeEditPropertiesFacet() => TestSerializeEditPropertiesFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeElementTypeFacet() => TestSerializeElementTypeFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeEnumFacet() => TestSerializeEnumFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeFinderActionFacet() => TestSerializeFinderActionFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeFindMenuFacet() => TestSerializeFindMenuFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeFromStreamFacetUsingFromStream() => TestSerializeFromStreamFacetUsingFromStream(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeGenericCollectionFacet() => TestSerializeGenericCollectionFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeGenericCollectionSetFacet() => TestSerializeGenericCollectionSetFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeGenericIEnumerableFacet() => TestSerializeGenericIEnumerableFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeGenericIEnumerableSetFacet() => TestSerializeGenericIEnumerableSetFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeGenericIQueryableFacet() => TestSerializeGenericIQueryableFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeGenericIQueryableSetFacet() => TestSerializeGenericIQueryableSetFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeHiddenFacet() => TestSerializeHiddenFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeHideForSessionFacetNone() => TestSerializeHideForSessionFacetNone(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeIdempotentFacet() => TestSerializeIdempotentFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeImmutableFacetAlways() => TestSerializeImmutableFacetAlways(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeImmutableFacetAnnotation() => TestSerializeImmutableFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeImmutableFacetNever() => TestSerializeImmutableFacetNever(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeIsASetFacet() => TestSerializeIsASetFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMandatoryFacet() => TestSerializeMandatoryFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMandatoryFacetDefault() => TestSerializeMandatoryFacetDefault(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMaskFacet() => TestSerializeMaskFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMaxLengthFacetAnnotation() => TestSerializeMaxLengthFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMaxLengthFacetZero() => TestSerializeMaxLengthFacetZero(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMemberNamedFacetAnnotation() => TestSerializeMemberNamedFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMemberNamedFacetInferred() => TestSerializeMemberNamedFacetInferred(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMemberOrderFacet() => TestSerializeMemberOrderFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMenuFacetDefault() => TestSerializeMenuFacetDefault(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMenuFacetViaMethod() => TestSerializeMenuFacetViaMethod(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMultiLineFacetAnnotation() => TestSerializeMultiLineFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMultiLineFacetNone() => TestSerializeMultiLineFacetNone(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeNamedFacetAnnotation() => TestSerializeNamedFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeNamedFacetInferred() => TestSerializeNamedFacetInferred(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeNotCountedFacet() => TestSerializeNotCountedFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeNotNavigableFacet() => TestSerializeNotNavigableFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeNotPersistedFacet() => TestSerializeNotPersistedFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeNullableFacetAlways() => TestSerializeNullableFacetAlways(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeOnPersistingErrorCallbackFacetNull() => TestSerializeOnPersistingErrorCallbackFacetNull(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeOnUpdatingErrorCallbackFacetNull() => TestSerializeOnUpdatingErrorCallbackFacetNull(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeOptionalFacet() => TestSerializeOptionalFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeOptionalFacetDefault() => TestSerializeOptionalFacetDefault(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePageSizeFacetAnnotation() => TestSerializePageSizeFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePageSizeFacetDefault() => TestSerializePageSizeFacetDefault(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeParseableFacetUsingParser() => TestSerializeParseableFacetUsingParser(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePasswordFacet() => TestSerializePasswordFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePluralFacetAnnotation() => TestSerializePluralFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePluralFacetInferred() => TestSerializePluralFacetInferred(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePresentationHintFacet() => TestSerializePresentationHintFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePropertyAccessorFacet() => TestSerializePropertyAccessorFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePropertyDefaultFacetAnnotation() => TestSerializePropertyDefaultFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePropertyDefaultFacetNone() => TestSerializePropertyDefaultFacetNone(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePropertyInitializationFacet() => TestSerializePropertyInitializationFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePropertySetterFacetViaSetterMethod() => TestSerializePropertySetterFacetViaSetterMethod(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePropertyValidateFacetDefault() => TestSerializePropertyValidateFacetDefault(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializePropertyValidateFacetNone() => TestSerializePropertyValidateFacetNone(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeRangeFacet() => TestSerializeRangeFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeRedirectedFacet() => TestSerializeRedirectedFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeRegExFacet() => TestSerializeRegExFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeRestExtensionFacet() => TestSerializeRestExtensionFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeSaveFacet() => TestSerializeSaveFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTableViewFacet() => TestSerializeTableViewFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTitleFacetNone() => TestSerializeTitleFacetNone(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTitleFacetUsingParser() => TestSerializeTitleFacetUsingParser(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTitleFacetViaToStringMethodNull() => TestSerializeTitleFacetViaToStringMethod(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTitleFacetViaToStringMethod() => TestSerializeTitleFacetViaMaskedToStringMethod(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTypeFacet() => TestSerializeTypeFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTypeIsAbstractFacet() => TestSerializeTypeIsAbstractFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTypeIsInterfaceFacet() => TestSerializeTypeIsInterfaceFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTypeIsSealedFacet() => TestSerializeTypeIsSealedFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTypeIsStaticFacet() => TestSerializeTypeIsStaticFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTypeIsVoidFacet() => TestSerializeTypeIsVoidFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTypeOfFacetDefaultToObject() => TestSerializeTypeOfFacetDefaultToObject(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTypeOfFacetInferredFromArray() => TestSerializeTypeOfFacetInferredFromArray(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTypeOfFacetInferredFromGenerics() => TestSerializeTypeOfFacetInferredFromGenerics(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeValidateObjectFacet() => TestSerializeValidateObjectFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeValidateObjectFacetNull() => TestSerializeValidateObjectFacetNull(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeValidateProgrammaticUpdatesFacetAnnotation() => TestSerializeValidateProgrammaticUpdatesFacetAnnotation(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeValueFacet() => TestSerializeValueFacet(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeValueFacetFromSemanticProvider() => TestSerializeValueFacetFromSemanticProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDescribedAsFacetI18N() => TestSerializeDescribedAsFacetI18N(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeMemberNamedFacetI18N() => TestSerializeMemberNamedFacetI18N(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeNamedFacetI18N() => TestSerializeNamedFacetI18N(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeArrayValueSemanticsProvider() => TestSerializeArrayValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeBooleanValueSemanticsProvider() => TestSerializeBooleanValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeByteValueSemanticsProvider() => TestSerializeByteValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeCharValueSemanticsProvider() => TestSerializeCharValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeColorValueSemanticsProvider() => TestSerializeColorValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDateTimeValueSemanticsProvider() => TestSerializeDateTimeValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDecimalValueSemanticsProvider() => TestSerializeDecimalValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeDoubleValueSemanticsProvider() => TestSerializeDoubleValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeEnumValueSemanticsProvider() => TestSerializeEnumValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeFileAttachmentValueSemanticsProvider() => TestSerializeFileAttachmentValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeFloatValueSemanticsProvider() => TestSerializeFloatValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeGuidValueSemanticsProvider() => TestSerializeGuidValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeImageValueSemanticsProvider() => TestSerializeImageValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeIntValueSemanticsProvider() => TestSerializeIntValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeLongValueSemanticsProvider() => TestSerializeLongValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeSbyteValueSemanticsProvider() => TestSerializeSbyteValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeShortValueSemanticsProvider() => TestSerializeShortValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeStringValueSemanticsProvider() => TestSerializeStringValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeTimeValueSemanticsProvider() => TestSerializeTimeValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeUIntValueSemanticsProvider() => TestSerializeUIntValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeULongValueSemanticsProvider() => TestSerializeULongValueSemanticsProvider(JsonRoundTrip);

    [TestMethod]
    public void TestJsonSerializeUShortValueSemanticsProvider() => TestSerializeUShortValueSemanticsProvider(JsonRoundTrip);
}