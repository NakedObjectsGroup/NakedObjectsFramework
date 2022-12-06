// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Net.Http;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Helpers;
using ROSI.Test.Helpers;

namespace ROSI.Test.ApiTests;

public class HomeApiTests : AbstractApiTests {
    [Test]
    public void TestGetHome() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/")).Result;
        var selfLink = home.GetSelfLink();
        var userLink = home.GetUserLink();
        var servicesLink = home.GetServicesLink();
        var versionLink = home.GetVersionLink();
        var menusLink = home.GetMenusLink();

        Assert.AreEqual("http://localhost/", selfLink.GetHref().ToString());
        Assert.AreEqual("http://localhost/user", userLink.GetHref().ToString());
        Assert.AreEqual("http://localhost/services", servicesLink.GetHref().ToString());
        Assert.AreEqual("http://localhost/version", versionLink.GetHref().ToString());
        Assert.AreEqual("http://localhost/menus", menusLink.GetHref().ToString());
    }

    [Test]
    public void TestGetUser() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/")).Result;
        HttpHelpers.Client = new HttpClient(new StubHttpMessageHandler(Api()));
        var user = home.GetUserAsync().Result;
        Assert.AreEqual("Test", user.GetUserName());
        Assert.IsNull(user.GetFriendlyName());
        Assert.IsNull(user.GetEmail());
        Assert.AreEqual(0, user.GetRoles().Count());
    }

    [Test]
    public void TestGetVersion() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/")).Result;
        HttpHelpers.Client = new HttpClient(new StubHttpMessageHandler(Api()));
        var version = home.GetVersionAsync().Result;
        Assert.AreEqual("1.2", version.GetSpecVersion());
        Assert.AreEqual("Naked Objects 13.1.0", version.GetImplVersion());
    }

    [Test]
    public void TestGetServices() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/")).Result;
        HttpHelpers.Client = new HttpClient(new StubHttpMessageHandler(Api()));
        var services = home.GetServicesAsync().Result;
        var value = services.GetValue();
        Assert.AreEqual(1, value.Count());

        var sl = services.GetServiceLink("ROSI.Test.Data.SimpleService");
        Assert.IsNotNull(sl);

        HttpHelpers.Client = new HttpClient(new StubHttpMessageHandler(Api()));

        var s = services.GetService("ROSI.Test.Data.SimpleService").Result;

        var actions = s.GetActions();

        Assert.AreEqual(1, actions.Count());
    }

    [Test]
    public void TestGetMenus() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/")).Result;
        HttpHelpers.Client = new HttpClient(new StubHttpMessageHandler(Api()));
        var menus = home.GetMenusAsync().Result;
        var value = menus.GetValue();
        Assert.AreEqual(1, value.Count());

        var ml = menus.GetMenuLink("SimpleService");
        Assert.IsNotNull(ml);

        HttpHelpers.Client = new HttpClient(new StubHttpMessageHandler(Api()));

        var m = menus.GetMenu("SimpleService").Result;

        var actions = m.GetActions();

        Assert.AreEqual(1, actions.Count());
    }
}