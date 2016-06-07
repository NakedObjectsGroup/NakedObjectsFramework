// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace NakedObjects.Web.UnitTests.Selenium
{

    /// <summary>
    /// Tests for collection-contributedActions
    /// </summary>
    public abstract class CCAtestsRoot : AWTest
    {
        public virtual void ListViewWithParmDialogAlreadyOpen()
        {
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

        public virtual void ListViewWithParmDialogNotOpen()
        {
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
            CheckIndividualItem(3, maxQty,newMax);
            CheckIndividualItem(4, maxQty,newMax);
            //Confirm others have not
            CheckIndividualItem(1, maxQty,newMax, false);
            CheckIndividualItem(5, maxQty,newMax, false);
        }

        public virtual void TableViewWithParmDialogAlreadyOpen()
        {
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

        public virtual void DateParam()
        {
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
            if (futureDate.StartsWith("0"))
            {
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
        public virtual void EmptyParam()
        {
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

        public virtual void ZeroParamAction()
        {
            GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=20&ps1=5&s1=0&as1=open&c1=Table");
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("td")).Count >30);
            //Wait for no checkboxes selected
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
            wait.Until( dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text.Contains("User unhappy")) ==3);
            //Confirm three checkboxes still selected:
            SelectCheckBox("#all"); //To clear
            Click(GetObjectAction("Clear Comments"));
            Thread.Sleep(1000);
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text.Contains("User unhappy")) == 0);
        }

        public virtual void TestSelectAll()
        {
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

        public virtual void SelectAllTableView()
        {
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

        public virtual void IfNoCCAs()
        {
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

        public virtual void NoAllIfNoResults()
        {
            GeminiUrl("list?m1=CustomerRepository&pm1_firstName=%22%22&pm1_lastName=%22zz%22&a1=FindIndividualCustomerByName&pg1=1&ps1=20&s1=0");
            Reload();
            WaitForCss("#all").AssertIsInvisible();
        }

        private void WaitForSelectedCheckboxes(int number)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector("input")).Count(el => el.GetAttribute("type") == "checkbox" && el.Selected) == number);
        }

        public virtual void SelectionRetainedWhenNavigatingAwayAndBack()
        {
            GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=1&ps1=20&s1=152&c1=List");
            Reload();
            WaitForSelectedCheckboxes(3);
            Click(HomeIcon());
            WaitForView(Pane.Single, PaneType.Home);
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.List);
            WaitForSelectedCheckboxes(3);
        }

        public virtual void SelectionClearedWhenPageChanged()
        {
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

        #region Helpers
        private void CheckIndividualItem(int itemNo, string label, string value, bool equal = true)
        {
            GeminiUrl("object?o1=___1.SpecialOffer--" + (itemNo + 1));
            var html = label + "\r\n" + value;
            if (equal)
            {
                wait.Until( dr => dr.FindElements(By.CssSelector(".property")).First(p => p.Text.StartsWith(label)).Text == html);
            }
            else
            {
                wait.Until(dr => dr.FindElements(By.CssSelector(".property")).First(p => p.Text.StartsWith(label)).Text != html);
            }
        }
        #endregion
    }

    public abstract class CCAtests : CCAtestsRoot
    {

        [TestMethod]
        public override void ListViewWithParmDialogAlreadyOpen() { base.ListViewWithParmDialogAlreadyOpen(); }

        [TestMethod]
        public override void ListViewWithParmDialogNotOpen() { base.ListViewWithParmDialogNotOpen(); }

        [TestMethod] //Unreliable on server
        public override void TableViewWithParmDialogAlreadyOpen() { base.TableViewWithParmDialogAlreadyOpen(); }

        [TestMethod]
        public override void TableViewWithParmDialogNotOpen() { base.TableViewWithParmDialogNotOpen(); }

        [TestMethod]
        public override void DateParam() { base.DateParam(); }
        [TestMethod]
        public override void EmptyParam() { base.EmptyParam(); }


        [TestMethod] //Unreliable on server
        public override void ZeroParamAction() { base.ZeroParamAction(); }

        [TestMethod]
        public override void TestSelectAll() { base.TestSelectAll(); }

        [TestMethod]
        public override void SelectAllTableView() { base.SelectAllTableView(); }

        [TestMethod]
        public override void IfNoCCAs() { base.IfNoCCAs(); }
        [TestMethod]
        public override void NoAllIfNoResults() { base.NoAllIfNoResults(); }
        [TestMethod]
        public override void SelectionRetainedWhenNavigatingAwayAndBack()
        {
            base.SelectionRetainedWhenNavigatingAwayAndBack();
        }
        [TestMethod]
        public override void SelectionClearedWhenPageChanged()
        {
            base.SelectionClearedWhenPageChanged();
        }
    }

    #region browsers specific subclasses

    public class CCAtestsIe : CCAtests
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
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class CCAtestsFirefox : CCAtests
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
    }

    public class CCAtestsChrome : CCAtests
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

        protected override void ScrollTo(IWebElement element)
        {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }
    }

    #endregion
}