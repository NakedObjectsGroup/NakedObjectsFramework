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
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Selenium {
    /// <summary>
    /// These are all tests that should pass when run locally, but are unreliable on the server
    /// </summary>
    public abstract class LocallyRunTestsRoot : AWTest {
        #region List

        public virtual void PagingTableView() {
            GeminiUrl("list?m1=CustomerRepository&a1=FindIndividualCustomerByName&p1=1&ps1=20&pm1_firstName=%22%22&pm1_lastName=%22a%22&c1=Table");
            Reload();
            //Confirm in Table view
            WaitForCss("thead tr th");
            WaitForCss(".icon-list");
            WaitUntilElementDoesNotExist(".icon-table");
            //Test content of collection
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                .Text.StartsWith("Page 1 of"));
            GetButton("First").AssertIsDisabled();
            GetButton("Previous").AssertIsDisabled();
            var next = GetButton("Next").AssertIsEnabled();
            GetButton("Last").AssertIsEnabled();
            //Go to next page
            Click(next);
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                .Text.StartsWith("Page 2 of"));
            //Confirm in Table view
            WaitForCss("thead tr th");
            WaitForCss(".icon-list");
            WaitUntilElementDoesNotExist(".icon-table");

            GetButton("First").AssertIsEnabled();
            GetButton("Previous").AssertIsEnabled();
            GetButton("Next").AssertIsEnabled();
            var last = GetButton("Last").AssertIsEnabled();
            Click(last);
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                .Text.StartsWith("Page 45 of 45"));
            //Confirm in Table view
            WaitForCss("thead tr th");
            var iconList = WaitForCss(".icon-list");
            WaitUntilElementDoesNotExist(".icon-table");

            GetButton("First").AssertIsEnabled();
            var prev = GetButton("Previous").AssertIsEnabled();
            GetButton("Next").AssertIsDisabled();
            GetButton("Last").AssertIsDisabled();
            Click(prev);
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                .Text.StartsWith("Page 44 of 45"));
            //Confirm in Table view
            WaitForCss("thead tr th");
            WaitForCss(".icon-list");
            WaitUntilElementDoesNotExist(".icon-table");
            var first = GetButton("First").AssertIsEnabled();
            GetButton("Previous").AssertIsEnabled();
            GetButton("Next").AssertIsEnabled();
            GetButton("Last").AssertIsEnabled();
            Click(first);
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                .Text.StartsWith("Page 1 of 45"));
            //Confirm in Table view
            WaitForCss("thead tr th");
            WaitForCss(".icon-list");
            WaitUntilElementDoesNotExist(".icon-table");
        }

        #endregion

        #region CCAs
        public virtual void TableViewWithParmDialogNotOpen()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&c1=Table");
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
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&c1=Table");
            WaitForView(Pane.Single, PaneType.List);
            Reload();
            WaitForCss("td", 64);
            OpenActionDialog("Change Discount");
            TypeIntoFieldWithoutClearing("#newdiscount1", "0.10");
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            SelectCheckBox("#item1-4");
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
        }

        public virtual void TableViewWithParmDialogAlreadyOpen() {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&c1=Table&as1=open&d1=ChangeDiscount");
            Reload();
            var rand = new Random();
            var newPct = "0." + rand.Next(51, 59);
            TypeIntoFieldWithoutClearing("#newdiscount1", newPct);
            WaitForCss("td", 64);
            //Now select items
            SelectCheckBox("#item1-6");
            SelectCheckBox("#item1-8");
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            CheckIndividualItem(6, "Discount Pct:", newPct);
            CheckIndividualItem(7, "Discount Pct:", newPct, false);
            CheckIndividualItem(8, "Discount Pct:", newPct);

            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            //Reset to below 50%
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&c1=Table&as1=open&d1=ChangeDiscount");
            Reload();
            TypeIntoFieldWithoutClearing("#newdiscount1", "0.10");
            var cells = WaitForCss("td", 64);
            SelectCheckBox("#item1-6");
            SelectCheckBox("#item1-8");
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
        }

        public virtual void ReloadingAQueryableClearsSelection()
        {
            GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=20&ps1=5&s1=0&as1=open");
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("td")).Count > 30);
            SelectCheckBox("#item1-4");
            SelectCheckBox("#item1-7");
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("input")).Count(el => el.GetAttribute("type") == "checkbox") == 0);
        }

        public virtual void ZeroParamAction() {
            GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=20&ps1=5&s1=0&as1=open&c1=Table");
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("td")).Count > 30);

            SelectCheckBox("#all"); //To clear
            Click(GetObjectAction("Clear Comments"));
            Thread.Sleep(1000);
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text.Contains("User unhappy")) == 0);

            SelectCheckBox("#all", true); //To clear
            WaitForSelectedCheckboxes(0);

            //Now add comments
            SelectCheckBox("#item1-1");
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            Click(GetObjectAction("Comment As Users Unhappy"));
            Thread.Sleep(1000); //Because there is no visible change to wait for
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text.Contains("User unhappy")) == 3);

            //Confirm that the three checkboxes have now been cleared
            wait.Until(dr => dr.FindElements(By.CssSelector("input")).Count(el => el.GetAttribute("type")=="checkbox") == 0);

            SelectCheckBox("#all"); //To clear
            Click(GetObjectAction("Clear Comments"));
            Thread.Sleep(1000);
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text.Contains("User unhappy")) == 0);
        }

        #endregion

        #region Copy & Paste

        public virtual void IfNoObjectInClipboardCtrlVRevertsToBrowserBehaviour() {
            GeminiUrl("home?m1=EmployeeRepository&d1=CreateNewEmployeeFromContact&f1_contactDetails=null");
            WaitForView(Pane.Single, PaneType.Home);
            var home = WaitForCss(".title");
            Actions action = new Actions(br);
            action.DoubleClick(home); //Should put "Home"into browser clipboard
            action.Perform();
            Thread.Sleep(500);
            home.SendKeys(Keys.Control + "c");
            string selector = "input.value";
            var target = WaitForCss(selector);
            Assert.AreEqual("", target.GetAttribute("value"));
            target.Click();
            target.SendKeys(Keys.Control + "v");
            Assert.AreEqual("Home", target.GetAttribute("value"));
        }

        public virtual void CanClearADroppableReferenceField() {
            GeminiUrl("object?o1=___1.PurchaseOrderHeader--561&i1=Edit");
            WaitForView(Pane.Single, PaneType.Object);
            var fieldCss = ".property:nth-child(4) .value.droppable";
            var field = WaitForCss(fieldCss);
            Assert.AreEqual("Ben Miller", field.Text);
            Thread.Sleep(100);
            field.SendKeys(Keys.Delete);
            wait.Until(dr => dr.FindElement(By.CssSelector(fieldCss)).Text == "* (drop here)");
        }

        #endregion

        #region Object Edit

        public virtual void ObjectEditChangeChoices() {
            GeminiUrl("object?o1=___1.Product--870");
            EditObject();

            // set product line 

            SelectDropDownOnField("#productline1", "S");

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

        #endregion

        #region Keyboard navigation
        public virtual void SelectFooterIconsWithAccessKeys()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            WaitForCss(".header .title").Click();
            var element = br.SwitchTo().ActiveElement();

            element.SendKeys(Keys.Alt + "h");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Home (Alt-h)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "b");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Back (Alt-b)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "f");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Forward (Alt-f)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "e");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Expand pane (Alt-e)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "s");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Swap panes (Alt-s)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "r");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Recent object (Alt-r)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "c");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Cicero - Speech Interface (Alt-c)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "p");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Application Properties (Alt-p)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "l");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Log off (Alt-l)", element.GetAttribute("title"));
        }

        public virtual void SelectObjectActionsWithAccessKey()
        {
            GeminiUrl("object?i1=View&o1=___1.Person--15748");
            WaitForView(Pane.Single, PaneType.Object);
            var element = br.SwitchTo().ActiveElement();
            element.SendKeys(Keys.Alt + "a");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Open actions (Alt-a)", element.GetAttribute("title"));
        }

        #endregion

        #region Cicero
        //Note: Requires the Cicero icon to be made visible
        public virtual void LaunchCiceroFromIcon()
        {
            GeminiUrl("object?o1=___1.Product--968");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            Click(WaitForCss(".icon-speech"));
            WaitForOutput("Product: Touring-1000 Blue, 54"); //Cicero
            GeminiUrl("object/list?o1=___1.Store--350&m2=OrderRepository&a2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            Click(WaitForCss(".icon-speech"));
            WaitForOutput("Store: Twin Cycles"); //Cicero

            GeminiUrl("object?o1=___1.Product--968&as1=open&d1=BestSpecialOffer&f1_quantity=%22%22");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            WaitForCss("#quantity1"); //i.e. dialog open
            Click(WaitForCss(".icon-speech"));
            WaitForOutput("Product: Touring-1000 Blue, 54\r\nAction dialog: Best Special Offer\r\nQuantity: empty");
        }
        #endregion
    }

    public abstract class LocallyRunTests : LocallyRunTestsRoot {
        #region List tests

        [TestMethod]
        public override void PagingTableView() {
            base.PagingTableView();
        }

        #endregion

        #region CCA Tests
        [TestMethod]
        public override void TableViewWithParmDialogNotOpen()
        {
            base.TableViewWithParmDialogNotOpen();
        }

        [TestMethod]
        public override void TableViewWithParmDialogAlreadyOpen() {
            base.TableViewWithParmDialogAlreadyOpen();
        }
        [TestMethod]
        public override void ReloadingAQueryableClearsSelection()
        {
            base.ReloadingAQueryableClearsSelection();
        }

        [TestMethod]
        public override void ZeroParamAction() {
            base.ZeroParamAction();
        }

        #endregion

        #region Copy & Paste

        [TestMethod]
        public override void CanClearADroppableReferenceField() {
            base.CanClearADroppableReferenceField();
        }

        [TestMethod]
        public override void IfNoObjectInClipboardCtrlVRevertsToBrowserBehaviour() {
            base.IfNoObjectInClipboardCtrlVRevertsToBrowserBehaviour();
        }

        #endregion

        #region Object Edit

        [TestMethod]
        public override void ObjectEditChangeChoices() {
            base.ObjectEditChangeChoices();
        }

        [TestMethod]
        public override void ObjectEditChangeConditionalChoices() {
            base.ObjectEditChangeConditionalChoices();
        }

        [TestMethod]
        public override void CoValidationOnSavingChanges() {
            base.CoValidationOnSavingChanges();
        }

        #endregion

        #region Keyboard navigation
        [TestMethod] //Doesn't work with Firefox?
        public override void SelectFooterIconsWithAccessKeys() { base.SelectFooterIconsWithAccessKeys(); }

        [TestMethod] //Doesn't work with Firefox?
        public override void SelectObjectActionsWithAccessKey() { base.SelectObjectActionsWithAccessKey(); }

        #endregion

        #region Cicero
        //Note: Requires the Cicero icon to be made visible
        public override void LaunchCiceroFromIcon()
        {
            base.LaunchCiceroFromIcon();
         }
        #endregion
    }

    #region browsers specific subclasses 

    //[TestClass, Ignore] //IE Individual
    public class LocallyRunTestsIe : LocallyRunTests {
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

    //[TestClass, Ignore] //Firefox Individual
    public class LocallyRunTestsFirefox : LocallyRunTests {
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

    //[TestClass, Ignore] //Chrome Individual
    public class LocallyRunTestsChrome : LocallyRunTests {
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

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}