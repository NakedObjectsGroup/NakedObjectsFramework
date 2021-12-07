// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider;

[TestClass]
public class DecimalValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<decimal> {
    private decimal dec;
    private ISpecification holder;
    private DecimalValueSemanticsProvider value;

    [TestMethod]
    public void TestInvalidParse() {
        try {
            value.ParseTextEntry("one");
            Assert.Fail();
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
        }
    }

    [TestMethod]
    public void TestParse() {
        var newValue = value.ParseTextEntry("120");
        Assert.AreEqual(120M, newValue);
    }

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
    public void TestParseOddlyFormedEntry() {
        var newValue = value.ParseTextEntry("1,20.0");
        Assert.AreEqual(120.0M, newValue);
    }

    [TestMethod]
    public void TestTitleString() {
        Assert.AreEqual("32", value.DisplayTitleOf(dec));
    }

    [TestMethod]
    public override void TestParseNull() {
        base.TestParseNull();
    }

  

    [TestMethod]
    public void TestValue() {
        var facet = (IDecimalValueFacet)GetValue();
        const decimal testValue = 121M;
        var mockNo = new Mock<INakedObjectAdapter>();
        mockNo.Setup(no => no.Object).Returns(testValue);
        Assert.AreEqual(testValue, facet.DecimalValue(mockNo.Object));
    }

    [TestMethod]
    public void TestAsParserTitle() {
        IParseableFacet parser = new ParseableFacetUsingParser<decimal>(value, null);
        var mockAdapter = MockAdapter(101M);
        Assert.AreEqual("101", parser.ParseableTitle(mockAdapter));
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        dec = 32;
        holder = new Mock<ISpecification>().Object;
        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(value = new DecimalValueSemanticsProvider(spec, holder));
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.