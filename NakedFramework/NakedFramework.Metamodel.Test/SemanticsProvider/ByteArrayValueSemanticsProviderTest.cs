// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider;

[TestClass]
public class ByteArrayValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<byte[]> {
    private object byteArray;
    private ISpecification specification;
    private ArrayValueSemanticsProvider<byte> value;

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
    public void TestParseInvalidString() {
        try {
            value.ParseTextEntry("fred");
            Assert.Fail("Invalid string");
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
        }
    }

    [TestMethod]
    public void TestParseOutOfRangeString() {
        try {
            value.ParseTextEntry("1 2 1000");
            Assert.Fail("out of range string");
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
        }
    }

    [TestMethod]
    public void TestParseString() {
        var parsed = (byte[])value.ParseTextEntry("0 0 1 100 255");
        Assert.IsTrue(parsed.SequenceEqual(new byte[] { 0, 0, 1, 100, 255 }));
    }

    [TestMethod]
    public void TestTitle() {
        Assert.AreEqual("1 2 100", value.DisplayTitleOf(new byte[] { 1, 2, 100 }));
    }

    [TestMethod]
    public void TestTitleEmpty() {
        Assert.AreEqual("", value.DisplayTitleOf(new byte[] { }));
    }

    [TestMethod]
    public override void TestParseNull() {
        base.TestParseNull();
    }

    [TestMethod]
    public void TestArrayValue() {
        var testArray = new byte[] { 1, 2, 101 };
        var mockNo = new Mock<INakedObjectAdapter>();
        mockNo.Setup(no => no.Object).Returns(testArray);
        IArrayValueFacet<byte> valueFacet = value;

        Assert.AreEqual(testArray, valueFacet.ArrayValue(mockNo.Object));
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        byteArray = new byte[0];
        CreateAdapter(byteArray);
        specification = new Mock<ISpecification>().Object;
        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(value = new ArrayValueSemanticsProvider<byte>(spec, specification));
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.