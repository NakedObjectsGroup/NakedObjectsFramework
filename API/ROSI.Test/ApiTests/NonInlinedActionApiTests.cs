// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Net.Http;
using NakedFramework.Rest.Extensions;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Test.Data;

namespace ROSI.Test.ApiTests;

public class NonInlinedActionApiTests : AbstractApiTests {


    protected override Action<RestfulObjectsOptions> RestfulObjectsOptions => options => {
        options.CacheSettings = (0, 3600, 86400);
        options.InlineDetailsInActionMemberRepresentations = false;
    };
    
    
    //[Test]
    //public void TestInvokeWithValueParmsReturnsObjectAction() {
    //    var parsedResult = GetObject(FullName<ClassWithActions>(), "1");
    //    var action = parsedResult.GetAction(nameof(ClassWithActions.ActionWithValueParmsReturnsObject));
    //    Assert.IsFalse(action.GetLinks().HasInvokeLink());

    //    var ar = action.Invoke(1, "test").Result;

    //    Assert.AreEqual(ActionResultApi.ResultType.@object, ar.GetResultType());

    //    var links = ar.GetLinks();
    //    Assert.AreEqual(1, links.Count());

    //    var o = ar.GetObject();
    //    Assert.AreEqual("http://localhost/objects/ROSI.Test.Data.Class/1", o.GetLinks().GetSelfLink().GetHref().ToString());
    //}

  
}