﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Tests;
using OpenQA.Selenium;

namespace NakedObjects.Selenium.Test.ObjectTests;

public abstract class SplitPaneTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void ListInSplitPaneUpdatesWhenSearchParamsChange() {
        GeminiUrl("home?m1=ProductRepository&d1=FindProductByName");
        ClearFieldThenType("#searchstring1", "a");
        RightClick(OKButton());
        WaitForView(Pane.Right, PaneType.List);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".reference"))[0].Text == "Adjustable Race");
        ClearFieldThenType("#searchstring1", "z");
        RightClick(OKButton());
        Wait.Until(dr => dr.FindElements(By.CssSelector(".reference"))[0].Text == "Hydration Pack - 70 oz.");
    }

    [TestMethod]
    public virtual void TwoListsCanBothBeReloaded() {
        GeminiUrl("home?m1=ProductRepository&d1=FindProductByName");
        ClearFieldThenType("#searchstring1", "x");
        RightClick(OKButton());
        WaitForView(Pane.Right, PaneType.List);
        Wait.Until(dr => dr.FindElements(By.CssSelector("#pane2 .reference"))[0].Text == "External Lock Washer 1");
        ClearFieldThenType("#searchstring1", "n");
        Click(OKButton());
        Wait.Until(dr => dr.FindElements(By.CssSelector("#pane1 .reference"))[0].Text == "All-Purpose Bike Stand");
        Reload(Pane.Left);
        Reload(Pane.Right);

        //Try a reload from a new Url with params
        GeminiUrl("list/list?m2=ProductRepository&a2=FindProductByName&pg2=1&ps2=20&s2_=0&c2=List&pm2_searchString=%22f%22&m1=ProductRepository&a1=FindProductByName&pg1=1&ps1=20&s1_=0&c1=List&pm1_searchString=%22y%22");
        WaitForView(Pane.Left, PaneType.List);
        WaitForView(Pane.Right, PaneType.List);
        Reload(Pane.Left);
        Wait.Until(dr => dr.FindElements(By.CssSelector("#pane1 .reference"))[0].Text == "Chain Stays");
        Reload(Pane.Right);
        Wait.Until(dr => dr.FindElements(By.CssSelector("#pane2 .reference"))[0].Text == "Fender Set - Mountain");
    }

    protected override void ScrollTo(IWebElement element) {
        var script = $"window.scrollTo({element.Location.X}, {element.Location.Y});return true;";
        ((IJavaScriptExecutor)Driver).ExecuteScript(script);
    }

    #region Actions that go from single to split panes

    [TestMethod]
    public virtual void RightClickActionReturningObjectFromHomeSingle() {
        Url(CustomersMenuUrl);
        WaitForView(Pane.Single, PaneType.Home, "Home");
        Wait.Until(d => d.FindElements(By.CssSelector("nof-action-list nof-action")).Count == CustomerServiceActions);
        OpenActionDialog("Find Customer By Account Number");
        ClearFieldThenType(".value  input", "AW00022262");
        RightClick(OKButton());
        WaitForView(Pane.Left, PaneType.Home, "Home");
        WaitForView(Pane.Right, PaneType.Object, "Marcus Collins, AW00022262");
        //Check that dialog is still open on the left:
        WaitForCss("#pane1 .dialog");
    }

    [TestMethod]
    public virtual void RightClickActionReturningListFromHomeSingle() {
        Url(OrdersMenuUrl);
        WaitForView(Pane.Single, PaneType.Home, "Home");
        RightClick(GetObjectEnabledAction("Highest Value Orders"));
        WaitForView(Pane.Left, PaneType.Home, "Home");
        WaitForView(Pane.Right, PaneType.List, "Highest Value Orders");
    }

    [TestMethod]
    public virtual void RightClickReferenceFromListSingle() {
        Url(OrdersMenuUrl);
        Click(GetObjectEnabledAction("Highest Value Orders"));
        WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
        var row = WaitForCss("table .reference");
        Assert.AreEqual("SO51131", row.Text);
        RightClick(row);
        WaitForView(Pane.Left, PaneType.List, "Highest Value Orders");
        WaitForView(Pane.Right, PaneType.Object, "SO51131");
    }

    [TestMethod]
    public virtual void RightClickReferencePropertyFromObjectSingle() {
        GeminiUrl("object?o1=___1.Store--350&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Twin Cycles");
        var reference = GetReferenceProperty("Sales Person", "Lynn Tsoflias");
        RightClick(reference);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
        WaitForView(Pane.Right, PaneType.Object, "Lynn Tsoflias");
    }

    [TestMethod]
    public virtual void RightClickActionFromObjectSingle() {
        GeminiUrl("object?o1=___1.Customer--30116&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Technical Parts Manufacturing, AW00030116");
        OpenSubMenu("Orders");
        RightClick(GetObjectEnabledAction("Last Order"));
        WaitForView(Pane.Left, PaneType.Object, "Technical Parts Manufacturing, AW00030116");
        WaitForView(Pane.Right, PaneType.Object, "SO67279");
    }

    [TestMethod]
    public virtual void RightClickHomeIconFromObjectSingle() {
        GeminiUrl("object?o1=___1.Store--350&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Twin Cycles");
        RightClick(HomeIcon());
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
        WaitForView(Pane.Right, PaneType.Home, "Home");
    }

    [TestMethod]
    public virtual void SwapPanesIconFromSingleOpensHomeOnLeft() {
        GeminiUrl("object?o1=___1.Store--350&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Twin Cycles");
        Click(SwapIcon());
        WaitForView(Pane.Left, PaneType.Home, "Home");
        WaitForView(Pane.Right, PaneType.Object, "Twin Cycles");
    }

    #endregion

    #region Actions within split panes

    private string TwoObjects => $"{GeminiBaseUrl}object/object?o1=___1.Customer--555&as1=open&o2=___1.SalesOrderHeader--71926&as2=open";
    private string TwoObjectsB => GeminiBaseUrl + "object/object?o1=___1.Store--350&as1=open&o2=___1.SalesOrderHeader--71926&as2=open";

    [TestMethod]
    public virtual void RightClickReferenceInLeftPaneObject() {
        Url(TwoObjects);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        WaitForView(Pane.Right, PaneType.Object, "SO71926");
        var reference = GetReferenceProperty("Sales Territory", "Australia", Pane.Left);
        RightClick(reference);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        WaitForView(Pane.Right, PaneType.Object, "Australia");
    }

    [TestMethod]
    public virtual void ClickReferenceInLeftPaneObject() {
        Url(TwoObjects);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        WaitForView(Pane.Right, PaneType.Object, "SO71926");

        var reference = GetReferenceProperty("Sales Territory", "Australia", Pane.Left);
        Click(reference);

        WaitForView(Pane.Left, PaneType.Object, "Australia");
        WaitForView(Pane.Right, PaneType.Object, "SO71926");
    }

    [TestMethod]
    public virtual void ClickReferenceInRightPaneObject() {
        Url(TwoObjectsB);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
        WaitForView(Pane.Right, PaneType.Object, "SO71926");

        var reference = GetReferenceProperty("Billing Address", "2253-217 Palmer Street ...", Pane.Right);
        Click(reference);

        WaitForView(Pane.Right, PaneType.Object, "2253-217 Palmer Street ...");
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
    }

    [TestMethod]
    public virtual void RightClickReferenceInRightPaneObject() {
        Url(TwoObjects);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        WaitForView(Pane.Right, PaneType.Object, "SO71926");
        var reference = GetReferenceProperty("Ship Method", "CARGO TRANSPORT 5", Pane.Right);
        RightClick(reference);
        WaitForView(Pane.Right, PaneType.Object, "SO71926");
        WaitForView(Pane.Left, PaneType.Object, "CARGO TRANSPORT 5");
    }

    [TestMethod]
    //Behaviour modified with #61 - assuming config option set to true
    public virtual void LeftClickHomeIconFromSplitObjectObject() {
        Url(TwoObjects);
        Click(HomeIcon());
        WaitForView(Pane.Single, PaneType.Home);
    }

    [TestMethod]
    public virtual void RightClickHomeIconFromSplitObjectObject() {
        Url(TwoObjects);
        RightClick(HomeIcon());
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        WaitForView(Pane.Right, PaneType.Home, "Home");
    }

    [TestMethod]
    public virtual void ActionDialogOpensInCorrectPane() {
        GeminiUrl("object/object?i1=View&o1=___1.Customer--543&i2=View&o2=___1.SalesOrderHeader--56672&as1=open&as2=open");
        WaitForView(Pane.Left, PaneType.Object, "Friendly Neighborhood Bikes, AW00000543");
        WaitForView(Pane.Right, PaneType.Object, "SO56672");
        OpenSubMenu("Orders", Pane.Left);
        OpenActionDialog("Create New Order", Pane.Left);
        Thread.Sleep(500);
        CancelDialog(Pane.Left);
        OpenActionDialog("Add New Sales Reasons", Pane.Right);
        Thread.Sleep(500);
        CancelDialog(Pane.Right);
    }

    [TestMethod]
    public virtual void RightClickIsSameAsLeftClickForOpeningDialog() {
        Url(TwoObjects);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        WaitForView(Pane.Right, PaneType.Object, "SO71926");
        OpenSubMenu("Orders", Pane.Left);
        RightClick(GetObjectEnabledAction("Create New Order", Pane.Left));
        var selector = CssSelectorFor(Pane.Left) + " .dialog ";
        Wait.Until(d => d.FindElement(By.CssSelector(selector)));
    }

    [TestMethod]
    public virtual void SwapPanes() {
        Url(TwoObjects);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        WaitForView(Pane.Right, PaneType.Object, "SO71926");
        Click(SwapIcon());
        WaitForView(Pane.Left, PaneType.Object, "SO71926");
        WaitForView(Pane.Right, PaneType.Object, "Twin Cycles, AW00000555");
    }

    [TestMethod]
    public virtual void FullPaneFromLeft() {
        Url(TwoObjects);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        WaitForView(Pane.Right, PaneType.Object, "SO71926");
        Click(FullIcon());
        WaitForView(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
    }

    [TestMethod]
    public virtual void FullPaneFromRight() {
        Url(TwoObjects);
        WaitForView(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        WaitForView(Pane.Right, PaneType.Object, "SO71926");
        RightClick(FullIcon());
        WaitForView(Pane.Single, PaneType.Object, "SO71926");
    }

    #endregion
}

#region Mega tests

[TestClass]
public class SplitPaneTestsChrome : SplitPaneTests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }
}

#endregion