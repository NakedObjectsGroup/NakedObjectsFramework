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
    /// <summary>
    /// Tests for collection-contributedActions
    /// </summary>
    public abstract class CCAtestsRoot : AWTest {
        public virtual void ListViewWithParmDialogAlreadyOpen() {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&d1=ChangeMaxQuantity&f1_newMax=%22%22");
            WaitForView(Pane.Single, PaneType.List);
            Reload();
            var rand = new Random();
            var newMax = rand.Next(10, 999).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);

            //Now select items
            SelectCheckBox("#item1-5");
            SelectCheckBox("#item1-7");
            SelectCheckBox("#item1-9");
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            string maxQty = "Max Qty:";
            CheckIndividualItem(5, maxQty, newMax);
            CheckIndividualItem(7, maxQty, newMax);
            CheckIndividualItem(9, maxQty, newMax);
            //Confirm others have not
            CheckIndividualItem(6, maxQty, newMax, false);
            CheckIndividualItem(8, maxQty, newMax, false);
        }

        public virtual void ListViewWithParmDialogNotOpen() {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open");
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("tbody tr .checkbox")).Count >= 16);
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
            string maxQty = "Max Qty:";
            CheckIndividualItem(2, maxQty, newMax);
            CheckIndividualItem(3, maxQty, newMax);
            CheckIndividualItem(4, maxQty, newMax);
            //Confirm others have not
            CheckIndividualItem(1, maxQty, newMax, false);
            CheckIndividualItem(5, maxQty, newMax, false);
        }

        public virtual void DateParam() {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1=0&as1=open&c1=Table");
            WaitForView(Pane.Single, PaneType.List);
            Reload();
            SelectCheckBox("#item1-6");
            SelectCheckBox("#item1-9");
            OpenActionDialog("Extend Offers");
            var rand = new Random();
            var futureDate = DateTime.Today.AddDays(rand.Next(1000)).ToString("dd MMM yyyy");
            if (futureDate.StartsWith("0")) {
                futureDate = futureDate.Substring(1);
            }
            ClearFieldThenType("#todate1", futureDate + Keys.Escape);
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            Thread.Sleep(1000);
            Reload();

            //Check that exactly two rows were updated
            string endDate = "End Date:";
            CheckIndividualItem(6, endDate, futureDate);
            CheckIndividualItem(9, endDate, futureDate);
            CheckIndividualItem(7, endDate, futureDate, false);
            CheckIndividualItem(8, endDate, futureDate, false);
        }

        //To test an error that was previously being thrown by the RO server
        public virtual void EmptyParam() {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1=0&as1=open&c1=Table");
            WaitForView(Pane.Single, PaneType.List);
            Reload();
            SelectCheckBox("#item1-6");
            SelectCheckBox("#item1-9");
            OpenActionDialog("Append To Description");
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            Reload();
        }

        public virtual void TestSelectAll() {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0");
            WaitForView(Pane.Single, PaneType.List);
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("input")).Count(el => el.GetAttribute("type") == "checkbox") == 17);
            WaitForSelectedCheckboxes(0);

            //Select all
            SelectCheckBox("#all");
            WaitForSelectedCheckboxes(17);

            //Deslect all
            SelectCheckBox("#all", true);
            WaitForSelectedCheckboxes(0);
        }

        public virtual void SelectAllTableView() {
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&c1=Table");
            Reload();
            WaitForCss("td", 64);
            WaitForSelectedCheckboxes(0);

            Thread.Sleep(1000);

            //Select all
            SelectCheckBox("#all");
            WaitForSelectedCheckboxes(17);

            //Deslect all
            SelectCheckBox("#all", true);
            WaitForSelectedCheckboxes(0);
        }

        public virtual void IfNoCCAs() {
            //test that Actions is disabled & no checkboxes appear
            GeminiUrl("list?m1=PersonRepository&a1=RandomContacts&pg1=1&ps1=20&s1=0&c1=List");
            Reload();
            var actions = wait.Until(dr => dr.FindElements(By.CssSelector(".menu")).Single(el => el.Text == "Actions"));
            Assert.AreEqual("true", actions.GetAttribute("disabled"));
            actions.AssertHasTooltip("No actions available");
            var checkboxes = WaitForCss("input", 3);
            Assert.AreEqual(0, checkboxes.Count(cb => cb.Displayed));
            //Check that actions menu is disabled and 
        }

        public virtual void SelectionRetainedWhenNavigatingAwayAndBack() {
            GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=1&ps1=20&s1=152&c1=List");
            Reload();
            WaitForSelectedCheckboxes(3);
            Click(HomeIcon());
            WaitForView(Pane.Single, PaneType.Home);
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.List);
            WaitForSelectedCheckboxes(3);
        }

        public virtual void SelectionClearedWhenPageChanged() {
            GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=1&ps1=20&s1=152&c1=List");
            Reload();
            WaitForTextStarting(".details", "Page 1 of ");
            WaitForSelectedCheckboxes(3);
            var row = WaitForCss(".reference"); //first one
            RightClick(row);
            WaitForView(Pane.Right, PaneType.Object);
            WaitForSelectedCheckboxes(3);
            Click(GetButton("Next", Pane.Left));
            WaitForTextStarting(".details", "Page 2 of ");
            WaitForSelectedCheckboxes(0);
            //Using back button retrieves original selection
            ClickBackButton();
            WaitForTextStarting(".details", "Page 1 of ");
            WaitForSelectedCheckboxes(3);
            //But going Next then Previous loses it
            Click(GetButton("Next", Pane.Left));
            WaitForTextStarting(".details", "Page 2 of ");
            WaitForSelectedCheckboxes(0);
            Click(GetButton("Previous", Pane.Left));
            WaitForTextStarting(".details", "Page 1 of ");
            WaitForSelectedCheckboxes(0);
        }
    }

    public abstract class CCAtestsServer : CCAtestsRoot {
        [TestMethod]
        public override void ListViewWithParmDialogAlreadyOpen() {
            base.ListViewWithParmDialogAlreadyOpen();
        }

        [TestMethod]
        public override void ListViewWithParmDialogNotOpen() {
            base.ListViewWithParmDialogNotOpen();
        }

        //[TestMethod] fails on IE TODO
        public override void DateParam() {
            base.DateParam();
        }

        [TestMethod]
        public override void EmptyParam() {
            base.EmptyParam();
        }

        [TestMethod]
        public override void TestSelectAll() {
            base.TestSelectAll();
        }

        [TestMethod]
        public override void SelectAllTableView() {
            base.SelectAllTableView();
        }

        [TestMethod]
        public override void IfNoCCAs() {
            base.IfNoCCAs();
        }

        [TestMethod]
        public override void SelectionRetainedWhenNavigatingAwayAndBack() {
            base.SelectionRetainedWhenNavigatingAwayAndBack();
        }

        [TestMethod]
        public override void SelectionClearedWhenPageChanged() {
            base.SelectionClearedWhenPageChanged();
        }
    }

    #region browsers specific subclasses

    //[TestClass]
    public class CCAtestsIe : CCAtestsServer {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    //[TestClass]
    public class CCAtestsFirefox : CCAtestsServer {
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
    }

    //[TestClass]
    public class CCAtestsChrome : CCAtestsServer {
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

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}