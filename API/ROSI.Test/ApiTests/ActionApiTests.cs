// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Exceptions;
using ROSI.Test.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace ROSI.Test.ApiTests;

public class ActionApiTests : AbstractRosiApiTests {
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
    public async Task TestGetDetails() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var details = await objectRep.GetAction(nameof(Class.Action1)).GetDetails(TestInvokeOptions());
        Assert.IsNotNull(details);
    }

    [Test]
    public async Task TestGetParametersOnDetails() {
        var objectRep = GetObject(FullName<ClassWithActions>(), "1");
        var details = await objectRep.GetAction(nameof(ClassWithActions.ActionWithMixedParmsReturnsObject)).GetDetails(TestInvokeOptions());
        Assert.IsNotNull(details);

        var parameters = details.GetParameters().Parameters();

        Assert.AreEqual(2, parameters.Count());

        Assert.AreEqual("index", parameters.Keys.First());
        Assert.AreEqual("class1", parameters.Keys.Last());

        Assert.AreEqual("Index", parameters["index"].GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.friendlyName));
        Assert.AreEqual(0, parameters["index"].GetLinks().Count());
    }

    [Test]
    public void TestGetParameters() {
        var objectRep = GetObject(FullName<ClassWithActions>(), "1");
        var action = objectRep.GetAction(nameof(ClassWithActions.ActionWithMixedParmsReturnsObject));
        Assert.IsNotNull(action);

        var parameters = action.GetParameters(TestInvokeOptions()).Result.Parameters();

        Assert.AreEqual(2, parameters.Count());

        Assert.AreEqual("index", parameters.Keys.First());
        Assert.AreEqual("class1", parameters.Keys.Last());

        Assert.AreEqual("Index", parameters["index"].GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.friendlyName));
        Assert.AreEqual(0, parameters["index"].GetLinks().Count());
    }

    [Test]
    public void TestInvokeNoParmReturnsObjectAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionNoParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action?.GetLinks().GetInvokeLink()?.GetMethod());

        Assert.AreEqual(0, action?.GetLinks().GetInvokeLink()?.GetArguments()?.Count());

        var ar = action.Invoke(TestInvokeOptions()).Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());

        Assert.IsNull(ar.GetList());
        Assert.IsNull(ar.GetScalarValue<int?>());
    }

    [Test]
    public void TestValidateNoParm() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionNoParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action?.GetLinks().GetInvokeLink()?.GetMethod());

        Assert.AreEqual(0, action?.GetLinks().GetInvokeLink()?.GetArguments()?.Count());

        action.Validate(TestInvokeOptions()).Wait();
    }

    [Test]
    public void TestInvokeNoParmReturnsListAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionNoParmsReturnsList));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(TestInvokeOptions()).Result;

        Assert.AreEqual(ActionResultApi.ResultType.List, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var l = ar.GetList();

        var v = l.GetValue();
        Assert.AreEqual(2, v.Count());

        var llinks = l.GetLinks();
        Assert.AreEqual(0, llinks.Count());

        Assert.IsNull(ar.GetObject());
        Assert.IsNull(ar.GetScalarValue<int?>());
    }

    [Test]
    public void TestInvokeNoParmReturnsVoidAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionNoParmsReturnsVoid));
        Assert.AreEqual(HttpMethod.Get, action?.GetLinks().GetInvokeLink()?.GetMethod());

        var ar = action?.Invoke(TestInvokeOptions()).Result;

        Assert.AreEqual(ActionResultApi.ResultType.Void, ar?.GetResultType());

        var links = ar?.GetLinks();
        Assert.AreEqual(1, links?.Count());

        Assert.IsNull(ar?.GetObject());
        Assert.IsNull(ar?.GetList());
        Assert.IsNull(ar?.GetScalarValue<int?>());
        Assert.IsNull(ar?.GetScalarValue<string>());
    }

    [Test]
    public void TestInvokeWithValueParmsReturnsObjectAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithValueParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var args = action?.GetLinks().GetInvokeLink()?.GetArguments();

        Assert.IsNotNull(args);

        Assert.AreEqual(2, args?.Count());

        var ar = action.Invoke(TestInvokeOptions(), 1, "test").Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestValidateWithValueParmsReturnsObjectAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithValueParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var args = action?.GetLinks().GetInvokeLink()?.GetArguments();

        Assert.IsNotNull(args);

        Assert.AreEqual(2, args?.Count());
        action.Validate(TestInvokeOptions(), 1, "test").Wait();
    }

    [Test]
    public void TestInvokeWithNamedValueParmsReturnsObjectAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithValueParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        var args = action?.GetLinks().GetInvokeLink()?.GetArguments();

        Assert.IsNotNull(args);

        Assert.AreEqual(2, args?.Count());

        var ar = action.InvokeWithNamedParams(TestInvokeOptions(), new() {{"index", 1}, {"str", "test"}}    ).Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

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

        var ar = action.Invoke(TestInvokeOptions(), o1, o2).Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

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

        var ar = action.Invoke(TestInvokeOptions(), 2, o1).Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

        var links = ar.GetLinks();
        Assert.AreEqual(1, links.Count());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestValidateWithMixedParms() {
        var o1 = GetObject(FullName<Class>(), "1");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithMixedParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Get, action.GetLinks().GetInvokeLink().GetMethod());

        action.Validate(TestInvokeOptions(), 2, o1).Wait();
    }

    [Test]
    public void TestInvokeWithValueParmsReturnsObjectIdempotentAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.IdempotentActionWithValueParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Put, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(TestInvokeOptions(), 1, "test").Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

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

        var ar = action.Invoke(TestInvokeOptions(), o1, o2).Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithMixedParmsReturnsObjectIdempotentAction() {
        var o1 = GetObject(FullName<Class>(), "1");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.IdempotentActionWithMixedParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Put, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(TestInvokeOptions(), 2, o1).Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithValueParmsReturnsObjectPotentAction() {
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithValueParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Post, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(TestInvokeOptions(), 1, "test").Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

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

        var ar = action.Invoke(TestInvokeOptions(), o1, o2).Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestValidateWithRefParmsPotentAction() {
        var o1 = GetObject(FullName<Class>(), "1");
        var o2 = GetObject(FullName<Class>(), "2");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithRefParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Post, action.GetLinks().GetInvokeLink().GetMethod());

        action.Validate(TestInvokeOptions(), o1, o2).Wait();
    }

    [Test]
    public void TestInvokeWithMixedParmsReturnsObjectPotentAction() {
        var o1 = GetObject(FullName<Class>(), "1");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithMixedParmsReturnsObject));
        Assert.AreEqual(HttpMethod.Post, action.GetLinks().GetInvokeLink().GetMethod());

        var ar = action.Invoke(TestInvokeOptions(), 2, o1).Result;

        Assert.AreEqual(ActionResultApi.ResultType.Object, ar.GetResultType());

        var o = ar.GetObject();
        Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    }

    [Test]
    public void TestInvokeWithWrongParms1() {
        var o1 = GetObject(FullName<ClassWithScalars>(), "1");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithMixedParmsReturnsObject));

        try {
            var ar = action.Invoke(TestInvokeOptions(), 2, o1).Result;
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpInvalidArgumentsRosiException hre) {
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, hre.StatusCode);
                Assert.IsNotNull(hre.Content);
                var args = hre.Content.GetArguments();
                
                Assert.AreEqual(2, args.Count);
                Assert.AreEqual("index", args.First().Key);
                Assert.AreEqual(2, args.First().Value.GetValue());
                Assert.IsNull(args.First().Value.GetInvalidReason());

                Assert.AreEqual("class1", args.Last().Key);
                Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.ClassWithScalars/1", args.Last().Value.GetLinkValue().GetHref().ToString());
                Assert.AreEqual("Not a suitable type; must be a Class", args.Last().Value.GetInvalidReason());

            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }

    }

    [Test]
    public void TestValidateWithWrongParms1() {
        var o1 = GetObject(FullName<ClassWithScalars>(), "1");

        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithMixedParmsReturnsObject));

        try {
            action.Validate(TestInvokeOptions(), 2, o1).Wait();
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpInvalidArgumentsRosiException hre) {
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, hre.StatusCode);
                Assert.IsNotNull(hre.Content);
                var args = hre.Content.GetArguments();
                
                Assert.AreEqual(2, args.Count);
                Assert.AreEqual("index", args.First().Key);
                Assert.AreEqual(2, args.First().Value.GetValue());
                Assert.IsNull(args.First().Value.GetInvalidReason());

                Assert.AreEqual("class1", args.Last().Key);
                Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.ClassWithScalars/1", args.Last().Value.GetLinkValue().GetHref().ToString());
                Assert.AreEqual("Not a suitable type; must be a Class", args.Last().Value.GetInvalidReason());

            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }

    }

    [Test]
    public void TestInvokeWithEmptyMandatoryParms() {
        
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithMixedParmsReturnsObject));

        try {
            var ar = action.Invoke(TestInvokeOptions(), "",  null!).Result;
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpInvalidArgumentsRosiException hre) {
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, hre.StatusCode);
                Assert.AreEqual("199 RestfulObjects \"Mandatory\", 199 RestfulObjects \"Mandatory\"", hre.Message);
                Assert.IsNotNull(hre.Content);
                var args = hre.Content.GetArguments();
                
                Assert.AreEqual(2, args.Count);
                Assert.AreEqual("index", args.First().Key);
                Assert.AreEqual("", args.First().Value.GetValue());
                Assert.AreEqual("Mandatory", args.First().Value.GetInvalidReason());

                Assert.AreEqual("class1", args.Last().Key);
                Assert.AreEqual(null, args.Last().Value.GetValue());
                Assert.AreEqual("Mandatory", args.Last().Value.GetInvalidReason());

            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }

    }

    [Test]
    public void TestValidateWithEmptyMandatoryParms() {
        
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.PotentActionWithMixedParmsReturnsObject));

        try {
            action.Validate(TestInvokeOptions(), "",  null!).Wait();
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpInvalidArgumentsRosiException hre) {
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, hre.StatusCode);
                Assert.AreEqual("199 RestfulObjects \"Mandatory\", 199 RestfulObjects \"Mandatory\"", hre.Message);
                Assert.IsNotNull(hre.Content);
                var args = hre.Content.GetArguments();
                
                Assert.AreEqual(2, args.Count);
                Assert.AreEqual("index", args.First().Key);
                Assert.AreEqual("", args.First().Value.GetValue());
                Assert.AreEqual("Mandatory", args.First().Value.GetInvalidReason());

                Assert.AreEqual("class1", args.Last().Key);
                Assert.AreEqual(null, args.Last().Value.GetValue());
                Assert.AreEqual("Mandatory", args.Last().Value.GetInvalidReason());

            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }

    }

    [Test]
    public void TestInvokeWithInvalidMandatoryParms() {
        
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithValueParmsReturnsObject));

        try {
            var ar = action.Invoke(TestInvokeOptions(), "fred",  "fred").Result;
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpInvalidArgumentsRosiException hre) {
                Assert.AreEqual(HttpStatusCode.BadRequest, hre.StatusCode);
                Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", hre.Message);
                Assert.IsNotNull(hre.Content);
                var args = hre.Content.GetArguments();
                
                Assert.AreEqual(2, args.Count);
                Assert.AreEqual("index", args.First().Key);
                Assert.AreEqual("fred", args.First().Value.GetValue());
                Assert.AreEqual("Invalid Entry", args.First().Value.GetInvalidReason());

                Assert.AreEqual("str", args.Last().Key);
                Assert.AreEqual("fred", args.Last().Value.GetValue());
                Assert.AreEqual(null, args.Last().Value.GetInvalidReason());

            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }

    }

    [Test]
    public void TestValidateWithInvalidMandatoryParms() {
        
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithValueParmsReturnsObject));

        try {
            action.Validate(TestInvokeOptions(), "fred",  "fred").Wait();
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpInvalidArgumentsRosiException hre) {
                Assert.AreEqual(HttpStatusCode.BadRequest, hre.StatusCode);
                Assert.AreEqual("199 RestfulObjects \"Invalid Entry\"", hre.Message);
                Assert.IsNotNull(hre.Content);
                var args = hre.Content.GetArguments();
                
                Assert.AreEqual(2, args.Count);
                Assert.AreEqual("index", args.First().Key);
                Assert.AreEqual("fred", args.First().Value.GetValue());
                Assert.AreEqual("Invalid Entry", args.First().Value.GetInvalidReason());

                Assert.AreEqual("str", args.Last().Key);
                Assert.AreEqual("fred", args.Last().Value.GetValue());
                Assert.AreEqual(null, args.Last().Value.GetInvalidReason());

            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }

    }

    [Test]
    public void TestInvokeWithFailCrossValidation() {
        
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionFailsCrossValidation));

        try {
            var ar = action.Invoke(TestInvokeOptions(), 1,  "fred").Result;
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpInvalidArgumentsRosiException hre) {
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, hre.StatusCode);
                Assert.AreEqual("199 RestfulObjects \"Fail parameter validation\"", hre.Message);
                Assert.IsNotNull(hre.Content);
                var args = hre.Content.GetArguments();
                
                Assert.AreEqual("Fail parameter validation", hre.Content.GetInvalidReason());

                Assert.AreEqual(2, args.Count);
                Assert.AreEqual("index", args.First().Key);
                Assert.AreEqual("1", args.First().Value.GetValue());
                Assert.AreEqual(null, args.Last().Value.GetInvalidReason());

                Assert.AreEqual("str", args.Last().Key);
                Assert.AreEqual("fred", args.Last().Value.GetValue());
                Assert.AreEqual(null, args.Last().Value.GetInvalidReason());

            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }

    }

    [Test]
    public void TestValidateWithFailCrossValidation() {
        
        var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
        var action = parsedResult.GetAction(nameof(ClassWithActions.ActionFailsCrossValidation));

        try {
            action.Validate(TestInvokeOptions(), 1,  "fred").Wait();
            Assert.Fail("Expect exception");
        }
        catch (AggregateException ae) {
            if (ae.InnerExceptions.FirstOrDefault() is HttpInvalidArgumentsRosiException hre) {
                Assert.AreEqual(HttpStatusCode.UnprocessableEntity, hre.StatusCode);
                Assert.AreEqual("199 RestfulObjects \"Fail parameter validation\"", hre.Message);
                Assert.IsNotNull(hre.Content);
                var args = hre.Content.GetArguments();
                
                Assert.AreEqual("Fail parameter validation", hre.Content.GetInvalidReason());

                Assert.AreEqual(2, args.Count);
                Assert.AreEqual("index", args.First().Key);
                Assert.AreEqual("1", args.First().Value.GetValue());
                Assert.AreEqual(null, args.Last().Value.GetInvalidReason());

                Assert.AreEqual("str", args.Last().Key);
                Assert.AreEqual("fred", args.Last().Value.GetValue());
                Assert.AreEqual(null, args.Last().Value.GetInvalidReason());

            }
            else {
                Assert.Fail("Unexpected exception type");
            }
        }
        catch {
            Assert.Fail("Unexpected exception type");
        }

    }
}