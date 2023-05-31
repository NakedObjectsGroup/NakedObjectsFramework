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

public abstract class FooterTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void Home() {
        GeminiUrl("object?o1=___1.Product--968");
        WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
        Click(Driver.FindElement(By.CssSelector(".icon.home")));
        WaitForView(Pane.Single, PaneType.Home, "Home");
    }

    [TestMethod]
    public virtual void BackAndForward() {
        Url(BaseUrl);
        GoToMenuFromHomePage("Orders");
        Click(GetObjectEnabledAction("Random Order"));
        WaitForView(Pane.Single, PaneType.Object);
        var orderTitle = WaitForCss(".title").Text;
        ClickBackButton();
        WaitForView(Pane.Single, PaneType.Home);
        ClickForwardButton();
        WaitForView(Pane.Single, PaneType.Object, orderTitle);
        EditObject();
        WaitForView(Pane.Single, PaneType.Object, "Editing - " + orderTitle);
        ClickBackButton();
        WaitForView(Pane.Single, PaneType.Home);
        ClickForwardButton();
        WaitForView(Pane.Single, PaneType.Object, "Editing - " + orderTitle);
        Click(GetCancelEditButton());
        WaitForView(Pane.Single, PaneType.Object, orderTitle);
        ClickBackButton();
        WaitForView(Pane.Single, PaneType.Home);
        ClickForwardButton();
        WaitForView(Pane.Single, PaneType.Object, orderTitle);

        var link = GetReferenceFromProperty("Customer");
        var cusTitle = link.Text;
        Click(link);
        WaitForView(Pane.Single, PaneType.Object, cusTitle);
        ClickBackButton();
        WaitForView(Pane.Single, PaneType.Object, orderTitle);
        ClickForwardButton();
        WaitForView(Pane.Single, PaneType.Object, cusTitle);
        OpenObjectActions();
        OpenSubMenu("Orders");
        OpenActionDialog("Create New Order");
        ClickBackButton();
        WaitForView(Pane.Single, PaneType.Object, cusTitle);
        ClickBackButton();
        WaitForView(Pane.Single, PaneType.Object, orderTitle);
    }

    [TestMethod]
    public virtual void RecentObjects() {
        GeminiUrl("home?m1=CustomerRepository&d1=FindCustomerByAccountNumber&f1_accountNumber=%22AW%22");
        ClearFieldThenType("#accountnumber1", "AW00000042");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.Object, "Healthy Activity Store, AW00000042");
        ClickHomeButton();
        GoToMenuFromHomePage("Customers");
        OpenActionDialog("Find Customer By Account Number");
        ClearFieldThenType("#accountnumber1", "AW00000359");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.Object, "Mechanical Sports Center, AW00000359");
        ClickHomeButton();
        GoToMenuFromHomePage("Customers");
        OpenActionDialog("Find Customer By Account Number");
        ClearFieldThenType("#accountnumber1", "AW00022262");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.Object, "Marcus Collins, AW00022262");
        ClickHomeButton();
        GoToMenuFromHomePage("Products");
        Click(GetObjectEnabledAction("Find Product By Number"));
        ClearFieldThenType("#number1", "LJ-0192-S");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.Object, "Long-Sleeve Logo Jersey, S");
        ClickRecentButton();
        WaitForView(Pane.Single, PaneType.Recent);
        var el = WaitForCssNo("tr td:nth-child(2)", 0);
        Assert.AreEqual("Long-Sleeve Logo Jersey, S", el.Text);
        el = WaitForCssNo("tr td:nth-child(2)", 1);
        Assert.AreEqual("Marcus Collins, AW00022262", el.Text);
        el = WaitForCssNo("tr td:nth-child(2)", 2);
        Assert.AreEqual("Mechanical Sports Center, AW00000359", el.Text);
        el = WaitForCssNo("tr td:nth-child(2)", 3);
        Assert.AreEqual("Healthy Activity Store, AW00000042", el.Text);

        //Test left- and right-click navigation from Recent
        el = WaitForCssNo("tr td:nth-child(2)", 0);
        Assert.AreEqual("Long-Sleeve Logo Jersey, S", el.Text);
        RightClick(el);
        WaitForView(Pane.Right, PaneType.Object, "Long-Sleeve Logo Jersey, S");
        WaitForView(Pane.Left, PaneType.Recent);
        el = WaitForCssNo("tr td:nth-child(2)", 1);
        Assert.AreEqual("Marcus Collins, AW00022262", el.Text);
        Click(el);
        WaitForView(Pane.Left, PaneType.Object, "Marcus Collins, AW00022262");

        //Test that clear button works
        ClickRecentButton();
        WaitForView(Pane.Left, PaneType.Recent);
        WaitForCss("tr td:nth-child(1)", 6);
        var clear = GetInputButton("Clear All", Pane.Left).AssertIsEnabled();
        Click(clear);
        GetInputButton("Clear All", Pane.Left).AssertIsDisabled();
        WaitForCss("tr td", 0);
    }

    [TestMethod]
    public virtual void ApplicationProperties() {
        var lastPropertyText = "Client version:";
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        ClickPropertiesButton();
        WaitForView(Pane.Single, PaneType.ApplicationProperties, "Application Properties");
        Wait.Until(d => Driver.FindElements(By.CssSelector(".property")).Count >= 7);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[6].Text.StartsWith(lastPropertyText));
        var properties = Driver.FindElements(By.CssSelector(".property"));
        Assert.IsTrue(properties[0].Text.StartsWith("Application Name:"), properties[0].Text);
        Assert.IsTrue(properties[1].Text.StartsWith("User Name:"), properties[1].Text);
        Assert.IsTrue(properties[2].Text.StartsWith("Server Url: http"), properties[2].Text); // maybe https
        Assert.IsTrue(properties[3].Text.StartsWith("Server API version:"), properties[3].Text);
        Assert.IsTrue(properties[4].Text.StartsWith("Server Framework version:"), properties[4].Text);
        Assert.IsTrue(properties[5].Text.StartsWith("Server Application version:"), properties[5].Text);
        Assert.IsTrue(properties[6].Text.StartsWith(lastPropertyText), properties[6].Text);
    }

    [TestMethod]
    public virtual void LogOff() {
        GeminiUrl("home");
        ClickLogOffButton();
        Wait.Until(d => Driver.FindElement(By.CssSelector(".title")).Text.StartsWith("Log Off"));
        var cancel = Wait.Until(dr => dr.FindElement(By.CssSelector("button[value='Cancel']")));
        Click(cancel);
        WaitForView(Pane.Single, PaneType.Home);
    }

    #region WarningsAndInfo

    [TestMethod]
    public virtual void ExplicitWarningsAndInfo() {
        GeminiUrl("home?m1=WorkOrderRepository");
        Click(GetObjectEnabledAction("Generate Info And Warning"));
        var warn = WaitForCss(".footer .warnings");
        Assert.AreEqual("Warn User of something else", warn.Text);
        var msg = WaitForCss(".footer .messages");
        Assert.AreEqual("Inform User of something", msg.Text);

        //Test that both are cleared by next action
        Click(GetObjectEnabledAction("Random Work Order"));
        WaitUntilElementDoesNotExist(".footer .warnings");
        WaitUntilElementDoesNotExist(".footer .messages");
    }

    [TestMethod]
    public virtual void ZeroParamActionReturningNullGeneratesGenericWarning() {
        GeminiUrl("home?m1=EmployeeRepository");
        Click(GetObjectEnabledAction("Me"));
        WaitForTextEquals(".footer .warnings", "no result found");
        Click(GetObjectEnabledAction("My Departmental Colleagues"));
        WaitForTextEquals(".footer .warnings", "Current user unknown");
    }

    #endregion
}

[TestClass]
public class FooterTestsChrome : FooterTests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }
}