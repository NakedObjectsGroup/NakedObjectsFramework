// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Tests;
using OpenQA.Selenium;

namespace NakedObjects.Selenium.Test.ObjectTests;

public abstract class ViewModelTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void CreateVM() {
        GeminiUrl("object?i1=View&o1=___1.CustomerDashboard--20071&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Sean Campbell - Dashboard");
        //TODO: test for no Edit button?
    }

    [TestMethod]
    public virtual void CreateEditableVM() {
        GeminiUrl("object?i1=View&o1=___1.Person--9169&as1=open");
        Click(GetObjectEnabledAction("Create Email"));
        WaitForView(Pane.Single, PaneType.Object, "New email");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[4].Text == "Status:\r\nNew");

        ClearFieldThenType("#to1", "Stef");
        ClearFieldThenType("#from1", "Richard");
        ClearFieldThenType("#subject1", "Test");
        ClearFieldThenType("#message1", "Hello");

        Click(GetInputButton("Send"));
        Wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(5)")).Text == "Status:\r\nSent");
        Assert.AreEqual("To:", WaitForCss(".property:nth-child(1)").Text);
        var title = WaitForCss(".title");
        Assert.AreEqual("Sent email", title.Text);
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
    }

    [TestMethod]
    //Test for #46
    public virtual void EditableVMWithEmptyLeadingKeys() {
        GeminiUrl("object?i1=View&o1=___1.Person--9169&as1=open");
        Click(GetObjectEnabledAction("Create Email"));
        WaitForView(Pane.Single, PaneType.Object, "New email");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[4].Text == "Status:\r\nNew");

        //leave 3/4 of the optional fields empty
        ClearFieldThenType("#subject1", "Test2");

        Click(GetInputButton("Send"));
        Wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(5)")).Text == "Status:\r\nSent");
        Assert.AreEqual("To:", WaitForCss(".property:nth-child(1)").Text);
        var title = WaitForCss(".title");
        Assert.AreEqual("Sent email", title.Text);
    }

    [TestMethod]
    public virtual void CreateSwitchableVM() {
        GeminiUrl("object?i1=View&o1=___1.StoreSalesInfo--AW00000293--False&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Sales Info for: Fashionable Bikes and Accessories");
        Click(GetObjectEnabledAction("Edit")); //Note: not same as the generic (object) Edit button
        WaitForView(Pane.Single, PaneType.Object, "Editing - Sales Info for: Fashionable Bikes and Accessories");
        SelectDropDownOnField("#salesterritory1", "Central");
        Click(SaveVMButton()); //TODO: check if this works
        WaitForView(Pane.Single, PaneType.Object, "Sales Info for: Fashionable Bikes and Accessories");
        WaitForTextEquals(".property", 2, "Sales Territory:\r\nCentral");
    }
}

[TestClass]
public class ViewModelTestsChrome : ViewModelTests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }
}