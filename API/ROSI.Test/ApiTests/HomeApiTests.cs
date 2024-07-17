// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Test.Data;

namespace ROSI.Test.ApiTests;

public class HomeApiTests : AbstractRosiApiTests {
    [Test]
    public void TestGetHome() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
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

        Assert.AreEqual(0, home.GetExtensions().Extensions().Count);
    }

    [Test]
    public void TestGetUser() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        var user = home.GetUser(TestInvokeOptions()).Result;

        Assert.AreEqual(2, user.GetLinks().Count());
        Assert.AreEqual("Test", user.GetUserName());
        Assert.IsNull(user.GetFriendlyName());
        Assert.IsNull(user.GetEmail());
        Assert.AreEqual(0, user.GetRoles().Count());

        Assert.AreEqual(0, user.GetExtensions().Extensions().Count);
    }

    [Test]
    public void TestGetVersion() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        //HttpHelpers.Client = new HttpClient(new StubHttpMessageHandler(Api()));
        var version = home.GetVersion(TestInvokeOptions()).Result;
        Assert.AreEqual(2, version.GetLinks().Count());
        Assert.AreEqual("1.2", version.GetSpecVersion());
        Assert.AreEqual("Naked Objects 16.0.0", version.GetImplVersion());

        Assert.AreEqual(0, version.GetExtensions().Extensions().Count);
    }

    [Test]
    public void TestGetServices() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        var services = home.GetServices(TestInvokeOptions()).Result;

        Assert.AreEqual(0, services.GetExtensions().Extensions().Count);

        Assert.AreEqual(2, services.GetLinks().Count());

        var value = services.GetValue();
        Assert.AreEqual(1, value.Count());

        var sl = services.GetServiceLink(FullName<SimpleService>());
        Assert.IsNotNull(sl);

        var s = services.GetService(FullName<SimpleService>(), TestInvokeOptions()).Result;

        Assert.AreEqual(FullName<SimpleService>(), s.GetServiceId());
        Assert.AreEqual("Simple Service", s.GetTitle());

        var actions = s.GetActions();

        Assert.AreEqual(3, actions.Count());
    }

    [Test]
    public void TestGetMenus() {
        var home = ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;
        var menus = home.GetMenus(TestInvokeOptions()).Result;

        Assert.AreEqual(0, menus.GetExtensions().Extensions().Count);

        Assert.AreEqual(2, menus.GetLinks().Count());

        var value = menus.GetValue();
        Assert.AreEqual(1, value.Count());

        var ml = menus.GetMenuLink(nameof(SimpleService));
        Assert.IsNotNull(ml);

        var m = menus.GetMenu(nameof(SimpleService), TestInvokeOptions()).Result;

        Assert.AreEqual(nameof(SimpleService), m.GetMenuId());
        Assert.AreEqual("Simple Service", m.GetTitle());

        var actions = m.GetActions();

        Assert.AreEqual(3, actions.Count());
    }
}