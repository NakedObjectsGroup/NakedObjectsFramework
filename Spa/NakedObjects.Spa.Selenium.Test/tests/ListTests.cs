// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Linq;

namespace NakedObjects.Web.UnitTests.Selenium
{
    /// <summary>
    /// Tests applied from a List view.
    /// </summary>
    public abstract class ListTestsRoot : AWTest
    {
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
        public virtual void TableViewAttributeHonoured()
        {
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1=0&c1=Table");
            Reload();
            var header = WaitForCss("thead");
            var cols = header.FindElements(By.CssSelector("th")).ToArray();
            Assert.AreEqual(4, cols.Length);
            Assert.AreEqual("Select", cols[0].Text);
            Assert.AreEqual("Description", cols[1].Text);
            Assert.AreEqual("Category", cols[2].Text);
            Assert.AreEqual("Discount Pct", cols[3].Text);
        }
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
        public virtual void NavigateToItemFromListView()
        {
            Url(SpecialOffersMenuUrl);
            Click(GetObjectAction("Current Special Offers"));
            WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
            var row = WaitForCss(".reference");
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, "No Discount");
        }
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
        public virtual void Paging()
        {
            GeminiUrl("list?m1=CustomerRepository&a1=FindIndividualCustomerByName&p1=1&ps1=20&pm1_firstName=%22%22&pm1_lastName=%22a%22");
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
        public virtual void PagingTableView()
        {
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
        public virtual void ListDoesNotRefreshWithoutReload()
        {
            GeminiUrl("list?m1=SpecialOfferRepository&a1=SpecialOffersWithNoMinimumQty&p1=1&ps1=20");
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details")).Text
                .StartsWith("Page 1 of 1;"));
            GeminiUrl("object?o1=___1.SpecialOffer-7");
            EditObject();
            ClearFieldThenType("#minqty1", "10");
            SaveObject();
            GeminiUrl("list?m1=SpecialOfferRepository&a1=SpecialOffersWithNoMinimumQty&p1=1&ps1=20");
            WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details")).Text
                == "Page 1 of 1; viewing 11 of 11 items");
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details")).Text
                == "Page 1 of 1; viewing 10 of 10 items");

            //Undo to leave in original state
            GeminiUrl("object?o1=___1.SpecialOffer-7");
            EditObject();
            ClearFieldThenType("#minqty1", Keys.Backspace + Keys.Backspace + "0");
            SaveObject();
            GeminiUrl("list?m1=SpecialOfferRepository&a1=SpecialOffersWithNoMinimumQty&p1=1&ps1=20");
            WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection .summary .details")).Text
                == "Page 1 of 1; viewing 11 of 11 items");
        }
    }
    public abstract class ListTests : ListTestsRoot
    {
        [TestMethod]
        public override void ActionReturnsListView() { base.ActionReturnsListView(); }
        [TestMethod]
        public override void TableViewAttributeHonoured() { base.TableViewAttributeHonoured(); }
        [TestMethod]
        public override void SwitchToTableViewAndBackToList() { base.SwitchToTableViewAndBackToList(); }
        [TestMethod]
        public override void NavigateToItemFromListView() { base.NavigateToItemFromListView(); }
        [TestMethod]
        public override void NavigateToItemFromTableView() { base.NavigateToItemFromTableView(); }
        [TestMethod]
        public override void Paging() { base.Paging(); }
        [TestMethod]
        public override void PagingTableView() { base.PagingTableView(); }
        [TestMethod]
        public override void ListDoesNotRefreshWithoutReload() { base.ListDoesNotRefreshWithoutReload(); }
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class ListTestsIe : ListTests
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

    //[TestClass]
    public class ListTestsFirefox : ListTests
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

    //[TestClass, Ignore]
    public class ListTestsChrome : ListTests
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
    #region Mega tests
    public abstract class MegaListTestsRoot : ListTestsRoot
    {
        [TestMethod]
        public void MegaListTest()
        {
            base.ActionReturnsListView();
            base.SwitchToTableViewAndBackToList();

            base.NavigateToItemFromListView();

            base.NavigateToItemFromTableView();

            base.Paging();

            base.PagingTableView();
            base.ListDoesNotRefreshWithoutReload();
        }
    }
    [TestClass]
    public class MegaListTestsFirefox : MegaListTestsRoot
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