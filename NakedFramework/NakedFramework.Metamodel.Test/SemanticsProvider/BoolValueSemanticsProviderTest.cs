// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedFramework.Metamodel.Test.SemanticsProvider;

[TestClass]
public class BoolValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<bool> {
    private INakedObjectAdapter booleanNO;
    private INakedObjectAdapter booleanNO1;
    private object booleanObj;
    private ISpecification specification;
    private BooleanValueSemanticsProvider value;
    private IBooleanValueFacet valueFacet;

    [TestMethod]
    public override void TestParseEmptyString() {
        try {
            var newValue = value.ParseTextEntry("");
            Assert.IsNull(newValue);
        }
        catch (Exception) {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void TestParseFalseString() {
        var parsed = value.ParseTextEntry("faLSe");
        Assert.AreEqual(false, parsed);
    }

    [TestMethod]
    public void TestParseInvalidString() {
        try {
            value.ParseTextEntry("yes");
            Assert.Fail("Invalid string");
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
        }
    }

    [TestMethod]
    public void TestParseTrueString() {
        var parsed = value.ParseTextEntry("TRue");
        Assert.AreEqual(true, parsed);
    }

    [TestMethod]
    public void TestTitleFalse() {
        Assert.AreEqual("False", value.DisplayTitleOf(false));
    }

    [TestMethod]
    public void TestTitleTrue() {
        Assert.AreEqual("True", value.DisplayTitleOf(true));
    }

    [TestMethod]
    public override void TestParseNull() {
        base.TestParseNull();
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        booleanObj = true;
        booleanNO = CreateAdapter(booleanObj);
        booleanNO1 = CreateAdapter(true);
        specification = new Mock<ISpecification>().Object;
        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(value = BooleanValueSemanticsProvider.Instance);
        valueFacet = value;
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.