// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NakedFramework.Rest.Extensions;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Records;
using ROSI.Test.Data;
using ROSI.Test.Helpers;

namespace ROSI.Test.ApiTests;

public class NonInlinedDetailsApiTests : AbstractApiTests {
    protected override Action<RestfulObjectsOptions> RestfulObjectsOptions => options => {
        options.CacheSettings = (0, 3600, 86400);
        options.InlineDetailsInActionMemberRepresentations = false;
        options.InlineDetailsInPropertyMemberRepresentations = false;
        options.InlineDetailsInCollectionMemberRepresentations = false;
        options.InlinedMemberRepresentations = false;
    };

    [Test]
    public void TestInvokeWithValueParmsReturnsObjectAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithValueParmsReturnsObject));
        Assert.IsFalse(action.GetLinks().HasInvokeLink());

        var ar = action.Invoke(new ForInlineInvokeOptions(this), 1, "test").Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestGetPrompts() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var property = objectRep.GetProperty(nameof(Class.PropertyWithAutoComplete));
        Assert.IsNotNull(property);

        var prompts = property.GetPrompts<string>(new ForInlineInvokeOptions(this), "search").Result;

        Assert.AreEqual(1, prompts.Count());

        Assert.AreEqual("search", prompts.First());
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

    // so it returns a new stub client each time
    protected record ForInlineInvokeOptions : InvokeOptions {
        private readonly AbstractApiTests tc;

        public ForInlineInvokeOptions(AbstractApiTests tc) => this.tc = tc;

        public override HttpClient HttpClient => new(new StubHttpMessageHandler(tc.Api()));
    }
}