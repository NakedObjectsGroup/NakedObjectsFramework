// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Selenium.Helpers.Tests;

namespace NakedObjects.Selenium.Test.ObjectTests;

public abstract class CustomTemplateTestsRoot : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    public virtual void CustomViewTemplate() {
        GeminiUrl("object?i1=View&o1=___1.Location--60");
        WaitForView(Pane.Single, PaneType.Object, "Location - custom view");
        Assert.AreEqual("Topaz", WaitForCss(".presentationHint").Text);
    }

    public virtual void CustomEditTemplate() {
        GeminiUrl("object?i1=Edit&o1=___1.WorkOrderRouting--43375--980--7");
        WaitForView(Pane.Single, PaneType.Object, "Work Order Routing - custom edit");
    }

    public virtual void CustomListTemplate() {
        GeminiUrl("list?m1=WorkOrderRepository&a1=AllLocations&pg1=1&ps1=20&s1_=0&c1=List");
        Reload();
        WaitForView(Pane.Single, PaneType.List, "Location - custom list");
    }

    public virtual void CustomErrorHandling() {
        Url(CustomersMenuUrl);
        WaitForCss(".actions nof-action", CustomerServiceActions);
        Click(GetObjectEnabledAction("Throw Domain Exception"));
        WaitForView(Pane.Single, PaneType.Error);
        Assert.AreEqual("Internal Server Error", WaitForCss(".title").Text);
        Assert.AreEqual("Foo", WaitForCss(".message").Text);
    }
}

public abstract class CustomTemplateTests : CustomTemplateTestsRoot {
    [TestMethod]
    public override void CustomViewTemplate() {
        base.CustomViewTemplate();
    }

    [TestMethod]
    public override void CustomEditTemplate() {
        base.CustomEditTemplate();
    }

    [TestMethod]
    public override void CustomListTemplate() {
        base.CustomListTemplate();
    }

    [TestMethod]
    public override void CustomErrorHandling() {
        base.CustomErrorHandling();
    }
}

#region browsers specific subclasses

public class CustomTemplateTestsChrome : CustomTemplateTests {
    [ClassInitialize]
    public static void InitialiseClass(TestContext context) {
        FilePath(@"drivers.chromedriver.exe");
    }

    [TestInitialize]
    public virtual void InitializeTest() {
        InitChromeDriver();
    }

    [TestCleanup]
    public virtual void CleanupTest() {
        CleanupChromeDriver();
    }
}

#endregion

#region Mega tests

public abstract class MegaCustomTemplateTestsRoot : CustomTemplateTestsRoot {
    [TestMethod] //Mega
    [Priority(0)]
    public void MegaCustomTemplateTests() {
        CustomViewTemplate();
        CustomEditTemplate();
        CustomListTemplate();
        CustomErrorHandling();
    }

    [TestMethod]
    [Priority(-1)]
    public void ProblematicTests() { }
}

#endregion