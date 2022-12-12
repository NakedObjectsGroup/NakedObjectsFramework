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

public class ExtensionsApiTests : AbstractApiTests {
    [Test]
    public void TestGetExtensions() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var extsRep = objectRep.GetExtensions();
        var dict = extsRep.Extensions();

        Assert.AreEqual("Class", dict["friendlyName"]);
        Assert.AreEqual("", dict["description"]);
        Assert.AreEqual("Classes", dict["pluralName"]);
        Assert.AreEqual("ROSI.Test.Data.Class", dict["domainType"]);
        Assert.AreEqual(false, dict["isService"]);
        Assert.AreEqual("persistent", dict["x-ro-nof-interactionMode"]);
    }

    [Test]
    public void TestGetExtension() {
        var objectRep = GetObject(FullName<Class>(), "1");
        var extsRep = objectRep.GetExtensions();

        Assert.AreEqual("Class", extsRep.GetExtension<string>("friendlyName"));
        Assert.AreEqual("", extsRep.GetExtension<string>("description"));
        Assert.AreEqual("Classes", extsRep.GetExtension<string>("pluralName"));
        Assert.AreEqual("ROSI.Test.Data.Class", extsRep.GetExtension<string>("domainType"));
        Assert.AreEqual(false, extsRep.GetExtension<bool>("isService"));
        Assert.AreEqual("persistent", extsRep.GetExtension<string>("x-ro-nof-interactionMode"));
    }
}