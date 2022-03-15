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
public class FloatValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<float> {
    private float floatObj;

    [TestMethod]
    public void TestInvalidParse() {
        try {
            GetValue().ParseTextEntry("one");
            Assert.Fail();
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
        }
    }

    [TestMethod]
    public void TestParse() {
        var newValue = GetValue().ParseTextEntry("120.56");
        Assert.AreEqual(120.56F, newValue);
    }

    [TestMethod]
    public void TestParse2() {
        var newValue = GetValue().ParseTextEntry("1,20.0");
        Assert.AreEqual(120F, newValue);
    }

    [TestMethod]
    public override void TestParseEmptyString() {
        try {
            var newValue = GetValue().ParseTextEntry("");
            Assert.IsNull(newValue);
        }
        catch (Exception) {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void TestTitleOf() {
        Assert.AreEqual("3500000", GetValue().DisplayTitleOf(3500000.0F));
    }

    [TestMethod]
    public void TestTitleOfWithMantissa() {
        Assert.AreEqual("32.5", GetValue().DisplayTitleOf(floatObj));
    }

    [TestMethod]
    public override void TestParseNull() {
        base.TestParseNull();
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();

        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(FloatValueSemanticsProvider.Instance);

        floatObj = 32.5F;
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.