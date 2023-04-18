// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Tests;

namespace NakedObjects.Selenium.Test.ObjectTests;

public abstract class CustomTemplateTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void CustomViewTemplate() {
        GeminiUrl("object?i1=View&o1=___1.Location--60");
        WaitForView(Pane.Single, PaneType.Object, "Location - custom view");
        Assert.AreEqual("Topaz", WaitForCss(".presentationHint").Text);
    }

    [TestMethod]
    public virtual void CustomEditTemplate() {
        GeminiUrl("object?i1=Edit&o1=___1.WorkOrderRouting--43375--980--7");
        WaitForView(Pane.Single, PaneType.Object, "Work Order Routing - custom edit");
    }

    [TestMethod]
    public virtual void CustomListTemplate() {
        GeminiUrl("list?m1=WorkOrderRepository&a1=AllLocations&pg1=1&ps1=20&s1_=0&c1=List");
        Reload();
        WaitForView(Pane.Single, PaneType.List, "Location - custom list");
    }

    [TestMethod]
    public virtual void CustomErrorHandling() {
        Url(CustomersMenuUrl);
        WaitForCss(".actions nof-action", CustomerServiceActions);
        Click(GetObjectEnabledAction("Throw Domain Exception"));
        WaitForView(Pane.Single, PaneType.Error);
        Assert.AreEqual("Internal Server Error", WaitForCss(".title").Text);
        Assert.AreEqual("Foo", WaitForCss(".message").Text);
    }
}

[TestClass]
[Ignore("No currently configured")]
public class CustomTemplateTestsChrome : CustomTemplateTests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }
}