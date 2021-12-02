// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NUnit.Framework;

namespace NakedObjects.SystemTest.Util;

[TestFixture]
public class DateTimeExtensionsTest {
    [Test]
    public void TestEndOfMonth() {
        var wellKnownMonth1 = new DateTime(2009, 2, 11);
        var endOfWellKnownMonth1 = new DateTime(2009, 2, 28);
        var wellKnownMonth2 = new DateTime(2007, 10, 20);
        var endOfWellKnownMonth2 = new DateTime(2007, 10, 31);

        Assert.IsTrue(wellKnownMonth1.EndOfMonth().Equals(endOfWellKnownMonth1));
        Assert.IsTrue(wellKnownMonth2.EndOfMonth().Equals(endOfWellKnownMonth2));
        Assert.IsFalse(wellKnownMonth1.EndOfMonth().Equals(endOfWellKnownMonth2));
        Assert.IsFalse(wellKnownMonth2.EndOfMonth().Equals(endOfWellKnownMonth1));
    }

    [Test]
    public void TestEndOfWeek() {
        var wellKnownWeek1 = new DateTime(2009, 2, 11);
        var endOfWellKnownWeek1 = new DateTime(2009, 2, 14);
        var wellKnownWeek2 = new DateTime(2007, 10, 20);
        var endOfWellKnownWeek2 = new DateTime(2007, 10, 20);

        Assert.IsTrue(wellKnownWeek1.EndOfWeek().Equals(endOfWellKnownWeek1));
        Assert.IsTrue(wellKnownWeek2.EndOfWeek().Equals(endOfWellKnownWeek2));
        Assert.IsFalse(wellKnownWeek1.EndOfWeek().Equals(endOfWellKnownWeek2));
        Assert.IsFalse(wellKnownWeek2.EndOfWeek().Equals(endOfWellKnownWeek1));
    }

    [Test]
    public void TestEndOfYear() {
        var wellKnownYear1 = new DateTime(2009, 12, 31);
        var endOfWellKnownYear1 = new DateTime(2009, 12, 31);
        var wellKnownYear2 = new DateTime(2007, 12, 31);
        var endOfWellKnownYear2 = new DateTime(2007, 12, 31);

        Assert.IsTrue(wellKnownYear1.EndOfYear().Equals(endOfWellKnownYear1));
        Assert.IsTrue(wellKnownYear2.EndOfYear().Equals(endOfWellKnownYear2));
        Assert.IsFalse(wellKnownYear1.EndOfYear().Equals(endOfWellKnownYear2));
        Assert.IsFalse(wellKnownYear2.EndOfYear().Equals(endOfWellKnownYear1));
    }

    [Test]
    public void TestIsAfterNullableToday() {
        Enumerable.Range(1, 9).Select(x => DateTime.Now.AddDays(x)).Select(x => new DateTime?(x)).ToList().ForEach(x => Assert.IsTrue(x.IsAfterToday()));

        Enumerable.Range(-10, 9).Select(x => DateTime.Now.AddDays(x)).Select(x => new DateTime?(x)).ToList().ForEach(x => Assert.IsFalse(x.IsAfterToday()));

        Assert.IsFalse(((DateTime?)null).IsAfterToday());
    }

    [Test]
    public void TestIsAfterToday() {
        Enumerable.Range(1, 9).Select(x => DateTime.Now.AddDays(x)).ToList().ForEach(x => Assert.IsTrue(x.IsAfterToday()));

        Enumerable.Range(-10, 9).Select(x => DateTime.Now.AddDays(x)).ToList().ForEach(x => Assert.IsFalse(x.IsAfterToday()));
    }

    [Test]
    public void TestIsBeforeNullableToday() {
        Enumerable.Range(-10, 9).Select(x => DateTime.Now.AddDays(x)).Select(x => new DateTime?(x)).ToList().ForEach(x => Assert.IsTrue(x.IsBeforeToday()));

        Enumerable.Range(1, 9).Select(x => DateTime.Now.AddDays(x)).Select(x => new DateTime?(x)).ToList().ForEach(x => Assert.IsFalse(x.IsBeforeToday()));

        Assert.IsFalse(((DateTime?)null).IsBeforeToday());
    }

    [Test]
    public void TestIsBeforeToday() {
        Enumerable.Range(-10, 9).Select(x => DateTime.Now.AddDays(x)).ToList().ForEach(x => Assert.IsTrue(x.IsBeforeToday()));

        Enumerable.Range(1, 9).Select(x => DateTime.Now.AddDays(x)).ToList().ForEach(x => Assert.IsFalse(x.IsBeforeToday()));
    }

    [Test]
    public void TestIsNullableToday() {
        DateTime? today = DateTime.Now;
        DateTime? tomorrow = today.Value.AddDays(1);
        DateTime? yesterday = today.Value.AddDays(-1);

        Assert.IsTrue(today.IsToday());
        Assert.IsFalse(tomorrow.IsToday());
        Assert.IsFalse(yesterday.IsToday());

        Assert.IsFalse(((DateTime?)null).IsToday());
    }

    [Test]
    public void TestIsSameDayAs() {
        var wellKnownDay = new DateTime(2005, 5, 18);
        var afterWellKnownDay = wellKnownDay.AddDays(1);
        var beforeWellKnownDay = wellKnownDay.AddDays(-1);

        Assert.IsTrue(wellKnownDay.IsSameDayAs(wellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameDayAs(afterWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameDayAs(beforeWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameDayAs(null));
    }

    [Test]
    public void TestIsSameMonthAs() {
        var wellKnownDay = new DateTime(2005, 5, 18);
        var afterWellKnownDay = wellKnownDay.AddMonths(1);
        var beforeWellKnownDay = wellKnownDay.AddMonths(-1);

        Assert.IsTrue(wellKnownDay.IsSameMonthAs(wellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameMonthAs(afterWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameMonthAs(beforeWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameMonthAs(null));
    }

    [Test]
    public void TestIsSameNullableDayAs() {
        DateTime? wellKnownDay = new DateTime(2005, 5, 18);
        DateTime? afterWellKnownDay = wellKnownDay.Value.AddDays(1);
        DateTime? beforeWellKnownDay = wellKnownDay.Value.AddDays(-1);

        Assert.IsTrue(wellKnownDay.IsSameDayAs(wellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameDayAs(afterWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameDayAs(beforeWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameDayAs(null));
        Assert.IsFalse(((DateTime?)null).IsSameDayAs(wellKnownDay));
        Assert.IsFalse(((DateTime?)null).IsSameDayAs(null));
    }

    [Test]
    public void TestIsSameNullableMonthAs() {
        DateTime? wellKnownDay = new DateTime(2005, 5, 18);
        DateTime? afterWellKnownDay = wellKnownDay.Value.AddMonths(1);
        DateTime? beforeWellKnownDay = wellKnownDay.Value.AddMonths(-1);

        Assert.IsTrue(wellKnownDay.IsSameMonthAs(wellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameMonthAs(afterWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameMonthAs(beforeWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameMonthAs(null));
        Assert.IsFalse(((DateTime?)null).IsSameMonthAs(wellKnownDay));
        Assert.IsFalse(((DateTime?)null).IsSameMonthAs(null));
    }

    [Test]
    public void TestIsSameNullableWeekAs() {
        DateTime? wellKnownDay = new DateTime(2005, 5, 18);
        DateTime? afterWellKnownDay = wellKnownDay.Value.AddDays(7);
        DateTime? beforeWellKnownDay = wellKnownDay.Value.AddDays(-7);

        Assert.IsTrue(wellKnownDay.IsSameWeekAs(wellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameWeekAs(afterWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameWeekAs(beforeWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameWeekAs(null));
        Assert.IsFalse(((DateTime?)null).IsSameWeekAs(wellKnownDay));
        Assert.IsFalse(((DateTime?)null).IsSameWeekAs(null));
    }

    [Test]
    public void TestIsSameNullableYearAs() {
        DateTime? wellKnownDay = new DateTime(2005, 5, 18);
        DateTime? afterWellKnownDay = wellKnownDay.Value.AddYears(1);
        DateTime? beforeWellKnownDay = wellKnownDay.Value.AddYears(-1);

        Assert.IsTrue(wellKnownDay.IsSameYearAs(wellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameYearAs(afterWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameYearAs(beforeWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameYearAs(null));
        Assert.IsFalse(((DateTime?)null).IsSameYearAs(wellKnownDay));
        Assert.IsFalse(((DateTime?)null).IsSameYearAs(null));
    }

    [Test]
    public void TestIsSameWeekAs() {
        var wellKnownDay = new DateTime(2005, 5, 18);
        var afterWellKnownDay = wellKnownDay.AddDays(7);
        var beforeWellKnownDay = wellKnownDay.AddDays(-7);

        Assert.IsTrue(wellKnownDay.IsSameWeekAs(wellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameWeekAs(afterWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameWeekAs(beforeWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameWeekAs(null));
    }

    [Test]
    public void TestIsSameYearAs() {
        var wellKnownDay = new DateTime(2005, 5, 18);
        var afterWellKnownDay = wellKnownDay.AddYears(1);
        var beforeWellKnownDay = wellKnownDay.AddYears(-1);

        Assert.IsTrue(wellKnownDay.IsSameYearAs(wellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameYearAs(afterWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameYearAs(beforeWellKnownDay));
        Assert.IsFalse(wellKnownDay.IsSameYearAs(null));
    }

    [Test]
    public void TestIsToday() {
        var today = DateTime.Now;
        var tomorrow = today.AddDays(1);
        var yesterday = today.AddDays(-1);

        Assert.IsTrue(today.IsToday());
        Assert.IsFalse(tomorrow.IsToday());
        Assert.IsFalse(yesterday.IsToday());
    }

    [Test]
    public void TestStartOfMonth() {
        var wellKnownMonth1 = new DateTime(2009, 2, 11);
        var startOfWellKnownMonth1 = new DateTime(2009, 2, 1);
        var wellKnownMonth2 = new DateTime(2007, 10, 20);
        var startOfWellKnownMonth2 = new DateTime(2007, 10, 1);

        Assert.IsTrue(wellKnownMonth1.StartOfMonth().Equals(startOfWellKnownMonth1));
        Assert.IsTrue(wellKnownMonth2.StartOfMonth().Equals(startOfWellKnownMonth2));
        Assert.IsFalse(wellKnownMonth1.StartOfMonth().Equals(startOfWellKnownMonth2));
        Assert.IsFalse(wellKnownMonth2.StartOfMonth().Equals(startOfWellKnownMonth1));
    }

    [Test]
    public void TestStartOfWeek() {
        var wellKnownWeek1 = new DateTime(2009, 2, 11);
        var startOfWellKnownWeek1 = new DateTime(2009, 2, 8);
        var wellKnownWeek2 = new DateTime(2007, 10, 20);
        var startOfWellKnownWeek2 = new DateTime(2007, 10, 14);

        Assert.IsTrue(wellKnownWeek1.StartOfWeek().Equals(startOfWellKnownWeek1));
        Assert.IsTrue(wellKnownWeek2.StartOfWeek().Equals(startOfWellKnownWeek2));
        Assert.IsFalse(wellKnownWeek1.StartOfWeek().Equals(startOfWellKnownWeek2));
        Assert.IsFalse(wellKnownWeek2.StartOfWeek().Equals(startOfWellKnownWeek1));
    }

    [Test]
    public void TestStartOfYear() {
        var wellKnownYear1 = new DateTime(2009, 1, 1);
        var startOfWellKnownYear1 = new DateTime(2009, 1, 1);
        var wellKnownYear2 = new DateTime(2007, 1, 1);
        var startOfWellKnownYear2 = new DateTime(2007, 1, 1);

        Assert.IsTrue(wellKnownYear1.StartOfYear().Equals(startOfWellKnownYear1));
        Assert.IsTrue(wellKnownYear2.StartOfYear().Equals(startOfWellKnownYear2));
        Assert.IsFalse(wellKnownYear1.StartOfYear().Equals(startOfWellKnownYear2));
        Assert.IsFalse(wellKnownYear2.StartOfYear().Equals(startOfWellKnownYear1));
    }
}