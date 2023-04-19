// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Tests;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Selenium.Test.ObjectTests;

public abstract class ObjectEditTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void EditPropertyInline_usingEditAttribute() {
        GeminiUrl("object?o1=___1.SpecialOffer--10");
        var minQty = WaitForCssNo(".property", 7);
        Assert.IsTrue(minQty.Text.StartsWith("Min Qty"));
        var pencil = minQty.FindElement(By.CssSelector(".icon.edit"));
        Click(pencil);
        ClearFieldThenType("nof-edit-parameter input", "2");
        Click(WaitForCss(".form-row input.ok"));
        WaitForCssNo(".property", 9);
        minQty = WaitForCssNo(".property", 7);
        Assert.AreEqual("2", minQty.FindElement(By.CssSelector(".value")).Text);
        //Revert
        pencil = minQty.FindElement(By.CssSelector(".icon.edit"));
        Click(pencil);
        ClearFieldThenType("nof-edit-parameter input", "1");
        Click(WaitForCss(".form-row input.ok"));
        WaitForTextEquals(".property .value", 7, "1");
    }

    [TestMethod]
    public virtual void EditAttributeHonoursValidationMethod() {
        GeminiUrl("object?o1=___1.SpecialOffer--10");
        var minQty = WaitForCssNo(".property", 7);
        Assert.IsTrue(minQty.Text.StartsWith("Min Qty"));
        var pencil = minQty.FindElement(By.CssSelector(".icon.edit"));
        Click(pencil);
        ClearFieldThenType("nof-edit-parameter input", "0");
        Click(WaitForCss(".form-row input.ok"));
        Wait.Until(el => el.FindElement(By.CssSelector(".parameter .validation")).Text == "Min Qty must be > 0");
    }

    [TestMethod]
    public virtual void EditAttribute_ValidateOnMultipleProperties() {
        GeminiUrl("object?i1=View&o1=___1.WorkOrder--26138");
        var prop = WaitForCssNo(".property", 5);
        Assert.IsTrue(prop.Text.StartsWith("Start Date"));
        var pencil = prop.FindElement(By.CssSelector(".icon.edit"));
        Click(pencil);
        ClearFieldThenType("nof-edit-parameter:nth-of-type(1) input", "6 Jan 2007");
        ClearFieldThenType("nof-edit-parameter:nth-of-type(2) input", "5 Jan 2007");
        Thread.Sleep(1000);
        Click(WaitForCss(".form-row input.ok"));
        Thread.Sleep(1000);
        Wait.Until(el => el.FindElement(By.CssSelector(".co-validation")).Text == "Due date is before start date");
    }

    [TestMethod]
    public virtual void ObjectEditChangeScalar() {
        var rand = new Random();
        GeminiUrl("object?o1=___1.Product--870");
        EditObject();
        var oldPrice = WaitForCss("#listprice1").GetAttribute("value");
        var newPrice = rand.Next(50, 150);
        ClearFieldThenType("#listprice1", newPrice.ToString());

        var oldDays = WaitForCss("#daystomanufacture1").GetAttribute("value");

        var newDays = rand.Next(1, 49).ToString();
        ClearFieldThenType("#daystomanufacture1", newDays);
        SaveObject();

        var properties = Driver.FindElements(By.CssSelector(".property"));
        var currency = "£" + newPrice.ToString("c")[1..];
        Assert.AreEqual("List Price:\r\n" + currency, properties[5].Text);
        Assert.AreEqual("Days To Manufacture:\r\n" + newDays, properties[17].Text);
    }

    private bool isInt(string s) {
        int temp;
        return int.TryParse(s, out temp);
    }

    [TestMethod]
    public virtual void ObjectEditCancelLeavesUnchanged() {
        var rand = new Random();
        GeminiUrl("object?o1=___1.Product--870");
        EditObject();
        var oldPrice = WaitForCss("#listprice1").GetAttribute("value");

        // try to make more robust
        while (!isInt(oldPrice)) {
            oldPrice = WaitForCss("#listprice1").GetAttribute("value");
        }

        var newPrice = rand.Next(50, 150);
        ClearFieldThenType("#listprice1", newPrice.ToString());

        var oldDays = WaitForCss("#daystomanufacture1").GetAttribute("value");

        var newDays = rand.Next(1, 49).ToString();
        ClearFieldThenType("#daystomanufacture1", newDays);

        // triggers caching of values 
        ClickHomeButton();
        ClickBackButton();

        CancelObject();

        var currency = "£" + int.Parse(oldPrice).ToString("c")[1..];

        Wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[5].Text == "List Price:\r\n" + currency);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[17].Text == "Days To Manufacture:\r\n" + oldDays);
    }

    [TestMethod]
    public virtual void LocalValidationOfMandatoryFields() {
        GeminiUrl("object?i1=Edit&o1=___1.SpecialOffer--11");
        SaveButton().AssertIsEnabled();
        ClearFieldThenType("#startdate1", "");
        Thread.Sleep(1000);
        SaveButton().AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Start Date; ");
        ClearFieldThenType("#minqty1", "");
        SaveButton().AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Start Date; Min Qty; ");
        ClearFieldThenType("#description1", "");
        SaveButton().AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Description; Start Date; Min Qty; ");
        ClearFieldThenType("#minqty1", "1");
        SaveButton().AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Description; Start Date; ");
    }

    [TestMethod]
    public virtual void LocalValidationOfMaxLength() {
        GeminiUrl("object?i1=Edit&o1=___1.Person--12125&c1_Addresses=List&c1_EmailAddresses=List");
        ClearFieldThenType("#title1", "Generalis");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".validation")).Count(el => el.Text == "Too long") == 1);
        SaveButton().AssertIsDisabled().AssertHasTooltip("Invalid fields: Title; ");

        TypeIntoFieldWithoutClearing("#title1", Keys.Backspace);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".validation")).Count(el => el.Text == "Too long") == 0);
        SaveButton().AssertIsEnabled();
    }

    [TestMethod]
    public virtual void LocalValidationOfRegex() {
        GeminiUrl("object?i1=Edit&o1=___1.EmailAddress--12043--11238");
        ClearFieldThenType("#emailaddress11", "arthur44@adventure-works");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".validation")).Count(el => el.Text == "Invalid entry") == 1);
        SaveButton().AssertIsDisabled().AssertHasTooltip("Invalid fields: Email Address; ");

        TypeIntoFieldWithoutClearing("#emailaddress11", ".com");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".validation")).Count(el => el.Text == "Invalid entry") == 0);
        SaveButton().AssertIsEnabled();
    }

    [TestMethod]
    public virtual void RangeValidationOnNumber() {
        GeminiUrl("object?i1=Edit&o1=___1.Product--817");
        WaitForView(Pane.Single, PaneType.Object, "Editing - HL Mountain Front Wheel");
        Wait.Until(dr => dr.FindElement(By.CssSelector("#daystomanufacture1")).GetAttribute("value") == "1");
        Thread.Sleep(500);
        ClearFieldThenType("#daystomanufacture1", "0");
        Wait.Until(dr => dr
                         .FindElements(By.CssSelector(".property .validation")).Count(el => el.Text == "Value is outside the range 1 to 90") == 1);
        //Confirm that the save button is disabled & has helper tooltip
        SaveButton().AssertIsDisabled().AssertHasTooltip("Invalid fields: Days To Manufacture; ");

        ClearFieldThenType("#daystomanufacture1", "1");
        Wait.Until(dr => dr
                         .FindElements(By.CssSelector(".property .validation")).Count(el => el.Text == "Value is outside the range 1 to 90") == 0);
        //Confirm that the save button is disabled & has helper tooltip
        SaveButton().AssertIsEnabled();

        ClearFieldThenType("#daystomanufacture1", "91");
        Wait.Until(dr => dr
                         .FindElements(By.CssSelector(".property .validation")).Count(el => el.Text == "Value is outside the range 1 to 90") == 1);
        //Confirm that the save button is disabled & has helper tooltip
        SaveButton().AssertIsDisabled().AssertHasTooltip("Invalid fields: Days To Manufacture; ");
    }

    [TestMethod]
    public virtual void RangeValidationOnDate() {
        GeminiUrl("object?i1=Edit&o1=___1.Product--448");
        WaitForView(Pane.Single, PaneType.Object, "Editing - Lock Nut 13");
        var outmask = "d MMM yyyy";
        var inmask = "dd/MM/yyyy";
        var intoday = DateTime.Today.ToString(inmask);
        var outtoday = DateTime.Today.ToString(outmask, CultureInfo.InvariantCulture);
        var inyesterday = DateTime.Today.AddDays(-1).ToString(inmask);
        var ind10 = DateTime.Today.AddDays(10).ToString(inmask);
        var outd10 = DateTime.Today.AddDays(10).ToString(outmask, CultureInfo.InvariantCulture);
        var ind11 = DateTime.Today.AddDays(11).ToString(inmask);
        var message = $"Value is outside the range {outtoday} to {outd10}";
        ClearDateFieldThenType("#discontinueddate1", inyesterday);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))[20].Text == message);
        //Assert.AreEqual(message, WebDriver.FindElements(By.CssSelector(".property .validation"))[20].Text);

        //Wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))[17].Text == message);
        ClearFieldThenType("#discontinueddate1", intoday);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))[20].Text == "");
        ClearFieldThenType("#discontinueddate1", ind11);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation")).Count == 23);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))[20].Text == message);
        ClearFieldThenType("#discontinueddate1", ind10);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))[20].Text == "");
    }

    [TestMethod]
    public virtual void ObjectEditChangeEnum() {
        GeminiUrl("object?i1=View&o1=___1.Person--6748");
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[6].Text == "Email Promotion:\r\nNo Promotions");
        EditObject();
        SelectDropDownOnField("#emailpromotion1", "Adventureworks Only");
        SaveObject();
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[6].Text == "Email Promotion:\r\nAdventureworks Only");
        EditObject();
        SelectDropDownOnField("#emailpromotion1", "No Promotions");
        SaveObject();
        Wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[6].Text == "Email Promotion:\r\nNo Promotions");
    }

    [TestMethod]
    [Ignore("#502")]
    public virtual void ObjectEditChangeDateTime() {
        GeminiUrl("object?o1=___1.Product--870");
        EditObject();

        var rand = new Random();
        var date = new DateTime(2000, 1, 1);
        var sellStart = date.AddDays(rand.Next(2000));
        var sellEnd = date.AddDays(rand.Next(2000, 3000));
        Thread.Sleep(500);

        // todo chrome datepicker doesn't handle this
        //ClearDateFieldThenType("#sellstartdate1", sellStart.ToString("d MMM yyyy"));
        //ClearDateFieldThenType("#sellenddate1", sellEnd.ToString("dd/MM/yy")); //Test different input format...

        ClearFieldThenType("#sellstartdate1", sellStart.ToString("dd/MM/yyyy"));
        ClearFieldThenType("#sellenddate1", sellEnd.ToString("dd/MM/yyyy"));

        ClearFieldThenType("#daystomanufacture1", "1");
        Thread.Sleep(2000);
        SaveObject();

        var properties = Driver.FindElements(By.CssSelector(".property"));

        Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
        Assert.AreEqual("Sell Start Date:\r\n" + sellStart.ToString("d MMM yyyy", CultureInfo.InvariantCulture), properties[18].Text);
        Assert.AreEqual("Sell End Date:\r\n" + sellEnd.ToString("d MMM yyyy"), properties[19].Text); //...but output format standardised.
    }

    [TestMethod]
    public virtual void CanSetAndClearAnOptionalDropDown() {
        GeminiUrl("object?o1=___1.WorkOrder--54064");
        WaitForView(Pane.Single, PaneType.Object);
        EditObject();
        SelectDropDownOnField("#scrapreason1", "Color incorrect");
        SaveObject();
        var prop = WaitForCssNo(".property", 4);
        Assert.AreEqual("Scrap Reason:\r\nColor incorrect", prop.Text);
        EditObject();
        SelectDropDownOnField("#scrapreason1", "");
        SaveObject();
        prop = WaitForCssNo(".property", 4);
        Assert.AreEqual("Scrap Reason:", prop.Text);
    }

    [TestMethod]
    public virtual void ObjectEditPicksUpLatestServerVersion() {
        GeminiUrl("object?o1=___1.Person--8410&as1=open");
        WaitForView(Pane.Single, PaneType.Object);
        var original = WaitForCss(".property:nth-child(6) .value").Text;
        var dialog = OpenActionDialog("Update Suffix"); //This is deliberately wrongly marked up as QueryOnly
        var field1 = WaitForCss(".parameter:nth-child(1) input");
        var newValue = DateTime.Now.Millisecond.ToString();
        ClearFieldThenType(".parameter:nth-child(1) input", newValue);
        Click(OKButton()); //This will have updated server, but not client-cached object
        WaitUntilElementDoesNotExist(".dialog");
        //Check view has not updated because it was a queryonly action
        Assert.AreEqual(original, WaitForCss(".property:nth-child(6) .value").Text);
        EditObject(); //This will update object from server
        Click(GetCancelEditButton()); //but can't read the value, so go back to view
        Assert.AreEqual(newValue, WaitForCss(".property:nth-child(6) .value").Text);
    }

    [TestMethod]
    public virtual void ViewModelEditOpensInEditMode() {
        GeminiUrl("object?i1=Form&r1=1&o1=___1.EmailTemplate----------New");
        WaitForCss("input#to1");
        WaitForCss("input#from1");
    }

    [TestMethod]
    public virtual void MultiLineText() {
        GeminiUrl("object?o1=___1.SalesOrderHeader--44440&as1=open");
        WaitForView(Pane.Single, PaneType.Object);
        Click(GetObjectEnabledAction("Clear Comment"));

        WaitUntilElementDoesNotExist(".tempdisabled");
        var dialog = OpenActionDialog("Add Multi Line Comment");
        var field1 = WaitForCss(".parameter:nth-child(1) textarea");
        ClearFieldThenType(".parameter:nth-child(1) textarea", "comment");
        Click(OKButton());

        Wait.Until(d => d.FindElement(By.CssSelector(".property .value.multiline")).Text == "comment");

        EditObject();
        var ta = WaitForCss("textarea#comment1");
        Assert.AreEqual("Free-form text", ta.GetAttribute("placeholder"));
        var rand = new Random();
        var ran1 = rand.Next(10000);
        var ran2 = rand.Next(10000);
        var ran3 = rand.Next(10000);
        ClearFieldThenType("#comment1", ran1 + Keys.Enter + ran2 + Keys.Enter + ran3);
        Click(SaveButton());

        Wait.Until(d => Driver.FindElement(By.CssSelector(".property .value.multiline")).Text ==
                        $"{ran1}\r\n{ran2}\r\n{ran3}");
    }

    [TestMethod]
    public virtual void ObjectEditChangeChoices() {
        GeminiUrl("object?o1=___1.Product--870");
        EditObject();

        // set product line 

        SelectDropDownOnField("#productline1", "S"); //Seems to need the space

        ClearFieldThenType("#daystomanufacture1", "1");
        SaveObject();

        var properties = Driver.FindElements(By.CssSelector(".property"));

        Assert.AreEqual("Product Line:\r\nS", properties[8].Text);
    }

    [TestMethod]
    public virtual void ObjectEditChangeConditionalChoices() {
        GeminiUrl("object?o1=___1.Product--870");
        EditObject();
        // set product category and sub category
        SelectDropDownOnField("#productcategory1", "Clothing");

        Wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Any(el => el.Text == "Bib-Shorts"));

        SelectDropDownOnField("#productsubcategory1", "Bib-Shorts");

        ClearFieldThenType("#daystomanufacture1", Keys.Backspace + "1");

        SaveObject();

        var properties = Driver.FindElements(By.CssSelector(".property"));

        Assert.AreEqual("Product Category:\r\nClothing", properties[6].Text);
        Assert.AreEqual("Product Subcategory:\r\nBib-Shorts", properties[7].Text);

        EditObject();

        // set product category and sub category
        Wait.Until(d => d.FindElement(By.CssSelector("select#productcategory1")));
        var slctd = new SelectElement(Driver.FindElement(By.CssSelector("select#productcategory1")));

        Assert.AreEqual("Clothing", slctd.SelectedOption.Text);

        Assert.AreEqual(5, Driver.FindElements(By.CssSelector("select#productcategory1 option")).Count);

        Wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Count == 9);

        Assert.AreEqual(9, Driver.FindElements(By.CssSelector("select#productsubcategory1 option")).Count);

        SelectDropDownOnField("#productcategory1", "Bikes");

        Wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Count == 4);

        SelectDropDownOnField("#productsubcategory1", "Mountain Bikes");

        SaveObject();

        properties = Driver.FindElements(By.CssSelector(".property"));

        Assert.AreEqual("Product Category:\r\nBikes", properties[6].Text);
        Assert.AreEqual("Product Subcategory:\r\nMountain Bikes", properties[7].Text);

        // set values back

        EditObject();

        Wait.Until(d => d.FindElement(By.CssSelector("select#productcategory1")));
        SelectDropDownOnField("select#productcategory1", "Accessories");
        Wait.Until(d => d.FindElement(By.CssSelector("select#productsubcategory1")));
        var slpsc = new SelectElement(Driver.FindElement(By.CssSelector("select#productsubcategory1")));
        Wait.Until(d => slpsc.Options.Count == 13);

        SelectDropDownOnField("#productsubcategory1", "Bottles and Cages");
        SaveObject();

        properties = Driver.FindElements(By.CssSelector(".property"));

        Assert.AreEqual("Product Category:\r\nAccessories", properties[6].Text);
        Assert.AreEqual("Product Subcategory:\r\nBottles and Cages", properties[7].Text);
    }

    [TestMethod]
    public virtual void CoValidationOnSavingChanges() {
        GeminiUrl("object?o1=___1.WorkOrder--43134&i1=Edit");
        WaitForView(Pane.Single, PaneType.Object);
        ClearFieldThenType("input#startdate1", "17 Oct 2007");
        ClearFieldThenType("input#duedate1", "15 Oct 2007");
        Thread.Sleep(2000);
        Click(SaveButton());
        WaitForMessage("StartDate must be before DueDate");
    }

    [TestMethod]
    // test for #108
    public virtual void ObjectEditScalarAutocomplete() {
        GeminiUrl("object?i1=Edit&o1=___1.Vendor--1686");
        WaitForView(Pane.Single, PaneType.Object);
        // autocomplete is rendered
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-edit-property .name"))[5].Text.StartsWith("Purchasing Web Service URL:"));
    }
}

[TestClass]
public class ObjectEditTestsChrome : ObjectEditTests {
    [TestInitialize]
    public virtual void InitializeTest() {
        Url(BaseUrl);
    }
}