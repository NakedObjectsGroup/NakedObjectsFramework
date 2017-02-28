// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {

   


    public abstract class MultiLineDialogTestsRoot : AWTest {
        protected void PageDownAndWait() {
            br.SwitchTo().ActiveElement().SendKeys(Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown);
            Thread.Sleep(1000);
        }

        public virtual void MultiLineMenuAction() {
            GeminiUrl("home?m1=SpecialOfferRepository");
            Click(GetObjectAction("Create Multiple Special Offers"));
            WaitForView(Pane.Single, PaneType.MultiLineDialog, "Create Multiple Special Offers");
            WaitForTextEquals(".count", "with 0 lines submitted.");
            //Enter line 0
            OKButtonOnLine(0).AssertIsDisabled("Missing mandatory fields: Description; Discount Pct; Type; Category; Min Qty; Start Date; ");
            ClearFieldThenType("#description0", "x");
            ClearFieldThenType("#discountpct0", ".03");
            ClearFieldThenType("#type0", "Promotion");
            SelectDropDownOnField("#category0", "Reseller");
            ClearFieldThenType("#minqty0", "10");
            ClearFieldThenType("#startdate0", "01/01/2002");
            OKButtonOnLine(0).AssertIsEnabled();
            //Submit line 0
            Click(OKButtonOnLine(0));
            WaitForTextEquals(".co-validation", 0, "Submitted");
            WaitForTextEquals(".count", "with 1 lines submitted.");
            //Check the read-only fields
            WaitForOKButtonToDisappear(0);
            WaitUntilElementDoesNotExist("#description0");
            WaitForReadOnlyEnteredParam(0, 0, "x");
            WaitForReadOnlyEnteredParam(0, 1, "0.03");
            WaitForReadOnlyEnteredParam(0, 2, "Promotion");
            WaitForReadOnlyEnteredParam(0, 3, "Reseller");
            WaitForReadOnlyEnteredParam(0, 4, "10");
            WaitForReadOnlyEnteredParam(0, 5, "2002-01-01");

            //line 1
            OKButtonOnLine(1).AssertIsDisabled("Missing mandatory fields: Description; Discount Pct; Type; Category; Min Qty; Start Date; ");
            var rand = new Random();
            var description = rand.Next(9999).ToString();
            ClearFieldThenType("#description1", description);
            ClearFieldThenType("#discountpct1", ".04");
            ClearFieldThenType("#type1", "Promotion");
            SelectDropDownOnField("#category1", "Reseller");
            ClearFieldThenType("#minqty1", "5");
            ClearFieldThenType("#startdate1", "01/01/2002");
            PageDownAndWait();
            OKButtonOnLine(1).AssertIsEnabled();
            Click(OKButtonOnLine(1));
            WaitForTextEquals(".co-validation", 1, "Submitted");
            WaitForTextEquals(".count", "with 2 lines submitted.");

            WaitForOKButtonToDisappear(1);
            WaitUntilElementDoesNotExist("#description1");

            //Check that new empty line (2) has been created
            WaitForCss("#description2");
            OKButtonOnLine(2).AssertIsDisabled("Missing mandatory fields: Description; Discount Pct; Type; Category; Min Qty; Start Date; ");

            PageDownAndWait();
            //Close the MLD
            Click(WaitForCss(".close"));
            WaitForView(Pane.Single, PaneType.Home);

            //Check that both special offers were in fact created
            GeminiUrl("list?m1=SpecialOfferRepository&a1=AllSpecialOffers&pg1=1&ps1=20&s1_=0&c1=List");
            Reload();
            WaitForView(Pane.Single, PaneType.List);
            WaitForCss(".reference", 10);
            var row1 = WaitForCssNo(".reference", 0);
            Assert.AreEqual(description, row1.Text);
        }

        public virtual void MultiLineObjectAction() {
            GeminiUrl("object?i1=View&r=1&o1=___1.Vendor--1504&as1=open");
            OpenSubMenu("Purchase Orders");
            Click(GetObjectAction("Create New Purchase Order"));
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Purchase Order Header");
            SaveObject();
            OpenObjectActions();
            Click(GetObjectAction("Add New Details"));
            WaitForView(Pane.Single, PaneType.MultiLineDialog);

            //Line 0
            ClearFieldThenType("#prod0", "Dissolver");
            wait.Until(d => d.FindElements(By.CssSelector(".suggestions a")).Count > 0);
            Click(WaitForCss(".suggestions a"));
            WaitForCss("#prod0.link-color4");
            ClearFieldThenType("#qty0", "4");
            ClearFieldThenType("#unitprice0", "2.54");
            Click(OKButtonOnLine(0));
            WaitForOKButtonToDisappear(0);

            //line 1
            ClearFieldThenType("#prod1", "bottle");
            wait.Until(d => d.FindElements(By.CssSelector(".suggestions a")).Count > 0);
            Click(WaitForCss(".suggestions a"));
            WaitForCss("#prod1.link-color4");
            ClearFieldThenType("#qty1", "5");
            ClearFieldThenType("#unitprice1", "2.54");
            Click(OKButtonOnLine(1));
            WaitForOKButtonToDisappear(1);

            PageDownAndWait();
            //close
            Click(WaitForCss(".close"));
            WaitForView(Pane.Single, PaneType.Object);
            WaitForTextEquals(".summary .details", 0, "2 Items");
        }

        public virtual void MultiLineObjectActionInCollection() {
            GeminiUrl("object?i1=View&r=1&o1=___1.Customer--29562&as1=open&d1=CreateNewOrder");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Sales Order");
            SaveObject();
            GetInputButton("Edit"); //Waiting for save
            WaitForTextEquals(".summary .details", 0, "Empty");

            var iconList = WaitForCssNo(".collection .icon-list", 0);
            Click(iconList);
            WaitForCss("table");

            var selector = ".collection .actions nof-action";
            var action = wait.Until(d => d.FindElements(By.CssSelector(selector)).
                Single(we => we.Text == "Add New Details"));
            Click(action);

            WaitForView(Pane.Single, PaneType.MultiLineDialog);
            wait.Until(dr => dr.FindElement(By.CssSelector(".title")).Text.Contains("Add New Details"));
            WaitForCss(".lineDialog", 6);
            WaitForTextEquals(".count", "with 0 lines submitted.");

            //line0
            OKButtonOnLine(0).AssertIsDisabled("Missing mandatory fields: Product; ");
            //Auto-complete
            ClearFieldThenType("#product0", "Dissolver");
            wait.Until(d => d.FindElements(By.CssSelector(".suggestions a")).Count > 0);
            //As the match has not yet been selected,the field is invalid, so...
            WaitForTextEquals(".validation", 0, "Pending auto-complete...");
            OKButtonOnLine(0).AssertIsDisabled().AssertHasTooltip("Invalid fields: Product; ");
            Click(WaitForCss(".suggestions a"));
            WaitForCss("#product0.link-color4");
            //Other field with validation
            ClearFieldThenType("#quantity0", "0");
            OKButtonOnLine(0).AssertIsEnabled();
            Click(OKButtonOnLine(0));
            WaitForTextEquals(".co-validation", 0, "See field validation message(s).");
            WaitForTextEquals(".validation", 1, "Must be > 0");
            ClearFieldThenType("#quantity0", "3");
            OKButtonOnLine(0).AssertIsEnabled();
            Click(OKButtonOnLine(0));
            //redo
            WaitForTextEquals(".co-validation", 0, "Submitted");
            WaitForTextEquals(".count", "with 1 lines submitted.");
            WaitUntilElementDoesNotExist("#description0");

            //line1
            OKButtonOnLine(1).AssertIsDisabled("Missing mandatory fields: Product; ");
            //Auto-complete
            ClearFieldThenType("#product1", "vest, S");
            wait.Until(d => d.FindElements(By.CssSelector(".suggestions a")).Count > 0);
            Click(WaitForCss(".suggestions a"));
            WaitForCss("#product1.link-color4");
            Click(OKButtonOnLine(1));
            WaitForTextEquals(".co-validation", 1, "Submitted");
            WaitForTextEquals(".count", "with 2 lines submitted.");
            WaitUntilElementDoesNotExist("#product1");

            WaitForCss("#product2");
            OKButtonOnLine(2);

            PageDownAndWait();
            //Close
            Click(WaitForCss(".close"));
            WaitForTextStarting(".title", "SO"); //back to order
            WaitForTextEquals(".summary .details", 0, "2 Items");
        }

        //#53
        public virtual void InvokeMLDFromObjectInRightPane()
        {
            GeminiUrl("home/object?i2=View&r=1&o2=___1.PurchaseOrderHeader--300&as2=open");
            Click(GetObjectAction("Add New Details", Pane.Right));
            WaitForView(Pane.Single, PaneType.MultiLineDialog);
            SwapIcon().AssertIsDisabled();
        }
    }

    public abstract class MultiLineDialogsTests : MultiLineDialogTestsRoot {
        [TestMethod]
        public override void MultiLineMenuAction() {
            base.MultiLineMenuAction();
        }

        [TestMethod]
        public override void MultiLineObjectAction() {
            base.MultiLineObjectAction();
        }

        [TestMethod]
        public override void MultiLineObjectActionInCollection() {
            base.MultiLineObjectActionInCollection();
        }
        [TestMethod]
        public override void InvokeMLDFromObjectInRightPane()
        {
            base.InvokeMLDFromObjectInRightPane();
        }
    }

    #region browsers specific subclasses

    public class MultiLineDialogTestsIe : MultiLineDialogsTests {
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
    public class MultiLineDialogFirefox : MultiLineDialogsTests {
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

    public class MultiLineDialogTestsChrome : MultiLineDialogsTests {
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

    public abstract class MegaMultiLineDialogTestsRoot : MultiLineDialogTestsRoot {
        [TestMethod] //Mega
        public void MegaMultiLineDialogTest() {
            MultiLineMenuAction();
            MultiLineObjectAction();
            // MultiLineObjectActionInCollection();  move to LocallyRun 
            InvokeMLDFromObjectInRightPane();
        }
    }

    //[TestClass]
    public class MegaMultiLineDialogTestsFirefox : MegaMultiLineDialogTestsRoot {
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

    public class MegaMultiLineDialogTestsIe : MegaMultiLineDialogTestsRoot {
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
    public class MegaMultiLineDialogTestsChrome : MegaMultiLineDialogTestsRoot {
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