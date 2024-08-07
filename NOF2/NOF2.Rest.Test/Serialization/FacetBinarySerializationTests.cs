﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NOF2.Rest.Test.Serialization.FacetSerializationTests;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NOF2.Rest.Test.Serialization;

[TestClass]
public class FacetBinarySerializationTests {
    [TestMethod]
    public void TestBinarySerializeActionChoicesViaAboutMethodFacet() => TestSerializeActionChoicesViaAboutMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionDefaultsViaAboutMethodFacet() => TestSerializeActionDefaultsViaAboutMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionInvocationFacetViaMethod() => TestSerializeActionInvocationFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionInvocationFacetViaStaticMethod() => TestSerializeActionInvocationFacetViaStaticMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionValidateViaAboutMethodFacet() => TestSerializeActionValidateViaAboutMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDescribedAsViaAboutMethodFacet() => TestSerializeDescribedAsViaAboutMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisableForContextViaAboutMethodFacet() => TestSerializeDisableForContextViaAboutMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeHideForContextViaAboutMethodFacet() => TestSerializeHideForContextViaAboutMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMemberNamedViaAboutMethodFacet() => TestSerializeMemberNamedViaAboutMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMenuFacetViaMethod() => TestSerializeMenuFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyChoicesViaAboutMethodFacet() => TestSerializePropertyChoicesViaAboutMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyValidateViaAboutMethodFacet() => TestSerializePropertyValidateViaAboutMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeSaveNullFacet() => TestSerializeSaveNullFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeSaveViaActionSaveFacet() => TestSerializeSaveViaActionSaveFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeSaveViaActionSaveWithAboutFacet() => TestSerializeSaveViaActionSaveWithAboutFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeStaticFunctionFacet() => TestSerializeStaticFunctionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeStaticMenuMethodFacet() => TestSerializeStaticMenuMethodFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeTitleFacetViaTitleMethod() => TestSerializeTitleFacetViaTitleMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeValueHolderWrappingValueSemanticsProvider() => TestSerializeValueHolderWrappingValueSemanticsProvider(BinaryRoundTrip);
}