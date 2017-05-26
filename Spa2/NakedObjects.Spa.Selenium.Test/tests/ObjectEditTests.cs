// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Selenium {
    public abstract class ObjectEditTestsRoot : AWTest {
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

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));
            string currency = "£" + newPrice.ToString("c").Substring(1);
            Assert.AreEqual("List Price:\r\n" + currency, properties[5].Text);
            Assert.AreEqual("Days To Manufacture:\r\n" + newDays, properties[17].Text);
        }

        public virtual void ObjectEditCancelLeavesUnchanged() {
            var rand = new Random();
            GeminiUrl("object?o1=___1.Product--870");
            EditObject();
            var oldPrice = WaitForCss("#listprice1").GetAttribute("value");
            var newPrice = rand.Next(50, 150);
            ClearFieldThenType("#listprice1", newPrice.ToString());

            var oldDays = WaitForCss("#daystomanufacture1").GetAttribute("value");

            var newDays = rand.Next(1, 49).ToString();
            ClearFieldThenType("#daystomanufacture1", newDays);

            // triggers caching of values 
            ClickHomeButton();
            ClickBackButton();

            CancelObject();

            string currency = "£" + int.Parse(oldPrice).ToString("c").Substring(1);
        
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[5].Text == "List Price:\r\n" + currency);
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[17].Text == "Days To Manufacture:\r\n" + oldDays);
        }


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

        public virtual void LocalValidationOfMaxLength() {
            GeminiUrl("object?i1=Edit&o1=___1.Person--12125&c1_Addresses=List&c1_EmailAddresses=List");
            ClearFieldThenType("#title1", "Generalis");
            wait.Until(dr => dr.FindElements(By.CssSelector(".validation")).Where(el => el.Text == "Too long").Count() == 1);
            SaveButton().AssertIsDisabled().AssertHasTooltip("Invalid fields: Title; ");

            TypeIntoFieldWithoutClearing("#title1", Keys.Backspace);
            wait.Until(dr => dr.FindElements(By.CssSelector(".validation")).Where(el => el.Text == "Too long").Count() == 0);
            SaveButton().AssertIsEnabled();
        }

        public virtual void LocalValidationOfRegex() {
            GeminiUrl("object?i1=Edit&o1=___1.EmailAddress--12043--11238");
            ClearFieldThenType("#emailaddress11", "arthur44@adventure-works");
            wait.Until(dr => dr.FindElements(By.CssSelector(".validation")).Where(el => el.Text == "Invalid entry").Count() == 1);
            SaveButton().AssertIsDisabled().AssertHasTooltip("Invalid fields: Email Address; ");

            TypeIntoFieldWithoutClearing("#emailaddress11", ".com");
            wait.Until(dr => dr.FindElements(By.CssSelector(".validation")).Where(el => el.Text == "Invalid entry").Count() == 0);
            SaveButton().AssertIsEnabled();
        }

        public virtual void RangeValidationOnNumber() {
            GeminiUrl("object?i1=Edit&o1=___1.Product--817");
            WaitForView(Pane.Single, PaneType.Object, "Editing - HL Mountain Front Wheel");
            wait.Until(dr => dr.FindElement(By.CssSelector("#daystomanufacture1")).GetAttribute("value") == "1");
            Thread.Sleep(500);
            ClearFieldThenType("#daystomanufacture1", "0");
            wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))
                                 .Where(el => el.Text == "Value is outside the range 1 to 90").Count() == 1);
            //Confirm that the save button is disabled & has helper tooltip
            SaveButton().AssertIsDisabled().AssertHasTooltip("Invalid fields: Days To Manufacture; ");

            ClearFieldThenType("#daystomanufacture1", "1");
            wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))
                                 .Where(el => el.Text == "Value is outside the range 1 to 90").Count() == 0);
            //Confirm that the save button is disabled & has helper tooltip
            SaveButton().AssertIsEnabled();

            ClearFieldThenType("#daystomanufacture1", "91");
            wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))
                                 .Where(el => el.Text == "Value is outside the range 1 to 90").Count() == 1);
            //Confirm that the save button is disabled & has helper tooltip
            SaveButton().AssertIsDisabled().AssertHasTooltip("Invalid fields: Days To Manufacture; ");
        }

        public virtual void RangeValidationOnDate() {
            GeminiUrl("object?i1=Edit&o1=___1.Product--448");
            WaitForView(Pane.Single, PaneType.Object, "Editing - Lock Nut 13");
            string outmask = "d MMM yyyy";
            string inmask = "dd/MM/yyyy";
            var intoday = DateTime.Today.ToString(inmask);
            var outtoday = DateTime.Today.ToString(outmask);
            var inyesterday = DateTime.Today.AddDays(-1).ToString(inmask);
            var ind10 = DateTime.Today.AddDays(10).ToString(inmask);
            var outd10 = DateTime.Today.AddDays(10).ToString(outmask);
            var ind11 = DateTime.Today.AddDays(11).ToString(inmask);
            var message = $"Value is outside the range {outtoday} to {outd10}";
            ClearDateFieldThenType("#discontinueddate1", inyesterday);
            wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation")).Count == 23);
            Assert.AreEqual(message, br.FindElements(By.CssSelector(".property .validation"))[20].Text);

            //wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))[17].Text == message);
            ClearDateFieldThenType("#discontinueddate1", intoday);
            wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))[20].Text == "");
            ClearDateFieldThenType("#discontinueddate1", ind11);
            wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation")).Count == 23);
            wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))[20].Text == message);
            ClearDateFieldThenType("#discontinueddate1", ind10);
            wait.Until(dr => dr.FindElements(By.CssSelector(".property .validation"))[20].Text == "");
        }

        public virtual void ObjectEditChangeEnum() {
            GeminiUrl("object?i1=View&o1=___1.Person--6748");
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[6].Text == "Email Promotion:\r\nNo Promotions");
            EditObject();
            SelectDropDownOnField("#emailpromotion1", "Adventureworks Only");
            SaveObject();
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[6].Text == "Email Promotion:\r\nAdventureworks Only");
            EditObject();
            SelectDropDownOnField("#emailpromotion1", "No Promotions");
            SaveObject();
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[6].Text == "Email Promotion:\r\nNo Promotions");
        }

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

            ClearDateFieldThenType("#sellstartdate1", sellStart.ToString("dd/MM/yyyy"));
            ClearDateFieldThenType("#sellenddate1", sellEnd.ToString("dd/MM/yyyy"));

            ClearFieldThenType("#daystomanufacture1", "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
            Assert.AreEqual("Sell Start Date:\r\n" + sellStart.ToString("d MMM yyyy"), properties[18].Text);
            Assert.AreEqual("Sell End Date:\r\n" + sellEnd.ToString("d MMM yyyy"), properties[19].Text); //...but output format standardised.
        }

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

        public virtual void ViewModelEditOpensInEditMode() {
            GeminiUrl("object?i1=Form&r1=1&o1=___1.EmailTemplate----------New");
            WaitForCss("input#to1");
            WaitForCss("input#from1");
        }

        public virtual void MultiLineText() {
            GeminiUrl("object?o1=___1.SalesOrderHeader--44440&as1=open");
            WaitForView(Pane.Single, PaneType.Object);
            Click(GetObjectAction("Clear Comment"));

            WaitUntilElementDoesNotExist(".tempdisabled");
            var dialog = OpenActionDialog("Add Multi Line Comment");
            var field1 = WaitForCss(".parameter:nth-child(1) textarea");
            ClearFieldThenType(".parameter:nth-child(1) textarea", "comment");
            Click(OKButton());

            wait.Until(d => d.FindElement(By.CssSelector(".property .value.multiline")).Text == "comment");

            EditObject();
            var ta = WaitForCss("textarea#comment1");
            Assert.AreEqual("Free-form text", ta.GetAttribute("placeholder"));
            var rand = new Random();
            var ran1 = rand.Next(10000);
            var ran2 = rand.Next(10000);
            var ran3 = rand.Next(10000);
            ClearFieldThenType("#comment1", ran1 + Keys.Enter + ran2 + Keys.Enter + ran3);
            Click(SaveButton());

            wait.Until(d => br.FindElement(By.CssSelector(".property .value.multiline")).Text ==
                            $"{ran1}\r\n{ran2}\r\n{ran3}");
        }


        public virtual void ObjectEditChangeChoices() {
            GeminiUrl("object?o1=___1.Product--870");
            EditObject();

            // set product line 

            SelectDropDownOnField("#productline1", "S "); // need space

            ClearFieldThenType("#daystomanufacture1", "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Line:\r\nS", properties[8].Text);
        }

        public virtual void ObjectEditChangeConditionalChoices() {
            GeminiUrl("object?o1=___1.Product--870");
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
            Thread.Sleep(4000);
            // set product category and sub category
            wait.Until(d => d.FindElement(By.CssSelector("select#productcategory1")));
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
            Thread.Sleep(4000);

            wait.Until(d => d.FindElement(By.CssSelector("select#productcategory1")));
            SelectDropDownOnField("select#productcategory1", "Accessories");
            wait.Until(d => d.FindElement(By.CssSelector("select#productsubcategory1")));
            var slpsc = new SelectElement(br.FindElement(By.CssSelector("select#productsubcategory1")));
            wait.Until(d => slpsc.Options.Count == 13);

            SelectDropDownOnField("#productsubcategory1", "Bottles and Cages");
            SaveObject();

            properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nAccessories", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nBottles and Cages", properties[7].Text);
        }

        public virtual void CoValidationOnSavingChanges() {
            GeminiUrl("object?o1=___1.WorkOrder--43134&i1=Edit");
            WaitForView(Pane.Single, PaneType.Object);
            //ClearFieldThenType("input#startdate1", ""); //Seems to be necessary to clear the date fields fully
            //ClearFieldThenType("input#startdate1", "");
            ClearFieldThenType("input#startdate1", "17 Oct 2007");
            //ClearFieldThenType("input#duedate1", ""); //Seems to be necessary to clear the date fields fully
            //ClearFieldThenType("input#duedate1", "");
            ClearFieldThenType("input#duedate1", "15 Oct 2007");
            Click(SaveButton());
            WaitForMessage("StartDate must be before DueDate");
        }


    }

    public abstract class ObjectEditTests : ObjectEditTestsRoot {
        [TestMethod]
        public override void ObjectEditChangeScalar() {
            base.ObjectEditChangeScalar();
        }

        [TestMethod]
        public override void LocalValidationOfMandatoryFields() {
            base.LocalValidationOfMandatoryFields();
        }

        [TestMethod]
        public override void LocalValidationOfMaxLength() {
            base.LocalValidationOfMaxLength();
        }

        [TestMethod]
        public override void LocalValidationOfRegex() {
            base.LocalValidationOfRegex();
        }

        [TestMethod]
        public override void RangeValidationOnNumber() {
            base.RangeValidationOnNumber();
        }

        [TestMethod]
        public override void RangeValidationOnDate() {
            base.RangeValidationOnDate();
        }

        [TestMethod]
        public override void ObjectEditChangeEnum() {
            base.ObjectEditChangeEnum();
        }

        [TestMethod]
        public override void ObjectEditChangeDateTime() {
            base.ObjectEditChangeDateTime();
        }

        [TestMethod]
        public override void CanSetAndClearAnOptionalDropDown() {
            base.CanSetAndClearAnOptionalDropDown();
        }

        [TestMethod]
        public override void ObjectEditPicksUpLatestServerVersion() {
            base.ObjectEditPicksUpLatestServerVersion();
        }

        [TestMethod]
        public override void ViewModelEditOpensInEditMode() {
            base.ViewModelEditOpensInEditMode();
        }

        [TestMethod]
        public override void MultiLineText() {
            base.MultiLineText();
        }
    }

    #region browsers specific subclasses

    //[TestClass]
    public class ObjectEditPageTestsIe : ObjectEditTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    //[TestClass] //Firefox Individual
    public class ObjectEditPageTestsFirefox : ObjectEditTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    //[TestClass]
    public class ObjectEditPageTestsChrome : ObjectEditTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    #endregion

    #region Mega tests

    public abstract class MegaObjectEditTestsRoot : ObjectEditTestsRoot {
        [TestMethod] //Mega
        public void MegaObjectEditTest() {
            ObjectEditChangeScalar();
            ObjectEditCancelLeavesUnchanged();
            LocalValidationOfMandatoryFields();
            LocalValidationOfMaxLength();
            LocalValidationOfRegex();
            RangeValidationOnNumber();
            //RangeValidationOnDate();  move to LocallyRun 
            ObjectEditChangeEnum();
            //ObjectEditChangeDateTime();  move to LocallyRun 
            CanSetAndClearAnOptionalDropDown();
            ObjectEditPicksUpLatestServerVersion();
            ViewModelEditOpensInEditMode();
            MultiLineText();

            ObjectEditChangeChoices();
            ObjectEditChangeConditionalChoices();
            //CoValidationOnSavingChanges(); move to LocallyRun 
        }
    }

    //[TestClass]
    public class MegaObjectEditTestsFirefox : MegaObjectEditTestsRoot {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    //[TestClass]
    public class MegaObjectEditTestsIe : MegaObjectEditTestsRoot {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class MegaObjectEditTestsChrome : MegaObjectEditTestsRoot {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    #endregion
}