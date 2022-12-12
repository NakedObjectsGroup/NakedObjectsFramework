// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Test.Data;

namespace ROSI.Test.ApiTests;

public class ObjectApiTests : AbstractApiTests {
    [Test]
    public void TestObject() {
        var objectRep = GetObject(FullName<Class>(), "1");
        Assert.AreEqual(FullName<Class>(), objectRep.GetDomainType());
        Assert.AreEqual("1", objectRep.GetInstanceId());
        Assert.AreEqual("Untitled Class", objectRep.GetTitle());
    }

    [Test]
    public void TestGetCollections() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetCollections();

        Assert.AreEqual(2, val.Count());
    }

    [Test]
    public void TestGetActions() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetActions();

        Assert.AreEqual(2, val.Count());
    }

    [Test]
    public void TestGetProperties() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetProperties();

        Assert.AreEqual(3, val.Count());
    }


    [Test]
    public void TestGetProperty() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetProperty("Property1");

        Assert.IsNotNull(val);
    }

    [Test]
    public void TestGetAction() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetAction("Action1");

        Assert.IsNotNull(val);
    }

    [Test]
    public void TestGetCollection() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetCollection("Collection1");

        Assert.IsNotNull(val);
    }
}