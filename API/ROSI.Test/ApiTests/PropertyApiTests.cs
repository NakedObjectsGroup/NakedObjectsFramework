// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Test.Data;

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
        Assert.AreEqual("Class:1", link.GetTitle());
    }

    [Test]
    public void TestGetDetails() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = objectRep.GetProperty(nameof(Class.Property1)).GetDetails(TestInvokeOptions()).Result;
        Assert.IsNotNull(details);
    }

    [Test]
    public void TestGetScalarChoicesFromDetails() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = objectRep.GetProperty(nameof(Class.PropertyWithScalarChoices)).GetDetails(TestInvokeOptions()).Result;
        Assert.IsNotNull(details);

        Assert.IsTrue(details.GetHasChoices());

        var choices = details.GetChoices<int>();

        Assert.AreEqual(3, choices.Count());

        var ext = details.GetExtensions().GetExtension<Dictionary<string, object>>(ExtensionsApi.ExtensionKeys.x_ro_nof_choices);

        Assert.AreEqual(3, ext.Count());

        Assert.AreEqual("Choice One", ext.Keys.First());
        Assert.AreEqual(0, ext.Values.First());
    }

    [Test]
    public void TestGetLinkChoicesFromDetails() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = objectRep.GetProperty(nameof(Class.Property3)).GetDetails(TestInvokeOptions()).Result;
        Assert.IsNotNull(details);

        Assert.IsTrue(details.GetHasChoices());

        var choices = details.GetLinkChoices();

        Assert.AreEqual(2, choices.Count());

        Assert.AreEqual("Class:1", choices.First().GetTitle());
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", choices.First().GetHref().ToString());
    }

    [Test]
    public void TestGetScalarChoices() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var property = objectRep.GetProperty(nameof(Class.PropertyWithScalarChoices));
        Assert.IsNotNull(property);

        Assert.IsTrue(property.GetHasChoices(TestInvokeOptions()).Result);

        var choices = property.GetChoices<int>(TestInvokeOptions()).Result;

        Assert.AreEqual(3, choices.Count());

        var ext = property.GetExtensions().GetExtension<Dictionary<string, object>>(ExtensionsApi.ExtensionKeys.x_ro_nof_choices);

        Assert.AreEqual(3, ext.Count());

        Assert.AreEqual("Choice One", ext.Keys.First());
        Assert.AreEqual(0, ext.Values.First());
    }

    [Test]
    public void TestGetLinkChoices() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var property = objectRep.GetProperty(nameof(Class.Property3));
        Assert.IsNotNull(property);

        Assert.IsTrue(property.GetHasChoices(TestInvokeOptions()).Result);

        var choices = property.GetLinkChoices(TestInvokeOptions()).Result;

        Assert.AreEqual(2, choices.Count());

        Assert.AreEqual("Class:1", choices.First().GetTitle());
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", choices.First().GetHref().ToString());
    }

    [Test]
    public void TestSetPropertyValue() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = objectRep.GetProperty(nameof(Class.Property1)).GetDetails(TestInvokeOptions()).Result;

        var result = details.SetValue("new", TestInvokeOptions()).Result;
        Assert.IsNotNull(result);
        Assert.AreEqual("new", result.GetValue<string>());
    }

    [Test]
    public void TestGetPrompts() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = objectRep.GetProperty(nameof(Class.PropertyWithAutoComplete)).GetDetails(TestInvokeOptions()).Result;
        Assert.IsNotNull(details);

        var prompts = details.GetPrompts<string>(TestInvokeOptions(), "search").Result;

        Assert.AreEqual(1, prompts.Count());

        Assert.AreEqual("search", prompts.First());
    }
}