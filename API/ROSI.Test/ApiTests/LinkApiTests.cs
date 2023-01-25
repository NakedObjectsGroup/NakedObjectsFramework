// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NUnit.Framework;
using ROSI.Apis;
using ROSI.Test.Data;

namespace ROSI.Test.ApiTests;

public class LinkApiTests : AbstractApiTests {
    [Test]
    public void TestGetInvokeLink() {
        var domainObject = GetObject(FullName<Class>(), "1");
        var action = domainObject.GetAction("Action1");

        var links = action.GetLinks();

        var invokeLink = links.GetInvokeLink();

        Assert.IsNotNull(invokeLink);
    }
}