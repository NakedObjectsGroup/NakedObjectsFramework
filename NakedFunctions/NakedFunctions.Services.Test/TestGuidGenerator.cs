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
public class TestGuidGenerator {
    [TestMethod]
    public void Test1() {
        IGuidGenerator gen = new GuidGenerator();
        var g1 = gen.NewGuid();
        Assert.AreNotEqual(Guid.Empty, g1);
        Assert.AreNotEqual(Guid.NewGuid(), g1);

        var g2 = gen.NewGuid();
        Assert.AreNotEqual(g1, g2);
    }
}