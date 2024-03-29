﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Tests;
using OpenQA.Selenium;

namespace NakedObjects.Selenium.Test.ObjectTests;

/// <summary>
///     Tests applied from a List view.
/// </summary>
public abstract class ListTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    private bool PageTitleOK() {
        var text = WaitForCss(".list .summary .details").Text;

        return text.StartsWith("Page 1 of 15");
    }

    [TestMethod]
    public virtual void ActionReturnsListView() {
        Url(OrdersMenuUrl);
        Click(GetObjectEnabledAction("Highest Value Orders"));
        WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
        //Test content of collection
        Assert.IsTrue(PageTitleOK());
        WaitForCss(".icon.table");
        WaitUntilElementDoesNotExist(".icon.list");
        WaitUntilElementDoesNotExist(".icon.summary");
        Assert.AreEqual(20, Driver.FindElements(By.CssSelector(".list table tbody tr td.reference")).Count);
        Assert.AreEqual(20, Driver.FindElements(By.CssSelector(".list table tbody tr td.checkbox")).Count);
        Assert.AreEqual(0, Driver.FindElements(By.CssSelector(".cell")).Count); //Cells are in Table view only
    }

    [TestMethod]
    public virtual void ActionReturnsEmptyList() {
        GeminiUrl("home?m1=ProductRepository&d1=FindProductByName");
        ClearFieldThenType("#searchstring1", "zzz");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.List, "Find Product By Name");
        WaitForTextEquals(".details", "No items found");
    }

    [TestMethod]
    public virtual void TableViewAttributeHonoured() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1_=0&c1=Table");
        Reload();
        var cols = WaitForCss("th", 5).ToArray();
        Assert.AreEqual("", cols[0].Text);
        Assert.AreEqual("Description", cols[1].Text);
        Assert.AreEqual("XNoMatchingColumn", cols[2].Text);
        Assert.AreEqual("Category", cols[3].Text);
        Assert.AreEqual("Discount Pct", cols[4].Text);
        cols = WaitForCss("tbody tr:first-child td", 5).ToArray();
        Assert.AreEqual("No Discount", cols[1].Text);
        Assert.AreEqual("", cols[2].Text); //As no such column
        Assert.AreEqual("", cols[3].Text); //Happens to be empty
        Assert.AreEqual("0", cols[4].Text);
    }

    [TestMethod]
    public virtual void TableViewWorksWithSubTypes() {
        GeminiUrl("list?m1=CustomerRepository&a1=RandomCustomers&pg1=1&ps1=20&s1_=0&c1=Table");
        WaitForView(Pane.Single, PaneType.List, "Random Customers");
        Reload();
        var cols = WaitForCss("th", 4).ToArray();
        Assert.AreEqual("Account Number", cols[0].Text);
        Assert.AreEqual("Store", cols[1].Text);
        Assert.AreEqual("Person", cols[2].Text);
        Assert.AreEqual("Sales Territory", cols[3].Text);
        cols = WaitForCss("tbody tr:first-child td", 4).ToArray();
        Assert.AreEqual("", cols[1].Text); //As no such column
    }

    [TestMethod]
    public virtual void TableViewCanIncludeCollectionSummaries() {
        GeminiUrl("list?m1=OrderRepository&a1=OrdersWithMostLines&pg1=1&ps1=20&s1_=0&c1=Table");
        Reload();
        var header = WaitForCss("thead");
        var cols = header.FindElements(By.CssSelector("th")).ToArray();
        Assert.AreEqual(4, cols.Length);
        Assert.AreEqual("", cols[0].Text);
        Assert.AreEqual("", cols[1].Text);
        Assert.AreEqual("Order Date", cols[2].Text);
        Assert.AreEqual("Details", cols[3].Text);
        WaitForTextEquals("tbody tr:nth-child(1) td:nth-child(4)", "72 Items");
    }

    [TestMethod]
    public virtual void SwitchToTableViewAndBackToList() {
        Url(SpecialOffersMenuUrl);
        Click(GetObjectEnabledAction("Current Special Offers"));
        WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
        var iconTable = WaitForCss(".icon.table");
        Click(iconTable);

        var iconList = WaitForCss(".icon.list");
        WaitUntilElementDoesNotExist(".icon.table");
        WaitUntilElementDoesNotExist(".icon.summary");

        Wait.Until(dr => dr.FindElements(By.CssSelector(".list table tbody tr")).Count > 1);

        //Switch back to List view
        Click(iconList);
        WaitForCss(".icon.table");
        WaitUntilElementDoesNotExist(".icon.list");
        WaitUntilElementDoesNotExist(".icon.summary");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
        Assert.AreEqual(0, Driver.FindElements(By.CssSelector(".cell")).Count); //Cells are in Table view only
    }

    [TestMethod]

    // test after bug #193
    public virtual void TableViewFromDialogWithOptionalInt() {
        Url(WorkOrdersMenuUrl);
        OpenActionDialog("Locations By Availability");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.List, "Locations By Availability");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
        var iconTable = WaitForCss(".icon.table");
        Click(iconTable);

        var iconList = WaitForCss(".icon.list");
        WaitUntilElementDoesNotExist(".icon.table");
        WaitUntilElementDoesNotExist(".icon.summary");

        Wait.Until(dr => dr.FindElements(By.CssSelector(".list table tbody tr")).Count > 1);
    }

    [TestMethod]
    public virtual void NavigateToItemFromListView() {
        Url(SpecialOffersMenuUrl);
        Click(GetObjectEnabledAction("Current Special Offers"));
        WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
        var row = WaitForCss(".reference");
        Click(row);
        WaitForView(Pane.Single, PaneType.Object, "No Discount");
    }

    [TestMethod]
    public virtual void NavigateToItemFromTableView() {
        Url(SpecialOffersMenuUrl);
        Click(GetObjectEnabledAction("Current Special Offers"));
        WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
        var iconTable = WaitForCss(".icon.table");
        Click(iconTable);
        Thread.Sleep(500);
        Wait.Until(dr => dr.FindElement(By.CssSelector("tbody tr:nth-child(1) td:nth-child(2)")).Text == "No Discount");
        var row = Driver.FindElement(By.CssSelector("tbody tr:nth-child(1) div"));
        Click(row);
        WaitForView(Pane.Single, PaneType.Object, "No Discount");
    }

    [TestMethod]
    public virtual void Paging() {
        GeminiUrl("list?m1=CustomerRepository&a1=FindIndividualCustomerByName&p1=1&ps1=20&pm1_firstName=%22%22&pm1_lastName=%22a%22");
        Thread.Sleep(1000);
        Reload();

        //Test content of collection
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 1 of 45"));
        GetInputButton("First").AssertIsDisabled();
        GetInputButton("Previous").AssertIsDisabled();
        var next = GetInputButton("Next").AssertIsEnabled();
        GetInputButton("Last").AssertIsEnabled();
        //Go to next page
        Click(next);
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 2 of 45"));
        GetInputButton("First").AssertIsEnabled();
        GetInputButton("Previous").AssertIsEnabled();
        GetInputButton("Next").AssertIsEnabled();
        var last = GetInputButton("Last").AssertIsEnabled();
        Click(last);
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 45 of 45"));
        GetInputButton("First").AssertIsEnabled();
        var prev = GetInputButton("Previous").AssertIsEnabled();
        GetInputButton("Next").AssertIsDisabled();
        GetInputButton("Last").AssertIsDisabled();
        Click(prev);
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 44 of 45"));
        var first = GetInputButton("First").AssertIsEnabled();
        GetInputButton("Previous").AssertIsEnabled();
        GetInputButton("Next").AssertIsEnabled();
        GetInputButton("Last").AssertIsEnabled();
        Click(first);
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 1 of 45"));
    }

    [TestMethod]
    public virtual void PageSizeRecognised() {
        //Method marked with PageSize(2)
        GeminiUrl("home?m1=CustomerRepository&d1=FindStoreByName");
        ClearFieldThenType("#name1", "bike");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.List, "Find Store By Name");
        var summary = WaitForCss(".list .summary .details");
        Assert.AreEqual("Page 1 of 177; viewing 2 of 353 items", summary.Text);
        var next = GetInputButton("Next").AssertIsEnabled();
        Click(next);
        Wait.Until(dr => dr.FindElement(
                               By.CssSelector(".list .summary .details"))
                           .Text == "Page 2 of 177; viewing 2 of 353 items");
    }

    [TestMethod]
    public virtual void ListDoesNotRefreshWithoutReload() {
        GeminiUrl("list?m1=SpecialOfferRepository&a1=SpecialOffersWithNoMinimumQty&p1=1&ps1=20");
        Reload();
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
                           .StartsWith("Page 1 of 1;"));
        Click(HomeIcon());
        WaitForView(Pane.Single, PaneType.Home, "Home");
        GoToMenuFromHomePage("Special Offers");
        Click(GetObjectEnabledAction("Special Offers With No Minimum Qty"));
        WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");

        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
                         == "Page 1 of 1; viewing 11 of 11 items");

        WaitForCss("tbody tr:nth-child(2) td:nth-child(2)");

        var row = Driver.FindElement(By.CssSelector("tbody tr:nth-child(2) td:nth-child(2)"));
        Click(row);
        WaitForView(Pane.Single, PaneType.Object, "Mountain-100 Clearance Sale");
        //GeminiUrl("object?o1=___1.SpecialOffer--7");
        EditObject();
        ClearFieldThenType("#minqty1", "10");
        SaveObject();
        ClickBackButton();
        ClickBackButton();
        //ClickBackButton();

        WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");

        //Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
        //                 == "Page 1 of 1; viewing 11 of 11 items");
        Reload();
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
                         == "Page 1 of 1; viewing 10 of 10 items");

        //Undo to leave in original state
        GeminiUrl("object?o1=___1.SpecialOffer--7");
        EditObject();
        ClearFieldThenType("#minqty1", Keys.Backspace + Keys.Backspace + "0");
        SaveObject();
        GeminiUrl("list?m1=SpecialOfferRepository&a1=SpecialOffersWithNoMinimumQty&p1=1&ps1=20");
        WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");
        Reload();
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
                         == "Page 1 of 1; viewing 11 of 11 items");
    }

    [TestMethod]
    public virtual void ReloadingListGetsUpdatedObject() {
        Url(SpecialOffersMenuUrl);
        Click(GetObjectEnabledAction("Current Special Offers"));
        WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
        WaitForCss(".reference", 16);
        var row = WaitForCssNo(".reference", 6);
        Click(row);
        WaitForView(Pane.Single, PaneType.Object);
        EditObject();
        var rand = new Random();
        var suffix = rand.Next().ToString();
        var newDescription = "Mountain-100 Clearance Sale " + suffix;
        ClearFieldThenType("#description1", newDescription);
        SaveObject();

        ClickBackButton();
        ClickBackButton();
        WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
        Reload();
        WaitForCss(".reference", 16);
        row = WaitForCssNo(".reference", 6);
        Click(row);
        WaitForView(Pane.Single, PaneType.Object);
        Wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(1)")).Text.EndsWith(suffix));

        //Now revert
        EditObject();
        ClearFieldThenType("#description1", "Mountain-100 Clearance Sale");
        SaveObject();
    }

    [TestMethod]
    public virtual void EagerlyRenderTableViewFromAction() {
        GeminiUrl("home?m1=EmployeeRepository");
        Click(GetObjectEnabledAction("List All Departments"));
        WaitForView(Pane.Single, PaneType.List, "List All Departments");
        WaitForCss(".icon.list");
        var header = WaitForCss("thead");
        var cols = header.FindElements(By.CssSelector("th")).ToArray();
        Assert.AreEqual(2, cols.Length);
        Assert.AreEqual("", cols[0].Text);
        Assert.AreEqual("Group Name", cols[1].Text);
    }

    [TestMethod]
    public virtual void PagingTableView() {
        GeminiUrl("list?m1=CustomerRepository&a1=FindIndividualCustomerByName&p1=1&ps1=20&pm1_firstName=%22%22&pm1_lastName=%22a%22&c1=Table");
        Reload();
        //Confirm in Table view
        WaitForCss("thead tr th");
        WaitForCss(".icon.list");
        WaitUntilElementDoesNotExist(".icon.table");
        //Test content of collection
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 1 of"));
        GetInputButton("First").AssertIsDisabled();
        GetInputButton("Previous").AssertIsDisabled();
        var next = GetInputButton("Next").AssertIsEnabled();
        GetInputButton("Last").AssertIsEnabled();
        //Go to next page
        Click(next);
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 2 of"));
        //Confirm in Table view
        WaitForCss("thead tr th");
        WaitForCss(".icon.list");
        WaitUntilElementDoesNotExist(".icon.table");

        GetInputButton("First").AssertIsEnabled();
        GetInputButton("Previous").AssertIsEnabled();
        GetInputButton("Next").AssertIsEnabled();
        var last = GetInputButton("Last").AssertIsEnabled();
        Click(last);
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 45 of 45"));
        //Confirm in Table view
        WaitForCss("thead tr th");
        var iconList = WaitForCss(".icon.list");
        WaitUntilElementDoesNotExist(".icon.table");

        GetInputButton("First").AssertIsEnabled();
        var prev = GetInputButton("Previous").AssertIsEnabled();
        GetInputButton("Next").AssertIsDisabled();
        GetInputButton("Last").AssertIsDisabled();
        Click(prev);
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 44 of 45"));
        //Confirm in Table view
        WaitForCss("thead tr th");
        WaitForCss(".icon.list");
        WaitUntilElementDoesNotExist(".icon.table");
        var first = GetInputButton("First").AssertIsEnabled();
        GetInputButton("Previous").AssertIsEnabled();
        GetInputButton("Next").AssertIsEnabled();
        GetInputButton("Last").AssertIsEnabled();
        Click(first);
        Wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                           .Text.StartsWith("Page 1 of 45"));
        //Confirm in Table view
        WaitForCss("thead tr th");
        WaitForCss(".icon.list");
        WaitUntilElementDoesNotExist(".icon.table");
    }
}

[TestClass]
public class ListTestsChrome : ListTests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }

    protected override void ScrollTo(IWebElement element) {
        var script = $"window.scrollTo({element.Location.X}, {element.Location.Y});return true;";
        ((IJavaScriptExecutor)Driver).ExecuteScript(script);
    }
}