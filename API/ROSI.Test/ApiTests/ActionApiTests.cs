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
using ROSI.Test.Data;

namespace ROSI.Test.ApiTests;

public class ActionApiTests : AbstractApiTests {
    [Test]
    public void TestGetLinks() {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var action = parsedResult.GetAction(nameof(Class.Action1));

        var links = action.GetLinks();
        Assert.AreEqual(2, links.Count());
    }

    [Test]
    public void TestGetProperties() {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var action = parsedResult.GetAction(nameof(Class.Action1));

        Assert.AreEqual("action", action.GetMemberType());
        Assert.AreEqual(nameof(Class.Action1), action.GetId());
    }

    [Test]
    public void TestGetExtensions() {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var action = parsedResult.GetAction(nameof(Class.Action1));

        var extensions = action.GetExtensions();
        Assert.AreEqual(5, extensions.Extensions().Count());
    }

    [Test]
    public void TestGetDetails() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = objectRep.GetAction(nameof(Class.Action1)).GetDetails().Result;
        Assert.IsNotNull(details);
    }

    [Test]
    public void TestInvokeNoParmReturnsObjectAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionNoParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke().Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeNoParmReturnsListAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionNoParmsReturnsList));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke().Result;

        Assert.AreEqual(ActionResultApi.ResultType.list, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var l = ar.GetList();

        var v = l.GetValue();
        Assert.AreEqual(2, v.Count());

        var llinks = l.GetLinks();
        Assert.AreEqual(0, llinks.Count());
    }

    [Test]
    public void TestInvokeNoParmReturnsVoidAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionNoParmsReturnsVoid));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke().Result;

        Assert.AreEqual(ActionResultApi.ResultType.@void, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        //Assert.IsNull(ar.GetObject());
        //Assert.IsNull(ar.GetList());
    }

    [Test]
    public void TestInvokeWithValueParmsReturnsObjectAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithValueParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(1, "test").Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithRefParmsReturnsObjectAction() {
        var o1 = GetObject(FullName<Class>(), "1");
        var o2 = GetObject(FullName<Class>(), "2");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithRefParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(o1, o2).Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithMixedParmsReturnsObjectAction() {
        var o1 = GetObject(FullName<Class>(), "1");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithMixedParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(2, o1).Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithValueParmsReturnsObjectIdempotentAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.IdempotentActionWithValueParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Put, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(1, "test").Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithRefParmsReturnsObjectIdempotentAction() {
        var o1 = GetObject(FullName<Class>(), "1");
        var o2 = GetObject(FullName<Class>(), "2");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.IdempotentActionWithRefParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Put, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(o1, o2).Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithMixedParmsReturnsObjectIdempotentAction() {
        var o1 = GetObject(FullName<Class>(), "1");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.IdempotentActionWithMixedParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Put, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(2, o1).Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithValueParmsReturnsObjectPotentAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithValueParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Post, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(1, "test").Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithRefParmsReturnsObjectPotentAction() {
        var o1 = GetObject(FullName<Class>(), "1");
        var o2 = GetObject(FullName<Class>(), "2");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithRefParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Post, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(o1, o2).Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithMixedParmsReturnsObjectPotentAction() {
        var o1 = GetObject(FullName<Class>(), "1");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithMixedParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Post, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(2, o1).Result;

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }
}