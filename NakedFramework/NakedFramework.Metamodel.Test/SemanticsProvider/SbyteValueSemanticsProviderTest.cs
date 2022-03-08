// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedFramework.Metamodel.Test.SemanticsProvider;

[TestClass]
public class SbyteValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<sbyte> {
    private sbyte byteObj;
    private SbyteValueSemanticsProvider value;

    [TestMethod]
    public void TestParseValidString() {
        var parsed = value.ParseTextEntry("21");
        Assert.AreEqual((sbyte)21, parsed);
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
        Assert.AreEqual("102", value.DisplayTitleOf(byteObj));
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

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        byteObj = 102;
        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(value = new SbyteValueSemanticsProvider(spec));
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}