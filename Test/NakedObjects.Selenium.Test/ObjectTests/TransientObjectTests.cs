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

public abstract class TransientObjectTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void CreateAndSaveTransientObject() {
        GeminiUrl("object?o1=___1.Person--12043&as1=open");
        Click(GetObjectEnabledAction("Create New Credit Card"));
        Wait.Until(d => d.FindElements(By.CssSelector("select#cardtype1 option")).First(el => el.Text == "*"));
        SelectDropDownOnField("#cardtype1", "Vista");
        var number = DateTime.Now.Ticks.ToString(); //pseudo-random string
        var obfuscated = number[^4..].PadLeft(number.Length, '*');
        ClearFieldThenType("#cardnumber1", number);
        SelectDropDownOnField("#expmonth1", "12");

        var year = (DateTime.Now.Year + 1).ToString();

        SelectDropDownOnField("#expyear1", year);
        Click(SaveButton());
        WaitForView(Pane.Single, PaneType.Object, obfuscated);
    }

    [TestMethod]
    public virtual void SaveAndClose() {
        GeminiUrl("object?o1=___1.Person--12043&as1=open");
        Click(GetObjectEnabledAction("Create New Credit Card"));
        SelectDropDownOnField("#cardtype1", "Vista");
        var number = DateTime.Now.Ticks.ToString(); //pseudo-random string
        var obfuscated = number[^4..].PadLeft(number.Length, '*');
        ClearFieldThenType("#cardnumber1", number);
        SelectDropDownOnField("#expmonth1", "12");

        var year = (DateTime.Now.Year + 1).ToString();

        SelectDropDownOnField("#expyear1", year);
        Click(SaveAndCloseButton());
        WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
        //But check that credit card was saved nonetheless
        GetObjectEnabledAction("Recent Credit Cards").Click();
        WaitForView(Pane.Single, PaneType.List, "Recent Credit Cards");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".list table tbody tr td.reference")).First().Text == obfuscated);
    }

    [TestMethod]
    public virtual void MissingMandatoryFieldsNotified() {
        GeminiUrl("object?o1=___1.Person--12043&as1=open");
        Click(GetObjectEnabledAction("Create New Credit Card"));
        SelectDropDownOnField("#cardtype1", "Vista");

        var year = (DateTime.Now.Year + 1).ToString();

        SelectDropDownOnField("#expyear1", year);
        SaveButton().AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Card Number; Exp Month; ");
    }

    [TestMethod]
    public virtual void IndividualFieldValidation() {
        GeminiUrl("object?o1=___1.Person--12043&as1=open");
        Click(GetObjectEnabledAction("Create New Credit Card"));
        SelectDropDownOnField("#cardtype1", "Vista");
        ClearFieldThenType("input#cardnumber1", "123");
        SelectDropDownOnField("#expmonth1", "1");

        var year = (DateTime.Now.Year + 1).ToString();

        SelectDropDownOnField("#expyear1", year);
        Click(SaveButton());
        Wait.Until(dr => dr.FindElements(
                       By.CssSelector(".validation")).Any(el => el.Text == "card number too short"));
        WaitForMessage("See field validation message(s).");
    }

    [TestMethod]
    public virtual void MultiFieldValidation() {
        GeminiUrl("object?o1=___1.Person--12043&as1=open");
        Click(GetObjectEnabledAction("Create New Credit Card"));
        SelectDropDownOnField("#cardtype1", "Vista");
        ClearFieldThenType("#cardnumber1", "1111222233334444");
        SelectDropDownOnField("#expmonth1", "1");

        var year = (DateTime.Now.Year - 1).ToString();

        SelectDropDownOnField("#expyear1", year);
        Click(SaveButton());
        WaitForMessage("Expiry date must be in the future");
    }

    [TestMethod]
    public virtual void PropertyDescriptionAndRequiredRenderedAsPlaceholder() {
        GeminiUrl("object?o1=___1.Person--12043&as1=open");
        Click(GetObjectEnabledAction("Create New Credit Card"));
        var name = WaitForCss("input#cardnumber1");
        Assert.AreEqual("* Without spaces", name.GetAttribute("placeholder"));
    }

    [TestMethod]
    public virtual void CancelTransientObject() {
        GeminiUrl("object?o1=___1.Person--12043&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
        Click(GetObjectEnabledAction("Create New Credit Card"));
        Click(GetCancelEditButton());
        WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
    }

    [TestMethod]
    public virtual void SwapPanesWithTransients() {
        GeminiUrl("object/object?o1=___1.Product--738&as1=open&o2=___1.Person--20774&as2=open");
        WaitForView(Pane.Left, PaneType.Object, "LL Road Frame - Black, 52");
        WaitForView(Pane.Right, PaneType.Object, "Isabella Richardson");

        OpenSubMenu("Work Orders", Pane.Left);
        Click(GetObjectEnabledAction("Create New Work Order", Pane.Left));
        WaitForView(Pane.Left, PaneType.Object, "Editing - Unsaved Work Order");
        ClearFieldThenType("#orderqty1", "4");
        Thread.Sleep(1000);
        Click(GetObjectEnabledAction("Create New Credit Card", Pane.Right));
        WaitForView(Pane.Right, PaneType.Object, "Editing - Unsaved Credit Card");
        ClearFieldThenType("#cardnumber2", "1111222233334444");

        Click(SwapIcon());
        WaitForView(Pane.Left, PaneType.Object, "Editing - Unsaved Credit Card");
        Wait.Until(dr => dr.FindElement(By.CssSelector("#cardnumber1")).GetAttribute("value") == "1111222233334444");
        WaitForView(Pane.Right, PaneType.Object, "Editing - Unsaved Work Order");
        Wait.Until(dr => dr.FindElement(By.CssSelector("#orderqty2")).GetAttribute("value") == "4");
    }

    [TestMethod]
    public virtual void BackAndForwardOverTransient() {
        GeminiUrl("object?o1=___1.Person--12043&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
        Click(GetObjectEnabledAction("Create New Credit Card"));
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Credit Card");
        Click(Driver.FindElement(By.CssSelector(".icon.back")));
        WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
        Click(Driver.FindElement(By.CssSelector(".icon.forward")));
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Credit Card");
    }

    [TestMethod]
    public virtual void RequestForExpiredTransient() {
        GeminiUrl("object?i1=Transient&o1=___1.CreditCard--100");
        Wait.Until(dr => dr.FindElement(By.CssSelector(".title")).Text == "The requested view of unsaved object details has expired.");
    }

    [TestMethod]
    public virtual void ConditionalChoicesOnTransient() {
        GeminiUrl("home?m1=ProductRepository");
        WaitForView(Pane.Single, PaneType.Home);
        Thread.Sleep(1000); // no idea why this keeps failing on server 
        Click(GetObjectEnabledAction("New Product"));
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Product");

        var sellStartDate = Driver.FindElement(By.CssSelector("#sellstartdate1"));
        Assert.AreEqual("* ", sellStartDate.GetAttribute("placeholder"));

        // set product category and sub category
        SelectDropDownOnField("#productcategory1", "Clothing");

        Wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Any(el => el.Text == "Bib-Shorts"));

        SelectDropDownOnField("#productsubcategory1", "Bib-Shorts");

        SelectDropDownOnField("#productcategory1", "Bikes");

        Wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Count == 4);

        SelectDropDownOnField("#productsubcategory1", "Mountain Bikes");
    }

    [TestMethod]
    public virtual void TransientWithHiddenNonOptionalFields() {
        GeminiUrl("object?i1=View&o1=___1.Product--380&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Hex Nut 8");
        Click(GetObjectEnabledAction("Create New Work Order", Pane.Single, "Work Orders"));
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Work Order");
        ClearFieldThenType("#scrappedqty1", "1");
        ClearFieldThenType("#orderqty1", "1");
        SaveObject();
    }

    [TestMethod]
    //Test for a previous bug  -  where Etag error was resulting
    public virtual void CanInvokeActionOnASavedTransient() {
        GeminiUrl("object?i1=View&o1=___1.Customer--11783&as1=open&d1=CreateNewOrder&f1_copyHeaderFromLastOrder=true");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Sales Order");
        Click(SaveButton());
        OpenObjectActions();
        OpenActionDialog("Add New Sales Reasons");
        SelectDropDownOnField("#reasons1", 1);
        Click(OKButton());
        Wait.Until(d => Driver.FindElements(By.CssSelector(".collection"))[1].Text == "Reasons:\r\n1 Item");
    }

    [TestMethod]
    public virtual void TransientCreatedFromDialogClosesDialog() {
        GeminiUrl("object?i1=View&o1=___1.Customer--30107&as1=open");
        OpenSubMenu("Orders");
        OpenActionDialog("Create New Order");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Sales Order");
        SaveObject();
        ClickBackButton();
        WaitForTextEquals(".title", "The requested view of unsaved object details has expired.");
        ClickBackButton();
        WaitForView(Pane.Single, PaneType.Object, "Permanent Finish Products, AW00030107");
        OpenSubMenu("Orders"); //Would fail if already open
    }

    [TestMethod]
    public virtual void CreateAndSaveNotPersistedObject() {
        GeminiUrl("home?m1=EmployeeRepository");
        Click(GetObjectEnabledAction("Create Staff Summary"));
        WaitForView(Pane.Single, PaneType.Object, "Staff Summary");
        // todo fix once type is no longer displayed
        WaitForTextStarting(".object", "Staff Summary\r\nFemale"); //i.e. no buttons in the header
    }

    [TestMethod]
    public virtual void ValuePropOnTransientEmptyIfNoDefault() {
        GeminiUrl("object?i1=View&o1=___1.Product--497&as1=open");
        OpenSubMenu("Work Orders");
        Click(GetObjectEnabledAction("Create New Work Order"));
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Work Order");
        var field = WaitForCss("#orderqty1");
        Assert.AreEqual("", field.GetAttribute("value"));
    }

    [TestMethod]
    //Test written against a specific failure scenario
    public virtual void InvalidPropOnTransientClearedAndReentered() {
        GeminiUrl("object?i1=View&o1=___1.Product--497&as1=open");
        OpenSubMenu("Work Orders");
        Click(GetObjectEnabledAction("Create New Work Order"));
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Work Order");
        ClearFieldThenType("#scrappedqty1", "0");
        ClearFieldThenType("#orderqty1", "0");
        Click(SaveButton());
        Wait.Until(dr => dr.FindElements(By.CssSelector(".validation"))
                           .Any(el => el.Text == "Order Quantity must be > 0"));
        ClearFieldThenType("#orderqty1", "1");
        Click(SaveButton());
        WaitForTextStarting(".title", "Pinch Bolt");
    }

    [TestMethod]
    public virtual void AutoCompletePropOnTransient() {
        GeminiUrl("object?i1=View&o1=___1.Customer--635&as1=open&d1=CreateNewOrder");
        Click(OKButton());
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Sales Order");
        Wait.Until(dr => dr.FindElement(By.CssSelector("#salesperson1")).GetAttribute("placeholder") == "(auto-complete or drop)");
        ClearFieldThenType("#salesperson1", "Va");

        // nof custom 
        Wait.Until(d => d.FindElements(By.CssSelector(".suggestions a")).Count > 0);

        // anagular/material
        //Wait.Until(d => d.FindElements(By.CssSelector("md-option")).Count > 0);
    }

    [TestMethod]
    // test for bug #104
    // add modified date visibility checks for bug #195
    public virtual void TransientWithHiddenUntilPersistedFields() {
        GeminiUrl("object?i1=View&o1=___1.Product--390&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Hex Nut 20");
        Click(GetObjectEnabledAction("Create New Work Order", Pane.Single, "Work Orders"));
        WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Work Order");
        ClearFieldThenType("#scrappedqty1", "1");
        ClearFieldThenType("#orderqty1", "1");

        // no end date or routings 
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-edit-property .name"))[6].Text.StartsWith("Due Date:"));
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-collection .name")).Count == 0);
        // visible modified date 
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-edit-property .name"))[7].Text.StartsWith("Modified Date:"));

        SaveObject();

        // visible end date and routings
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-view-property .name")).Count == 8);
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-view-property .name"))[6].Text == "End Date:");
        Wait.Until(dr => dr.FindElement(By.CssSelector("nof-collection .name")).Text == "Work Order Routings:");
        // no modified date
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-view-property .name"))[7].Text == "Due Date:");

        // visible add routing action
        OpenObjectActions();
        GetObjectEnabledAction("Add New Routing");
    }

    [TestMethod]
    // test for bug #128
    public virtual void PersistentWithHiddenUntilPersistedFields() {
        GeminiUrl("object?i1=View&o1=___1.Product--390&as1=open");
        WaitForView(Pane.Single, PaneType.Object, "Hex Nut 20");

        Click(GetObjectEnabledAction("Create New Work Order3", Pane.Single, "Work Orders"));
        ClearFieldThenType("#orderqty1", "1");
        Click(OKButton());

        // visible end date and routings
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-view-property .name"))[6].Text == "End Date:");
        Wait.Until(dr => dr.FindElement(By.CssSelector("nof-collection .name")).Text == "Work Order Routings:");
        // visible add routing action
        OpenObjectActions();
        GetObjectEnabledAction("Add New Routing");
    }

    [TestMethod]
    // test for bug #137
    public virtual void TransientWithOtherPaneChanges() {
        GeminiUrl("home/home");

        WaitForCss("#pane1 nof-menu-bar nof-action input", MainMenusCount);
        WaitForCss("#pane2 nof-menu-bar nof-action input", MainMenusCount);

        OpenMenu("Purchase Orders", Pane.Left);
        Click(GetObjectEnabledAction("Create New Purchase Order2", Pane.Left));
        WaitForView(Pane.Left, PaneType.Object, "Editing - Unsaved Purchase Order Header");

        OpenMenu("Vendors", Pane.Right);
        Click(GetObjectEnabledAction("Random Vendor", Pane.Right));
        WaitForView(Pane.Right, PaneType.Object);

        WaitForCss("#pane1 [value='Save']");

        SaveButton(Pane.Left).AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Vendor; Order Placed By; Ship Date; ");

        var title = WaitForCss("#pane2 .header .title");
        title.Click();
        CopyToClipboard(title);

        PasteIntoReferenceField("#vendor1");

        ClickBackButton();
        WaitForView(Pane.Right, PaneType.Home);
        WaitForCss("#pane1 [value='Save']");

        SaveButton(Pane.Left).AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Order Placed By; Ship Date; ");

        OpenMenu("Employees", Pane.Right);
        Click(GetObjectEnabledAction("Random Employee", Pane.Right));
        WaitForView(Pane.Right, PaneType.Object);

        var title1 = WaitForCss("#pane2 .header .title");
        title1.Click();
        CopyToClipboard(title1);

        PasteIntoReferenceField("#orderplacedby1");

        WaitForCss("#pane1 [value='Save']");
        SaveButton(Pane.Left).AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Ship Date; ");

        var today = DateTime.Today;
        ClearFieldThenType("#shipdate1", today.ToString("dd/MM/yyyy"));
        WaitForCss("#pane1 [value='Save']");

        WaitUntilEnabled(SaveButton(Pane.Left));
        SaveButton(Pane.Left).AssertIsEnabled().AssertHasTooltip("");

        Click(SaveButton(Pane.Left));

        // US/UK locale
        var date = today.ToString("M/d/yyyy") + " 12:00:00 AM";
        //var date = today.ToString("dd/MM/yyyy") + " 00:00:00";
        WaitForView(Pane.Left, PaneType.Object, date);
    }
}

[TestClass]
public class TransientObjectTestsChrome : TransientObjectTests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }
}