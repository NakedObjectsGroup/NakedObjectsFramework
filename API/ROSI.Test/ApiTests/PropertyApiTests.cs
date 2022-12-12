// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Helpers;
using ROSI.Test.Data;
using ROSI.Test.Helpers;

namespace ROSI.Test.ApiTests;

public class PropertyApiTests : AbstractApiTests {
    [Test]
    public void TestGetLinks() {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var property = parsedResult.GetProperty(nameof(Class.Property1));

        var links = property.GetLinks();
        Assert.AreEqual(2, links.Count());
    }

    [Test]
    public void TestGetProperties() {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var property = parsedResult.GetProperty(nameof(Class.Property1));

        Assert.AreEqual("property", property.GetMemberType());
        Assert.AreEqual(nameof(Class.Property1), property.GetId());
    }

    
    [Test]
    public void TestGetExtensions() {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var action = parsedResult.GetProperty(nameof(Class.Property1));

        var extensions = action.GetExtensions();
        Assert.AreEqual(8, extensions.Extensions().Count());
    }

    [Test]
    public void TestPropertyScalarValue() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var val = objectRep.GetProperty(nameof(Class.Property1)).GetValue<string>();

        Assert.AreEqual("One", val);
    }

    [Test]
    public void TestPropertyReferenceValue() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var link = objectRep.GetProperty(nameof(Class.Property3)).GetLinkValue();

        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", link.GetHref().ToString());
        Assert.AreEqual("Untitled Class", link.GetTitle());
    }

    [Test]
    public void TestGetDetails() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = objectRep.GetProperty(nameof(Class.Property1)).GetDetails().Result;
        Assert.IsNotNull(details);
    }

    [Test]
    public void TestSetPropertyValue() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = objectRep.GetProperty(nameof(Class.Property1)).GetDetails().Result;

        HttpHelpers.Client = new HttpClient(new StubHttpMessageHandler(Api()));

        var result = details.SetValue("new").Result;
        Assert.IsNotNull(result);
        Assert.AreEqual("new", result.GetValue<string>());
       
    }
}