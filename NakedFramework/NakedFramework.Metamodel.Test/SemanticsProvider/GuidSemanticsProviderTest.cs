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
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider;

[TestClass]
public class GuidValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<Guid> {
    private Guid guidObj;
    private ISpecification holder;
    private GuidValueSemanticsProvider value;

    [TestMethod]
    public void TestParseValidString() {
        var guid = Guid.NewGuid();
        var str = guid.ToString();
        var parsed = (Guid)value.ParseTextEntry(str);
        Assert.AreEqual(guid, parsed);
    }

    [TestMethod]
    public void TestParseInvalidString() {
        try {
            value.ParseTextEntry("xs21z4xxx23");
            Assert.Fail();
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
        }
    }

    [TestMethod]
    public void TestTitleOf() {
        var guid = Guid.NewGuid();
        var str = guid.ToString();
        Assert.AreEqual(str, value.DisplayTitleOf(guid));
    }

    [TestMethod]
    public void TestEncode() {
        Assert.AreEqual(guidObj.ToString(), value.ToEncodedString(guidObj));
    }

    [TestMethod]
    public void TestDecode() {
        var parsed = value.FromEncodedString(guidObj.ToString());
        Assert.AreEqual(guidObj, parsed);
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
    public override void TestParseNull() {
        base.TestParseNull();
    }

    [TestMethod]
    public override void TestDecodeNull() {
        base.TestDecodeNull();
    }

    [TestMethod]
    public override void TestEmptyEncoding() {
        base.TestEmptyEncoding();
    }

    [TestMethod]
    public void TestValue() {
        IGuidValueFacet facet = value;
        var testValue = Guid.NewGuid();
        var mockNo = new Mock<INakedObjectAdapter>();
        mockNo.Setup(no => no.Object).Returns(testValue);
        Assert.AreEqual(testValue, facet.GuidValue(mockNo.Object));
    }

    [TestMethod]
    public void TestAsParserInvariant() {
        var mgr = MockNakedObjectManager();
        var testValue = Guid.NewGuid();
        IParseableFacet parser = new ParseableFacetUsingParser<Guid>(value, null);
        Assert.AreEqual(testValue, parser.ParseInvariant(testValue.ToString(), mgr.Object).Object);
    }

    [TestMethod]
    public void TestAsParserTitle() {
        IParseableFacet parser = new ParseableFacetUsingParser<Guid>(value, null);
        var testValue = Guid.NewGuid();
        var mockAdapter = MockAdapter(testValue);
        Assert.AreEqual(testValue.ToString(), parser.ParseableTitle(mockAdapter));
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        guidObj = Guid.NewGuid();
        holder = new Mock<ISpecification>().Object;
        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(value = new GuidValueSemanticsProvider(spec, holder));
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}