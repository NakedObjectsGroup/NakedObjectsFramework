// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Threading;

namespace NakedObjects.Web.UnitTests.Selenium {

    /// <summary>
    /// Tests applied from a List view.
    /// </summary>
    public abstract class ListTests : AWTest {
        [TestMethod]
        public virtual void ActionReturnsListView()
        {
            Url(OrdersMenuUrl);
            Click(GetObjectAction("Highest Value Orders"));
            WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
            //Test content of collection
            Assert.AreEqual("Page 1 of 1574; viewing 20 of 31465 items", WaitForCss(".collection .summary .details").Text);
            WaitForCss(".icon-table");
            WaitUntilElementDoesNotExist(".icon-list");
            WaitUntilElementDoesNotExist(".icon-summary");
            Assert.AreEqual(20, br.FindElements(By.CssSelector(".collection table tbody tr td.reference")).Count);
            Assert.AreEqual(20, br.FindElements(By.CssSelector(".collection table tbody tr td.checkbox")).Count);
            Assert.AreEqual(0, br.FindElements(By.CssSelector(".cell")).Count); //Cells are in Table view only

        }

        [TestMethod]
        public virtual void SwitchToTableViewAndBackToList()
        {
            Url(SpecialOffersMenuUrl);
            Click(GetObjectAction("Current Special Offers"));
            WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
            wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
            var iconTable = WaitForCss(".icon-table");
            Click(iconTable);
            
            var iconList = WaitForCss(".icon-list");
            WaitUntilElementDoesNotExist(".icon-table");
            WaitUntilElementDoesNotExist(".icon-summary");

            wait.Until(dr => dr.FindElements(By.CssSelector(".collection table tbody tr")).Count > 1);
 
            //Switch back to List view
            Click(iconList);
            WaitForCss(".icon-table");
            WaitUntilElementDoesNotExist(".icon-list");
            WaitUntilElementDoesNotExist(".icon-summary");
            wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
            Assert.AreEqual(0, br.FindElements(By.CssSelector(".cell")).Count); //Cells are in Table view only
        }

        [TestMethod]
        public virtual void NavigateToItemFromListView()
        {
            Url(SpecialOffersMenuUrl);
            Click(GetObjectAction("Current Special Offers"));
            WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
            var row = WaitForCss(".reference");
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, "No Discount");
        }

        [TestMethod]
        public virtual void NavigateToItemFromTableView()
        {
            Url(SpecialOffersMenuUrl);
            Click(GetObjectAction("Current Special Offers"));
            WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
            wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
            var iconTable = WaitForCss(".icon-table");
            Click(iconTable);
            wait.Until(dr => dr.FindElements(By.CssSelector("table tbody tr")).Count > 1);
            var row = wait.Until(dr => dr.FindElement(By.CssSelector("table tbody tr")));
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, "No Discount");
        }

        [TestMethod]
        public virtual void Paging()
        {
            GeminiUrl("list?menu1=CustomerRepository&action1=FindIndividualCustomerByName&page1=1&pageSize1=20&parm1_firstName=%2522%2522&parm1_lastName=%2522a%2522");
            Reload();

            //Test content of collection
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                .Text.StartsWith("Page 1 of 45"));
            GetButton("First").AssertIsDisabled();
            GetButton("Previous").AssertIsDisabled();
            var next = GetButton("Next").AssertIsEnabled();
            GetButton("Last").AssertIsEnabled();
            //Go to next page
            Click(next);
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                 .Text.StartsWith("Page 2 of 45"));
            GetButton("First").AssertIsEnabled();
            GetButton("Previous").AssertIsEnabled();
            GetButton("Next").AssertIsEnabled();
            var last = GetButton("Last").AssertIsEnabled();
            Click(last);
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                    .Text.StartsWith("Page 45 of 45"));
            GetButton("First").AssertIsEnabled();
            var prev = GetButton("Previous").AssertIsEnabled();
            GetButton("Next").AssertIsDisabled();
            GetButton("Last").AssertIsDisabled();
            Click(prev);
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                .Text.StartsWith("Page 44 of 45"));
            var first = GetButton("First").AssertIsEnabled();
            GetButton("Previous").AssertIsEnabled();
            GetButton("Next").AssertIsEnabled();
            GetButton("Last").AssertIsEnabled();
            Click(first);
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                 .Text.StartsWith("Page 1 of 45"));
        }

        [TestMethod]
        public virtual void PagingTableView()
        {
            GeminiUrl("list?menu1=CustomerRepository&action1=FindIndividualCustomerByName&page1=1&pageSize1=20&parm1_firstName=%2522%2522&parm1_lastName=%2522a%2522&collection1=Table");
            Reload();
            //Confirm in Table view
            WaitForCss("thead tr th"); 
            WaitForCss(".icon-list");
            WaitUntilElementDoesNotExist(".icon-table");
            //Test content of collection
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                .Text.StartsWith("Page 1 of 45"));
            GetButton("First").AssertIsDisabled();
            GetButton("Previous").AssertIsDisabled();
            var next = GetButton("Next").AssertIsEnabled();
            GetButton("Last").AssertIsEnabled();
            //Go to next page
            Click(next);
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details"))
                 .Text.StartsWith("Page 2 of 45"));
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

        [TestMethod]
        public void ListDoesNotRefreshWithoutReload()
        {
            GeminiUrl("list?menu1=SpecialOfferRepository&action1=SpecialOffersWithNoMinimumQty&page1=1&pageSize1=20");
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details")).Text
                .StartsWith("Page 1 of 1;"));
            GeminiUrl("object?object1=AdventureWorksModel.SpecialOffer-7");
            EditObject();
            ClearFieldThenType("#minqty1", "10");
            SaveObject();
            GeminiUrl("list?menu1=SpecialOfferRepository&action1=SpecialOffersWithNoMinimumQty&page1=1&pageSize1=20");
            WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details")).Text
                == "Page 1 of 1; viewing 11 of 11 items");
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details")).Text 
                == "Page 1 of 1; viewing 10 of 10 items");

            //Undo to leave in original state
            GeminiUrl("object?object1=AdventureWorksModel.SpecialOffer-7");
            EditObject();
            ClearFieldThenType("#minqty1", Keys.Backspace + Keys.Backspace + "0");
            SaveObject();
            GeminiUrl("list?menu1=SpecialOfferRepository&action1=SpecialOffersWithNoMinimumQty&page1=1&pageSize1=20");
            WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details")).Text
                == "Page 1 of 1; viewing 11 of 11 items");
        }

        [TestMethod, Ignore] //Initially pending fix by stef
        public void CCA1()
        {
            GeminiUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0&actions1=open");
            Reload();
            WaitForView(Pane.Single, PaneType.List);
            SelectCheckBox("#item1-0");
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-4");
            OpenActionDialog("Extend Offers");
            var date = new DateTime(2029, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dateStr = date.ToString("d MMM yyyy");
            TypeIntoFieldWithoutClearing("#todate1", dateStr);
            TypeIntoFieldWithoutClearing("#todate1", Keys.Escape); //To lose datepicker

            Thread.Sleep(300); //TODO: not good, but can't find any other way to detect that datepicker is no longer overlapping OK
            Click(OKButton());

        }

        [TestMethod, Ignore]
        public void CCAWithDialogNotOpen()
        {
            GeminiUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0&actions1=open");
            Reload();
            WaitForView(Pane.Single, PaneType.List);
            SelectCheckBox("#item1-0");
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-4");
            OpenActionDialog("Change Type");

        }


    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class ListTestsIe : ListTests
    {
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

    [TestClass]
    public class ListTestsFirefox : ListTests
    {
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

    //[TestClass, Ignore]
    public class ListTestsChrome : ListTests {
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