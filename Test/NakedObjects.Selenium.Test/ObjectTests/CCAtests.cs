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
///     Tests for collection-contributedActions
/// </summary>
public abstract class CCATests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void ListViewWithParmDialogAlreadyOpen() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0&as1=open&d1=ChangeMaxQuantity&f1_newMax=%22%22");
        WaitForView(Pane.Single, PaneType.List);
        Reload();
        var rand = new Random();
        var newMax = rand.Next(10, 999).ToString();
        TypeIntoFieldWithoutClearing("#newmax1", newMax);

        PageDownAndWait();
        //Now select items
        SelectCheckBox("#item1-5");
        SelectCheckBox("#item1-7");
        SelectCheckBox("#item1-9");
        Click(OKButton());
        WaitUntilElementDoesNotExist(".dialog");
        var maxQty = "Max Qty:";
        CheckIndividualItem(5, maxQty, newMax);
        CheckIndividualItem(7, maxQty, newMax);
        CheckIndividualItem(9, maxQty, newMax);
        //Confirm others have not
        CheckIndividualItem(6, maxQty, newMax, false);
        CheckIndividualItem(8, maxQty, newMax, false);
    }

    [TestMethod]
    public virtual void ListViewWithParmDialogNotOpen() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0&as1=open");
        Reload();
        wait.Until(dr => dr.FindElements(By.CssSelector("tbody tr .checkbox")).Count >= 16);
        PageDownAndWait();
        SelectCheckBox("#item1-2");
        SelectCheckBox("#item1-3");
        SelectCheckBox("#item1-4");
        OpenActionDialog("Change Max Quantity");
        var rand = new Random();
        var newMax = rand.Next(10, 999).ToString();
        TypeIntoFieldWithoutClearing("#newmax1", newMax);
        Click(OKButton());
        WaitUntilElementDoesNotExist(".dialog");
        Thread.Sleep(1000);
        var maxQty = "Max Qty:";
        PageDownAndWait();
        CheckIndividualItem(2, maxQty, newMax);
        CheckIndividualItem(3, maxQty, newMax);
        CheckIndividualItem(4, maxQty, newMax);
        //Confirm others have not
        CheckIndividualItem(1, maxQty, newMax, false);
        CheckIndividualItem(5, maxQty, newMax, false);
    }

    [TestMethod]
    public virtual void DateParam() {
        GeminiUrl("home");

        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1_=0&as1=open&c1=Table");
        Reload();

        wait.Until(dr => dr.FindElements(By.CssSelector("tbody tr .checkbox")).Count >= 16);
        PageDownAndWait();

        SelectCheckBox("#item1-6");
        SelectCheckBox("#item1-9");
        OpenActionDialog("Extend Offers");
        var rand = new Random();
        var futureDate = DateTime.Today.AddDays(rand.Next(1000));

        var outmask = "d MMM yyyy";
        var inmask = "dd/MM/yyyy";
        var inFutureDate = futureDate.ToString(inmask);
        var outFutureDate = futureDate.ToString(outmask);

        ClearFieldThenType("#todate1", inFutureDate + Keys.Escape);
        Click(OKButton());
        WaitUntilElementDoesNotExist(".dialog");
        Thread.Sleep(1000);
        Reload();
        WaitForView(Pane.Single, PaneType.List);
        //Check that exactly two rows were updated
        var endDate = "End Date:";
        CheckIndividualItem(6, endDate, outFutureDate);
        CheckIndividualItem(9, endDate, outFutureDate);
        CheckIndividualItem(7, endDate, outFutureDate, false);
        CheckIndividualItem(8, endDate, outFutureDate, false);
    }

    //To test an error that was previously being thrown by the RO server
    [TestMethod]
    public virtual void EmptyParam() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1_=0&as1=open&c1=Table");
        WaitForView(Pane.Single, PaneType.List);
        Thread.Sleep(1000);
        Reload();
        PageDownAndWait();
        SelectCheckBox("#item1-6");
        PageDownAndWait();
        SelectCheckBox("#item1-9");
        OpenActionDialog("Append To Description");
        Click(OKButton());
        WaitUntilElementDoesNotExist(".dialog");
        Reload();
    }

    [TestMethod]
    public virtual void TestSelectAll() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0");
        WaitForView(Pane.Single, PaneType.List);
        Reload();
        wait.Until(dr => dr.FindElements(By.CssSelector("input")).Count(el => el.GetAttribute("type") == "checkbox") > 10);
        var count = br.FindElements(By.CssSelector("input")).Count(el => el.GetAttribute("type") == "checkbox");
        WaitForSelectedCheckboxes(0);

        //Select all
        SelectCheckBox("#item1-all");
        WaitForSelectedCheckboxes(count);

        //Deselect all
        SelectCheckBox("#item1-all", true);
        WaitForSelectedCheckboxes(0);
    }

    [TestMethod]
    public virtual void SelectAllTableView() {
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0&c1=Table");
        Reload();
        WaitForCss("td", 64);
        WaitForSelectedCheckboxes(0);

        Thread.Sleep(1000);

        //Select all
        SelectCheckBox("#item1-all");
        WaitForSelectedCheckboxesAtLeast(17);

        //Deselect all
        SelectCheckBox("#item1-all", true);
        WaitForSelectedCheckboxes(0);
    }

    [TestMethod]
    public virtual void IfNoCCAs() {
        //test that Actions is disabled & no checkboxes appear
        GeminiUrl("list?m1=PersonRepository&a1=RandomContacts&pg1=1&ps1=20&s1_=0&c1=List");
        Reload();
        var actions = wait.Until(dr => dr.FindElements(By.CssSelector("input")).Single(el => el.GetAttribute("value") == "Actions"));
        Assert.AreEqual("true", actions.GetAttribute("disabled"));
        actions.AssertHasTooltip("No actions available");
        WaitUntilElementDoesNotExist("input[type='checkbox']");
    }

    [TestMethod]
    public virtual void SelectionRetainedWhenNavigatingAwayAndBack() {
        GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=1&ps1=20&s1_=152&c1=List");
        Reload();
        WaitForSelectedCheckboxes(3);
        Click(HomeIcon());
        WaitForView(Pane.Single, PaneType.Home);
        ClickBackButton();
        WaitForView(Pane.Single, PaneType.List);
        WaitForSelectedCheckboxes(3);
    }

    [TestMethod]
    public virtual void SelectionClearedWhenPageChanged() {
        GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=1&ps1=20&s1_=152&c1=List");
        Reload();
        WaitForTextStarting(".details", "Page 1 of ");
        WaitForSelectedCheckboxes(3);
        var row = WaitForCss(".reference"); //first one
        RightClick(row);
        WaitForView(Pane.Right, PaneType.Object);
        WaitForSelectedCheckboxes(3);
        Click(GetInputButton("Next", Pane.Left));
        WaitForTextStarting(".details", "Page 2 of ");
        WaitForSelectedCheckboxes(0);
        //Using back button retrieves original selection
        ClickBackButton();
        WaitForTextStarting(".details", "Page 1 of ");
        WaitForSelectedCheckboxes(3);
        //But going Next then Previous loses it
        Click(GetInputButton("Next", Pane.Left));
        WaitForTextStarting(".details", "Page 2 of ");
        WaitForSelectedCheckboxes(0);
        Click(GetInputButton("Previous", Pane.Left));
        WaitForTextStarting(".details", "Page 1 of ");
        WaitForSelectedCheckboxes(0);
    }

    [TestMethod]
    public virtual void TableViewWithParmDialogNotOpen() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0&as1=open&c1=Table");
        WaitForView(Pane.Single, PaneType.List);
        Reload();
        SelectCheckBox("#item1-2");
        SelectCheckBox("#item1-3");
        SelectCheckBox("#item1-4");
        OpenActionDialog("Change Discount");
        var rand = new Random();
        var newPct = "0." + rand.Next(51, 59);
        TypeIntoFieldWithoutClearing("#newdiscount1", newPct);
        Click(OKButton());
        WaitUntilElementDoesNotExist(".dialog");
        Reload();
        //Check that exactly three rows were updated
        CheckIndividualItem(1, "Discount Pct:", newPct, false);
        CheckIndividualItem(2, "Discount Pct:", newPct);
        CheckIndividualItem(3, "Discount Pct:", newPct);
        CheckIndividualItem(4, "Discount Pct:", newPct);
        CheckIndividualItem(5, "Discount Pct:", newPct, false);

        //Reset to below 50%
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0&as1=open&c1=Table");
        WaitForView(Pane.Single, PaneType.List);
        Reload();
        WaitForCss("td", 64);
        Thread.Sleep(1000); // temperamental on server 
        OpenActionDialog("Change Discount");
        TypeIntoFieldWithoutClearing("#newdiscount1", "0.10");
        SelectCheckBox("#item1-2");
        SelectCheckBox("#item1-3");
        SelectCheckBox("#item1-4");
        Click(OKButton());
        WaitUntilElementDoesNotExist(".dialog");
    }

    [TestMethod]
    [Ignore("#499")]
    public virtual void TableViewWithParmDialogAlreadyOpen() {
        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0&c1=Table&as1=open&d1=ChangeDiscount");
        Reload();
        var rand = new Random();
        var newPct = "0." + rand.Next(51, 59);
        TypeIntoFieldWithoutClearing("#newdiscount1", newPct);
        WaitForCss("td", 64);
        //Now select items
        PageDownAndWait();
        SelectCheckBox("#item1-6");
        PageDownAndWait();
        SelectCheckBox("#item1-8");
        Click(OKButton());
        WaitUntilElementDoesNotExist(".dialog");
        CheckIndividualItem(6, "Discount Pct:", newPct);
        CheckIndividualItem(7, "Discount Pct:", newPct, false);
        CheckIndividualItem(8, "Discount Pct:", newPct);

        GeminiUrl("home");
        WaitForView(Pane.Single, PaneType.Home);
        //Reset to below 50%
        GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0&c1=Table&as1=open&d1=ChangeDiscount");
        Reload();
        TypeIntoFieldWithoutClearing("#newdiscount1", "0.10");
        WaitForCss("td", 64);
        PageDownAndWait();
        SelectCheckBox("#item1-6");
        PageDownAndWait();
        SelectCheckBox("#item1-8");
        Click(OKButton());
        WaitUntilElementDoesNotExist(".dialog");
    }

    [TestMethod]
    public virtual void ReloadingAQueryableClearsSelection() {
        GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=20&ps1=5&s1_=0&as1=open");
        Reload();
        wait.Until(dr => dr.FindElements(By.CssSelector("td")).Count > 30);
        SelectCheckBox("#item1-4");
        SelectCheckBox("#item1-7");
        wait.Until(dr => dr.FindElements(By.CssSelector("input")).Where(el => el.GetAttribute("type") == "checkbox").Any(el => el.Selected));
        Reload();
        wait.Until(dr => !dr.FindElements(By.CssSelector("input")).Where(el => el.GetAttribute("type") == "checkbox").Any(el => el.Selected));
    }

    [TestMethod]
    public virtual void ZeroParamAction() {
        GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=20&ps1=5&s1_=0&as1=open&c1=Table");
        Reload();
        wait.Until(dr => dr.FindElements(By.CssSelector("td")).Count > 30);

        SelectCheckBox("#item1-all"); //To clear

        Click(GetObjectEnabledAction("Clear Comments"));

        Reload();
        wait.Until(dr => dr.FindElements(By.CssSelector("td")).Count > 30);

        //Now add comments
        SelectCheckBox("#item1-1");
        SelectCheckBox("#item1-2");
        SelectCheckBox("#item1-3");

        Click(GetObjectEnabledAction("Comment As Users Unhappy"));
        Thread.Sleep(2000); //Because there is no visible change to wait for
        Reload();

        wait.Until(dr => dr.FindElements(By.CssSelector("span.loading:empty")));

        wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text.Contains("User unhappy")) == 3);

        //Confirm that the three checkboxes have now been cleared
        wait.Until(dr => !dr.FindElements(By.CssSelector("input")).Where(el => el.GetAttribute("type") == "checkbox").Any(el => el.Selected));

        SelectCheckBox("#item1-all"); //To clear

        Click(GetObjectEnabledAction("Clear Comments"));
        Thread.Sleep(1000);
        Reload();
        wait.Until(dr => dr.FindElements(By.CssSelector("td")).Count > 30);

        wait.Until(dr => dr.FindElements(By.CssSelector("span.loading:empty")));
        wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text.Contains("User unhappy")) == 0);
    }
}

[TestClass]
public class CCATestsChrome : CCATests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }

    protected override void ScrollTo(IWebElement element) {
        var script = $"window.scrollTo({element.Location.X}, {element.Location.Y});return true;";
        ((IJavaScriptExecutor)br).ExecuteScript(script);
    }
}