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

public class ActionApiTests : ApiTests
{
    [Test]
    public void TestGetLinks()
    {
        var parsedResult = GetObject(FullName<Class>(), "1");
        var action = parsedResult.GetAction(nameof(Class.Action1));

        var links = action.GetLinks();

        Assert.AreEqual(2, links.Count());
    }

    [Test]
    public void TestInvokeNoParmReturnsObjectAction()
    {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionNoParmsReturnsObject));

        var ar = action.Invoke();

        Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

        var o = ar.GetObject();

        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeNoParmReturnsListAction()
    {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionNoParmsReturnsList));

        var ar = action.Invoke();

        Assert.AreEqual(ActionResultApi.ResultType.list, ar.GetResultType());

        var l = ar.GetList().GetValue();

        Assert.AreEqual(1, l.Count());
    }
}