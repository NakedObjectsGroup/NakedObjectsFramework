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
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedFramework.Metamodel.Test.SemanticsProvider;

[TestClass]
public class DateTimeValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<DateTime> {
    private DateTimeValueSemanticsProvider adapter;

    private void AssertEntry(string entry, int year, int month, int day, int hour, int minute, int second) {
        var obj = adapter.ParseTextEntry(entry);
        Assert.AreEqual(new DateTime(year, month, day, hour, minute, second), obj);
    }

    [TestMethod]
    public void TestEmptyClears() {
        Assert.IsNull(adapter.ParseTextEntry(""));
    }

    [TestMethod]
    public void TestEntryWithLongISOFormat() {
        var dt = new DateTime(2007, 5, 21, 10, 30, 0);
        dt = dt.ToUniversalTime();
        var entry = dt.ToString("u");
        AssertEntry(entry, 2007, 5, 21, 10, 30, 0);
    }

    [TestMethod]
    public void TestEntryWithMediumFormat() {
        var dt = new DateTime(2007, 5, 21, 10, 30, 0);
        var entry = dt.ToString("f");
        // "21-May-2007 10:30"
        AssertEntry(entry, 2007, 5, 21, 10, 30, 0);
    }

    [TestMethod]
    public void TestEntryWithShortFormat() {
        var dt = new DateTime(2007, 5, 21, 10, 30, 0);
        var entry = dt.ToString("g");
        const int year = 2007;
        const int month = 5;
        const int day = 21;
        const int hour = 10;
        const int minute = 30;
        AssertEntry(entry, year, month, day, hour, minute, 0);
    }

    [TestMethod]
    public override void TestParseNull() {
        base.TestParseNull();
    }

    [TestMethod]
    public override void TestParseEmptyString() {
        base.TestParseEmptyString();
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(adapter = DateTimeValueSemanticsProvider.Instance);
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.