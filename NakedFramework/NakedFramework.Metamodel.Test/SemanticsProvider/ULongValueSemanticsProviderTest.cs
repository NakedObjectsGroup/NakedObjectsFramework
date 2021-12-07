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
public class ULongValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<ulong> {
    private ISpecification holder;
    private ulong l;
    private ULongValueSemanticsProvider value;

   

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
        Assert.AreEqual(120UL, newValue);
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
        Assert.AreEqual(120UL, newValue);
    }

    [TestMethod]
    public void TestTitleString() {
        Assert.AreEqual("32", value.DisplayTitleOf(l));
    }

    [TestMethod]
    public override void TestParseNull() {
        base.TestParseNull();
    }

   

    [TestMethod]
    public void TestValue() {
        var facet = (IUnsignedLongValueFacet)GetValue();
        const ulong testValue = 121L;
        var mockNo = new Mock<INakedObjectAdapter>();
        mockNo.Setup(no => no.Object).Returns(testValue);
        Assert.AreEqual(testValue, facet.UnsignedLongValue(mockNo.Object));
    }

    [TestMethod]
    public void TestAsParserTitle() {
        IParseableFacet parser = new ParseableFacetUsingParser<ulong>(value, null);
        var mockAdapter = MockAdapter(101UL);
        Assert.AreEqual("101", parser.ParseableTitle(mockAdapter));
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        l = 32;
        holder = new Mock<ISpecification>().Object;
        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(value = new ULongValueSemanticsProvider(spec, holder));
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.