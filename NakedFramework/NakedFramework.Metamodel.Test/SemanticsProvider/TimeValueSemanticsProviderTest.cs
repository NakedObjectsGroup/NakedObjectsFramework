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
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider;

[TestClass]
public class TimeValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<TimeSpan> {
    private TimeValueSemanticsProvider adapter;
    private ISpecification holder;
    private TimeSpan time;

    [TestMethod]
    public void TestParseInvariant() {
        var d1 = new TimeSpan(1, 5, 1, 25);
        var s1 = d1.ToString(null, CultureInfo.InvariantCulture);
        var d2 = adapter.ParseInvariant(s1);
        Assert.AreEqual(d1, d2);
    }

    [TestMethod]
    public override void TestParseNull() {
        base.TestParseNull();
    }

    [TestMethod]
    public override void TestParseEmptyString() {
        base.TestParseEmptyString();
    }


    [TestMethod]
    public void TestValue() {
        var facet = (ITimeValueFacet)GetValue();
        var testValue = TimeSpan.FromHours(22);
        var mockNo = new Mock<INakedObjectAdapter>();
        mockNo.Setup(no => no.Object).Returns(testValue);
        Assert.AreEqual(testValue, facet.TimeValue(mockNo.Object));
    }

    [TestMethod]
    public void TestAsParserInvariant() {
        var mgr = MockNakedObjectManager();
        IParseableFacet parser = new ParseableFacetUsingParser<TimeSpan>(GetValue(), null);
        var parsed = parser.ParseInvariant("08:13:00", mgr.Object).Object;
        Assert.AreEqual(time, parsed);
    }

    [TestMethod]
    public void TestAsParserTitle() {
        IParseableFacet parser = new ParseableFacetUsingParser<TimeSpan>(GetValue(), null);
        var mockAdapter = MockAdapter(time);
        var str = DateTime.Today.Add(time).ToShortTimeString();
        Assert.AreEqual(str, parser.ParseableTitle(mockAdapter));
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        time = new TimeSpan(8, 13, 0);
        holder = new Mock<ISpecification>().Object;
        var spec = new Mock<IObjectSpecImmutable>().Object;
        SetValue(adapter = new TimeValueSemanticsProvider(spec, holder));
    }

    [TestCleanup]
    public override void TearDown() {
        base.TearDown();
    }

    #endregion
}

public class TestClock {
    public static long GetTicks() => new DateTime(2003, 8, 17, 21, 30, 25).Ticks;
}

// Copyright (c) Naked Objects Group Ltd.