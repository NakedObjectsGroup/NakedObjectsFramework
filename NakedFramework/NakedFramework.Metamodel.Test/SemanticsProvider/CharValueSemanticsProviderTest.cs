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
public class CharValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<char> {
    private char character;
    private ISpecification holder;
    private CharValueSemanticsProvider value;

    [TestMethod]
    public void TestDecode() {
        object restore = value.FromEncodedString("Y");
        Assert.AreEqual('Y', restore);
    }

    [TestMethod]
    public void TestEncode() {
        Assert.AreEqual("r", value.ToEncodedString(character));
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
    public void TestParseInvariant() {
        const char c1 = 'z';
        var s1 = c1.ToString(CultureInfo.InvariantCulture);
        var c2 = value.ParseInvariant(s1);
        Assert.AreEqual(c1, c2);
    }

    [TestMethod]
    public void TestParseLongString() {
        try {
            value.ParseTextEntry("one");
            Assert.Fail();
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
        }
    }

    [TestMethod]
    public void TestTitleOf() {
        Assert.AreEqual("r", value.DisplayTitleOf(character));
    }

    [TestMethod]
    public void TestValidParse() {
        var parse = value.ParseTextEntry("t");
        Assert.AreEqual('t', parse);
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
        ICharValueFacet facet = value;
        const char testValue = (char) 101;
        var mockNo = new Mock<INakedObjectAdapter>();
        mockNo.Setup(no => no.Object).Returns(testValue);
        Assert.AreEqual(testValue, facet.CharValue(mockNo.Object));
    }

    [TestMethod]
    public void TestAsParserInvariant() {
        var mgr = MockNakedObjectManager();
        IParseableFacet parser = new ParseableFacetUsingParser<char>(value, null);
        Assert.AreEqual('t', parser.ParseInvariant("t", mgr.Object).Object);
    }

    [TestMethod]
    public void TestAsParserTitle() {
        IParseableFacet parser = new ParseableFacetUsingParser<char>(value, null);
        var mockAdapter = MockAdapter('t');
        Assert.AreEqual("t", parser.ParseableTitle(mockAdapter));
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        character = 'r';
        holder = new Mock<ISpecification>().Object;
        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(value = new CharValueSemanticsProvider(spec, holder));
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}