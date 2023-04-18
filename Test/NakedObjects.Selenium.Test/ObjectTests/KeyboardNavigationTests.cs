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

public abstract class KeyboardNavigationTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void EnterEquivalentToLeftClick() {
        GeminiUrl("object?o1=___1.Store--350&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Twin Cycles");
        var reference = GetReferenceProperty("Sales Person", "Lynn Tsoflias");
        reference.SendKeys(Keys.Enter);
        WaitForView(Pane.Single, PaneType.Object, "Lynn Tsoflias");
    }

    [TestMethod]
    public virtual void ShiftEnterEquivalentToRightClick() {
        Url(CustomersMenuUrl);
        WaitForView(Pane.Single, PaneType.Home, "Home");
        Wait.Until(d => d.FindElements(By.CssSelector("nof-action")).Count >= CustomerServiceActions);
        OpenActionDialog("Find Customer By Account Number");
        ClearFieldThenType(".value  input", "AW00022262");
        OKButton().SendKeys(Keys.Shift + Keys.Enter);
        WaitForView(Pane.Left, PaneType.Home, "Home");
        WaitForView(Pane.Right, PaneType.Object, "Marcus Collins, AW00022262");
    }

    [TestMethod]
    [Ignore("#501")]
    public virtual void SelectFooterIconsWithAccessKeys() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        WaitForCss(".header .title").Click();
        var element = Driver.SwitchTo().ActiveElement();

        element.SendKeys(Keys.Alt + "h");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Home (Alt-h)", element.GetAttribute("title"));

        element.SendKeys(Keys.Alt + "b");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Back (Alt-b)", element.GetAttribute("title"));

        element.SendKeys(Keys.Alt + "f");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Forward (Alt-f)", element.GetAttribute("title"));

        element.SendKeys(Keys.Alt + "e");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Expand pane (Alt-e)", element.GetAttribute("title"));

        element.SendKeys(Keys.Alt + "s");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Swap panes (Alt-s)", element.GetAttribute("title"));

        element.SendKeys(Keys.Alt + "r");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Recent object (Alt-r)", element.GetAttribute("title"));

        element.SendKeys(Keys.Alt + "c");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Cicero - Speech Interface (Alt-c)", element.GetAttribute("title"));

        element.SendKeys(Keys.Alt + "p");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Application Properties (Alt-p)", element.GetAttribute("title"));

        element.SendKeys(Keys.Alt + "l");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Log off (Alt-l)", element.GetAttribute("title"));
    }

    [TestMethod]
    [Ignore("#501")]
    public virtual void SelectObjectActionsWithAccessKey() {
        GeminiUrl("object?i1=View&o1=___1.Person--15748");
        WaitForView(Pane.Single, PaneType.Object);
        var element = Driver.SwitchTo().ActiveElement();
        element.SendKeys(Keys.Alt + "a");
        element = Driver.SwitchTo().ActiveElement();
        Assert.AreEqual("Open actions (Alt-a)", element.GetAttribute("title"));
    }
}

[TestClass]
public class KeyboardNavigationTestsChrome : KeyboardNavigationTests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }
}