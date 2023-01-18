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

public class CollectionApiTests : AbstractApiTests {
    [Test]
    public void TestGetLinks() {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var collection = parsedResult.GetCollection(nameof(Class.Collection1));

        var links = collection.GetLinks();
        Assert.AreEqual(2, links.Count());
    }

    [Test]
    public void TestGetProperties() {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var collection = parsedResult.GetCollection(nameof(Class.Collection1));

        Assert.AreEqual("collection", collection.GetMemberType());
        Assert.AreEqual(nameof(Class.Collection1), collection.GetId());
        Assert.AreEqual(0, collection.GetSize());
    }

    [Test]
    public void TestGetExtensions() {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var collection = parsedResult.GetCollection(nameof(Class.Collection1));

        var extensions = collection.GetExtensions();
        Assert.AreEqual(8, extensions.Extensions().Count());
    }

    [Test]
    public void TestGetValueFromMember() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var links = objectRep.GetCollection(nameof(Class.Collection1)).GetValue(TestInvokeOptions()).Result;

        Assert.AreEqual(0, links.Count());
    }

    [Test]
    public void TestDetails() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = objectRep.GetCollection(nameof(Class.Collection1)).GetDetails(TestInvokeOptions()).Result;
        Assert.IsNotNull(details);
    }
}