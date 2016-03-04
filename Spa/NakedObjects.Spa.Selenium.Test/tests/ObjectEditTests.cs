// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Web.UnitTests.Selenium
{

    public abstract class ObjectEditTestsRoot : AWTest
    {

        public virtual void ObjectEditChangeScalar()
        {
            var rand = new Random();
            GeminiUrl("object?o1=___1.Product-870");
            EditObject();
            var oldPrice = WaitForCss("#listprice1").GetAttribute("value");
            var newPrice = rand.Next(50, 150);
            ClearFieldThenType("#listprice1", newPrice.ToString());

            var oldDays = WaitForCss("#daystomanufacture1").GetAttribute("value");

            var newDays = rand.Next(1, 49).ToString();
            ClearFieldThenType("#daystomanufacture1", newDays);
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("List Price:\r\n" + newPrice.ToString("c"), properties[5].Text);
            Assert.AreEqual("Days To Manufacture:\r\n" + newDays, properties[17].Text);
        }

        public virtual void ObjectEditChangeDateTime()
        {
            GeminiUrl("object?o1=___1.Product-870");
            EditObject();

            // set price and days to mfctr
            var date = new DateTime(2014, 7, 18);
            var dateStr = date.ToString("d MMM yyyy");

            //for (int i = 0; i < 12; i++) {
            //    ClearFieldThenType("#sellstartdate1", Keys.Backspace);
            //}

            ClearFieldThenType("#sellstartdate1", dateStr);
            ClearFieldThenType("#daystomanufacture1", "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Sell Start Date:\r\n" + dateStr, properties[18].Text);
            Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
        }

        public virtual void ObjectEditChangeChoices()
        {
            GeminiUrl("object?o1=___1.Product-870");
            EditObject();

            // set product line 

            SelectDropDownOnField("#productline1", "S");

            ClearFieldThenType("#daystomanufacture1", Keys.Backspace + "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Line:\r\nS", properties[8].Text);
        }

        public virtual void CanSetAndClearAnOptionalDropDown()
        {
            GeminiUrl("object?o1=___1.WorkOrder-54064");
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

        public virtual void ObjectEditChangeConditionalChoices()
        {
            GeminiUrl("object?o1=___1.Product-870");
            EditObject();
            // set product category and sub category
            SelectDropDownOnField("#productcategory1", "Clothing");

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Any(el => el.Text == "Bib-Shorts"));

            SelectDropDownOnField("#productsubcategory1", "Bib-Shorts");

            ClearFieldThenType("#daystomanufacture1", Keys.Backspace + "1");

            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nClothing", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nBib-Shorts", properties[7].Text);

            EditObject();

            // set product category and sub category

            var slctd = new SelectElement(br.FindElement(By.CssSelector("select#productcategory1")));

            Assert.AreEqual("Clothing", slctd.SelectedOption.Text);

            Assert.AreEqual(5, br.FindElements(By.CssSelector("select#productcategory1 option")).Count);

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Count == 9);

            Assert.AreEqual(9, br.FindElements(By.CssSelector("select#productsubcategory1 option")).Count);

            SelectDropDownOnField("#productcategory1", "Bikes");

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Count == 4);

            SelectDropDownOnField("#productsubcategory1", "Mountain Bikes");

            SaveObject();

            properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nBikes", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nMountain Bikes", properties[7].Text);

            // set values back
            EditObject();

            SelectDropDownOnField("#productcategory1", "Accessories");

            var slpsc = new SelectElement(br.FindElement(By.CssSelector("select#productsubcategory1")));
            wait.Until(d => slpsc.Options.Count == 13);

            SelectDropDownOnField("#productsubcategory1", "Bottles and Cages");
            SaveObject();

            properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nAccessories", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nBottles and Cages", properties[7].Text);
        }

        public virtual void ObjectEditPicksUpLatestServerVersion()
        {

            GeminiUrl("object?o1=___1.Person-8410&as1=open");
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

        public virtual void SinglePropertyValidationError()
        {
            GeminiUrl("object?i1=Edit&o1=___1.Product-817");
            WaitForView(Pane.Single, PaneType.Object, "Editing - HL Mountain Front Wheel");
            ClearFieldThenType("#daystomanufacture1", "0");
            Click(SaveButton());
            WaitForMessage("See field validation message(s).");

            wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))
            .Where(el => el.Text == "Value is outside the range 1 to 90").Count() == 1);

            //Test for an earlier bug, that references still rendered correctly
            var field = WaitForCss("#productmodel1");
            Assert.AreEqual("HL Mountain Front Wheel", field.GetAttribute("value"));
        }

        public virtual void CoValidationOnSavingChanges()
        {
            GeminiUrl("object?o1=___1.WorkOrder-43134&i1=Edit");
            WaitForView(Pane.Single, PaneType.Object);
            ClearFieldThenType("input#startdate1", ""); //Seems to be necessary to clear the date fields fully
            ClearFieldThenType("input#startdate1", "");
            ClearFieldThenType("input#startdate1", "17 Oct 2007");
            ClearFieldThenType("input#enddate1", ""); //Seems to be necessary to clear the date fields fully
            ClearFieldThenType("input#enddate1", "");
            ClearFieldThenType("input#enddate1", "15 Oct 2007");
            Click(SaveButton());
            WaitForMessage("StartDate must be before EndDate");
        }

        public virtual void ViewModelEditOpensInEditMode()
        {
            GeminiUrl("object?o1=___1.EmailTemplate-1&i1=Form");
            WaitForCss("input#to1");
            WaitForCss("input#from1");
            //TODO: Check that actions are rendered e.g. Send
            //as individual buttons, and NO generic Save button
        }
    }
    public abstract class ObjectEditTests : ObjectEditTestsRoot
    {

        [TestMethod]
        public override void ObjectEditChangeScalar() { base.ObjectEditChangeScalar(); }

        [TestMethod]
        public override void ObjectEditChangeDateTime() { base.ObjectEditChangeDateTime(); }

        [TestMethod]
        public override void ObjectEditChangeChoices() { base.ObjectEditChangeChoices(); }
        [TestMethod]
        public override void CanSetAndClearAnOptionalDropDown() { base.CanSetAndClearAnOptionalDropDown(); }

        [TestMethod]
        public override void ObjectEditChangeConditionalChoices() { base.ObjectEditChangeConditionalChoices(); }

        [TestMethod]
        public override void ObjectEditPicksUpLatestServerVersion() { base.ObjectEditPicksUpLatestServerVersion(); }
        [TestMethod]
        public override void SinglePropertyValidationError() { base.SinglePropertyValidationError(); }

        [TestMethod]
        public override void CoValidationOnSavingChanges() { base.CoValidationOnSavingChanges(); }

        [TestMethod]
        public override void ViewModelEditOpensInEditMode() { base.ViewModelEditOpensInEditMode(); }
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class ObjectEditPageTestsIe : ObjectEditTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    //[TestClass]
    public class ObjectEditPageTestsFirefox : ObjectEditTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element)
        {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }
    }

    //[TestClass, Ignore]
    public class ObjectEditPageTestsChrome : ObjectEditTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitChromeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    #endregion

    #region Mega tests
    public abstract class MegaObjectEditTestsRoot : ObjectEditTestsRoot
    {
        [TestMethod]
        public void MegaObjectEditTest()
        {
            base.ObjectEditChangeScalar();
            base.ObjectEditChangeDateTime();
            base.ObjectEditChangeChoices();
            base.CanSetAndClearAnOptionalDropDown();
            base.ObjectEditChangeConditionalChoices();
            base.ObjectEditPicksUpLatestServerVersion();
            base.SinglePropertyValidationError();
            base.CoValidationOnSavingChanges();
            base.ViewModelEditOpensInEditMode();
        }
    }

    [TestClass]
    public class MegaObjectEditTestsFirefox : MegaObjectEditTestsRoot
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }
    #endregion
}