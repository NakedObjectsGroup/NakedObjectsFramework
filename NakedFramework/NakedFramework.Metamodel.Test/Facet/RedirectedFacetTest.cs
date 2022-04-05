// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Facet;
using NakedFramework.Metamodel.Facet;

namespace NakedFramework.Metamodel.Test.Facet;

public class TestClass {
    public const string Sid = "Server Id";
    public const string Oid = "Object Id";

    public string OidName => Oid;
    public string ServerName => Sid;
}

[TestClass]
public class RedirectedFacetTest {
    private static PropertyInfo GetSn() => typeof(TestClass).GetProperty(nameof(TestClass.ServerName));
    private static PropertyInfo GetOid() => typeof(TestClass).GetProperty(nameof(TestClass.OidName));

    [TestMethod]
    public void TestRedirectedFacet() {
        IRedirectedFacet facet = new RedirectedFacet(GetSn(), GetOid(), null);
        var address = facet.GetRedirection(new TestClass());
        Assert.AreEqual((TestClass.Sid, TestClass.Oid), address);
    }
}