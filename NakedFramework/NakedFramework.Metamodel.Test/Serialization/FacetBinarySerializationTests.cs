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
public class FacetBinarySerializationTests {
    [TestMethod]
    public void TestBinarySerializeActionChoicesFacetNone() => TestSerializeActionChoicesFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionDefaultsFacetAnnotation() => TestSerializeActionDefaultsFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionDefaultsFacetNone() => TestSerializeActionDefaultsFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionDefaultsFacetViaProperty() => TestSerializeActionDefaultsFacetViaProperty(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAggregatedFacetAlways() => TestSerializeAggregatedFacetAlways(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeArrayFacet() => TestSerializeArrayFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAuthorizationDisableForSessionFacetAnnotation() => TestSerializeAuthorizationDisableForSessionFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAuthorizationDisableForSessionFacet() => TestSerializeAuthorizationDisableForSessionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAuthorizationHideForSessionFacetAnnotation() => TestSerializeAuthorizationHideForSessionFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAuthorizationHideForSessionFacet() => TestSerializeAuthorizationHideForSessionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeBoundedFacet() => TestSerializeBoundedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeCollectionFacet() => TestSerializeCollectionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeComplexTypeFacet() => TestSerializeComplexTypeFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeConcurrencyCheckFacet() => TestSerializeConcurrencyCheckFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeContributedActionFacet() => TestSerializeContributedActionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeCreatedCallbackFacetNull() => TestSerializeCreatedCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeCreateNewFacet() => TestSerializeCreateNewFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDataTypeFacet() => TestSerializeDataTypeFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDateOnlyFacet() => TestSerializeDateOnlyFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDefaultedFacetUsingDefaultsProvider() => TestSerializeDefaultedFacetUsingDefaultsProvider(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDescribedAsFacetAnnotation() => TestSerializeDescribedAsFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDescribedAsFacetNone() => TestSerializeDescribedAsFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisabledFacetAnnotation() => TestSerializeDisabledFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisabledFacetAlways() => TestSerializeDisabledFacetAlways(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisableForSessionFacetNone() => TestSerializeDisableForSessionFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisplayAsPropertyFacet() => TestSerializeDisplayAsPropertyFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeEagerlyFacet() => TestSerializeEagerlyFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeEditPropertiesFacet() => TestSerializeEditPropertiesFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeElementTypeFacet() => TestSerializeElementTypeFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeEnumFacet() => TestSerializeEnumFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeFinderActionFacet() => TestSerializeFinderActionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeFindMenuFacet() => TestSerializeFindMenuFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeFromStreamFacetUsingFromStream() => TestSerializeFromStreamFacetUsingFromStream(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericCollectionFacet() => TestSerializeGenericCollectionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericCollectionSetFacet() => TestSerializeGenericCollectionSetFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericIEnumerableFacet() => TestSerializeGenericIEnumerableFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericIEnumerableSetFacet() => TestSerializeGenericIEnumerableSetFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericIQueryableFacet() => TestSerializeGenericIQueryableFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericIQueryableSetFacet() => TestSerializeGenericIQueryableSetFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeHiddenFacet() => TestSerializeHiddenFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeHideForSessionFacetNone() => TestSerializeHideForSessionFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeIdempotentFacet() => TestSerializeIdempotentFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeImmutableFacetAlways() => TestSerializeImmutableFacetAlways(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeImmutableFacetAnnotation() => TestSerializeImmutableFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeImmutableFacetNever() => TestSerializeImmutableFacetNever(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeIsASetFacet() => TestSerializeIsASetFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMandatoryFacet() => TestSerializeMandatoryFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMandatoryFacetDefault() => TestSerializeMandatoryFacetDefault(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMaskFacet() => TestSerializeMaskFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMaxLengthFacetAnnotation() => TestSerializeMaxLengthFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMaxLengthFacetZero() => TestSerializeMaxLengthFacetZero(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMemberNamedFacetAnnotation() => TestSerializeMemberNamedFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMemberNamedFacetInferred() => TestSerializeMemberNamedFacetInferred(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMemberOrderFacet() => TestSerializeMemberOrderFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMenuFacetDefault() => TestSerializeMenuFacetDefault(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMenuFacetViaMethod() => TestSerializeMenuFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMultiLineFacetAnnotation() => TestSerializeMultiLineFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMultiLineFacetNone() => TestSerializeMultiLineFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNamedFacetAnnotation() => TestSerializeNamedFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNamedFacetInferred() => TestSerializeNamedFacetInferred(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNotCountedFacet() => TestSerializeNotCountedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNotNavigableFacet() => TestSerializeNotNavigableFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNotPersistedFacet() => TestSerializeNotPersistedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNullableFacetAlways() => TestSerializeNullableFacetAlways(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOnPersistingErrorCallbackFacetNull() => TestSerializeOnPersistingErrorCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOnUpdatingErrorCallbackFacetNull() => TestSerializeOnUpdatingErrorCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOptionalFacet() => TestSerializeOptionalFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOptionalFacetDefault() => TestSerializeOptionalFacetDefault(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePageSizeFacetAnnotation() => TestSerializePageSizeFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePageSizeFacetDefault() => TestSerializePageSizeFacetDefault(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeParseableFacetUsingParser() => TestSerializeParseableFacetUsingParser(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePasswordFacet() => TestSerializePasswordFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePluralFacetAnnotation() => TestSerializePluralFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePluralFacetInferred() => TestSerializePluralFacetInferred(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePresentationHintFacet() => TestSerializePresentationHintFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyAccessorFacet() => TestSerializePropertyAccessorFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyDefaultFacetAnnotation() => TestSerializePropertyDefaultFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyDefaultFacetNone() => TestSerializePropertyDefaultFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyInitializationFacet() => TestSerializePropertyInitializationFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertySetterFacetViaSetterMethod() => TestSerializePropertySetterFacetViaSetterMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyValidateFacetDefault() => TestSerializePropertyValidateFacetDefault(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyValidateFacetNone() => TestSerializePropertyValidateFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeRangeFacet() => TestSerializeRangeFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeRedirectedFacet() => TestSerializeRedirectedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeRegExFacet() => TestSerializeRegExFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeRestExtensionFacet() => TestSerializeRestExtensionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeSaveFacet() => TestSerializeSaveFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTableViewFacet() => TestSerializeTableViewFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTitleFacetNone() => TestSerializeTitleFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTitleFacetUsingParser() => TestSerializeTitleFacetUsingParser(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTitleFacetViaToStringMethodNull() => TestSerializeTitleFacetViaToStringMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTitleFacetViaToStringMethod() => TestSerializeTitleFacetViaMaskedToStringMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTypeFacet() => TestSerializeTypeFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTypeIsAbstractFacet() => TestSerializeTypeIsAbstractFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTypeIsInterfaceFacet() => TestSerializeTypeIsInterfaceFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTypeIsSealedFacet() => TestSerializeTypeIsSealedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTypeIsStaticFacet() => TestSerializeTypeIsStaticFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTypeIsVoidFacet() => TestSerializeTypeIsVoidFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTypeOfFacetDefaultToObject() => TestSerializeTypeOfFacetDefaultToObject(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTypeOfFacetInferredFromArray() => TestSerializeTypeOfFacetInferredFromArray(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTypeOfFacetInferredFromGenerics() => TestSerializeTypeOfFacetInferredFromGenerics(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeValidateObjectFacet() => TestSerializeValidateObjectFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeValidateObjectFacetNull() => TestSerializeValidateObjectFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeValidateProgrammaticUpdatesFacetAnnotation() => TestSerializeValidateProgrammaticUpdatesFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeValueFacet() => TestSerializeValueFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeValueFacetFromSemanticProvider() => TestSerializeValueFacetFromSemanticProvider(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDescribedAsFacetI18N() => TestSerializeDescribedAsFacetI18N(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMemberNamedFacetI18N() => TestSerializeMemberNamedFacetI18N(BinaryRoundTrip);
}