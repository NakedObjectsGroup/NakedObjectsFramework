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

public class ParameterApiTests : AbstractApiTests {
    [Test]
    public void TestGetParameters() {
        var objectRep = GetObject(FullName<ClassWithActions>(), "1");
        var details = objectRep.GetAction(nameof(ClassWithActions.ActionWithMixedParmsReturnsObject)).GetDetails(TestInvokeOptions()).Result;
        Assert.IsNotNull(details);

        var parameters = details.GetParameters().Parameters();

        Assert.AreEqual(2, parameters.Count());

        var indexParameter = parameters["index"];

        Assert.AreEqual(3, indexParameter.GetDefault<int>());
        Assert.AreEqual(2, indexParameter.GetChoices<int>().Count());
        Assert.AreEqual(1, indexParameter.GetChoices<int>().First());
        Assert.AreEqual(2, indexParameter.GetChoices<int>().Last());

        Assert.AreEqual(0, indexParameter.GetLinkChoices().Count());

        Assert.IsNull(indexParameter.GetLinkDefault());

        var classParameter = parameters["class1"];

        Assert.AreEqual("Class:1", classParameter.GetLinkDefault().GetTitle());
        Assert.AreEqual(2, classParameter.GetLinkChoices().Count());
        Assert.AreEqual("Class:1", classParameter.GetLinkChoices().First().GetTitle());
        Assert.AreEqual("Class:2", classParameter.GetLinkChoices().Last().GetTitle());

        Assert.AreEqual(0, classParameter.GetChoices<int>().Count());

        Assert.IsNull(classParameter.GetDefault<int?>());
    }
}