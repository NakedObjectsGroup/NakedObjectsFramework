// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedFunctions.Services.Test;

[TestClass]
public class TestClock {
    [TestMethod]
    public void TestNow() {
        var c = new Clock();
        var actual = c.Now();
        var expected = DateTime.UtcNow;
        Assert.AreEqual(expected.Date, actual.Date);
        Assert.AreEqual(expected.Hour, actual.Hour);
        Assert.AreEqual(expected.Minute, actual.Minute);
        Assert.IsTrue(Math.Abs(expected.Second - actual.Second) <= 1);
    }

    [TestMethod]
    public void TestToday() {
        var c = new Clock();
        Assert.AreEqual(DateTime.Today, c.Today());
    }
}