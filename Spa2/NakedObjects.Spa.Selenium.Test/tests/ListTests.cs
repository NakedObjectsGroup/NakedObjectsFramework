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
    /// Tests applied from a List view.
    /// </summary>
    public abstract class ListTestsRoot : AWTest {
        public virtual void ActionReturnsListView() {
            Url(OrdersMenuUrl);
            Click(GetObjectEnabledAction("Highest Value Orders"));
            WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
            //Test content of collection
            Assert.IsTrue(WaitForCss(".list .summary .details").Text.StartsWith("Page 1 of 1574; viewing 20 of"));
            WaitForCss(".icon.table");
            WaitUntilElementDoesNotExist(".icon.list");
            WaitUntilElementDoesNotExist(".icon.minimise");
            Assert.AreEqual(20, br.FindElements(By.CssSelector(".list table tbody tr td.reference")).Count);
            Assert.AreEqual(20, br.FindElements(By.CssSelector(".list table tbody tr td.checkbox")).Count);
            Assert.AreEqual(0, br.FindElements(By.CssSelector(".cell")).Count); //Cells are in Table view only
        }

        public virtual void ActionReturnsEmptyList() {
            GeminiUrl("home?m1=ProductRepository&d1=FindProductByName");
            ClearFieldThenType("#searchstring1", "zzz");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Find Product By Name");
            WaitForTextEquals(".details", "No items found");
        }

        public virtual void TableViewAttributeHonoured() {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1_=0&c1=Table");
            Reload();
            var cols = WaitForCss("th", 5).ToArray();
            Assert.AreEqual("", cols[0].Text);
            Assert.AreEqual("Description", cols[1].Text);
            Assert.AreEqual("XNoMatchingColumn", cols[2].Text);
            Assert.AreEqual("Category", cols[3].Text);
            Assert.AreEqual("Discount Pct", cols[4].Text);
            cols = WaitForCss("tbody tr:first-child td", 5).ToArray();
            Assert.AreEqual("No Discount", cols[1].Text);
            Assert.AreEqual("", cols[2].Text); //As no such column
            Assert.AreEqual("", cols[3].Text); //Happens to be empty
            Assert.AreEqual("0", cols[4].Text);
        }

        public virtual void TableViewWorksWithSubTypes() {
            GeminiUrl("list?m1=CustomerRepository&a1=RandomCustomers&pg1=1&ps1=20&s1_=0&c1=Table");
            WaitForView(Pane.Single, PaneType.List, "Random Customers");
            Reload();
            var cols = WaitForCss("th", 4).ToArray();
            Assert.AreEqual("Account Number", cols[0].Text);
            Assert.AreEqual("Store", cols[1].Text);
            Assert.AreEqual("Person", cols[2].Text);
            Assert.AreEqual("Sales Territory", cols[3].Text);
            cols = WaitForCss("tbody tr:first-child td", 4).ToArray();
            Assert.AreEqual("", cols[1].Text); //As no such column
        }

        public virtual void TableViewCanIncludeCollectionSummaries() {
            GeminiUrl("list?m1=OrderRepository&a1=OrdersWithMostLines&pg1=1&ps1=20&s1_=0&c1=Table");
            Reload();
            var header = WaitForCss("thead");
            var cols = header.FindElements(By.CssSelector("th")).ToArray();
            Assert.AreEqual(4, cols.Length);
            Assert.AreEqual("", cols[0].Text);
            Assert.AreEqual("", cols[1].Text);
            Assert.AreEqual("Order Date", cols[2].Text);
            Assert.AreEqual("Details", cols[3].Text);
            WaitForTextEquals("tbody tr:nth-child(1) td:nth-child(4)", "72 Items");
        }

        public virtual void SwitchToTableViewAndBackToList() {
            Url(SpecialOffersMenuUrl);
            Click(GetObjectEnabledAction("Current Special Offers"));
            WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
            wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
            var iconTable = WaitForCss(".icon.table");
            Click(iconTable);

            var iconList = WaitForCss(".icon.list");
            WaitUntilElementDoesNotExist(".icon.table");
            WaitUntilElementDoesNotExist(".icon.minimise");

            wait.Until(dr => dr.FindElements(By.CssSelector(".list table tbody tr")).Count > 1);

            //Switch back to List view
            Click(iconList);
            WaitForCss(".icon.table");
            WaitUntilElementDoesNotExist(".icon.list");
            WaitUntilElementDoesNotExist(".icon.minimise");
            wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
            Assert.AreEqual(0, br.FindElements(By.CssSelector(".cell")).Count); //Cells are in Table view only
        }

        public virtual void NavigateToItemFromListView() {
            Url(SpecialOffersMenuUrl);
            Click(GetObjectEnabledAction("Current Special Offers"));
            WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
            var row = WaitForCss(".reference");
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, "No Discount");
        }

        public virtual void NavigateToItemFromTableView() {
            Url(SpecialOffersMenuUrl);
            Click(GetObjectEnabledAction("Current Special Offers"));
            WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
            wait.Until(dr => dr.FindElements(By.CssSelector(".reference")).Count > 1);
            var iconTable = WaitForCss(".icon.table");
            Click(iconTable);
            Thread.Sleep(500);
            wait.Until(dr => dr.FindElement(By.CssSelector("tbody tr:nth-child(1) td:nth-child(2)")).Text == "No Discount");
            var row = br.FindElement(By.CssSelector("tbody tr:nth-child(1) div"));
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, "No Discount");
        }

        public virtual void Paging() {
            GeminiUrl("list?m1=CustomerRepository&a1=FindIndividualCustomerByName&p1=1&ps1=20&pm1_firstName=%22%22&pm1_lastName=%22a%22");
            Thread.Sleep(1000);
            Reload();

            //Test content of collection
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 1 of 45"));
            GetInputButton("First").AssertIsDisabled();
            GetInputButton("Previous").AssertIsDisabled();
            var next = GetInputButton("Next").AssertIsEnabled();
            GetInputButton("Last").AssertIsEnabled();
            //Go to next page
            Click(next);
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 2 of 45"));
            GetInputButton("First").AssertIsEnabled();
            GetInputButton("Previous").AssertIsEnabled();
            GetInputButton("Next").AssertIsEnabled();
            var last = GetInputButton("Last").AssertIsEnabled();
            Click(last);
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 45 of 45"));
            GetInputButton("First").AssertIsEnabled();
            var prev = GetInputButton("Previous").AssertIsEnabled();
            GetInputButton("Next").AssertIsDisabled();
            GetInputButton("Last").AssertIsDisabled();
            Click(prev);
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 44 of 45"));
            var first = GetInputButton("First").AssertIsEnabled();
            GetInputButton("Previous").AssertIsEnabled();
            GetInputButton("Next").AssertIsEnabled();
            GetInputButton("Last").AssertIsEnabled();
            Click(first);
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 1 of 45"));
        }

        public virtual void PageSizeRecognised() {
            //Method marked with PageSize(2)
            GeminiUrl("home?m1=CustomerRepository&d1=FindStoreByName");
            ClearFieldThenType("#name1", "bike");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Find Store By Name");
            var summary = WaitForCss(".list .summary .details");
            Assert.AreEqual("Page 1 of 177; viewing 2 of 353 items", summary.Text);
            var next = GetInputButton("Next").AssertIsEnabled();
            Click(next);
            wait.Until(dr => dr.FindElement(
                                     By.CssSelector(".list .summary .details"))
                                 .Text == "Page 2 of 177; viewing 2 of 353 items");
        }

        public virtual void ListDoesNotRefreshWithoutReload() {
            GeminiUrl("list?m1=SpecialOfferRepository&a1=SpecialOffersWithNoMinimumQty&p1=1&ps1=20");
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
                .StartsWith("Page 1 of 1;"));
            Click(HomeIcon());
            WaitForView(Pane.Single, PaneType.Home, "Home");
            GoToMenuFromHomePage("Special Offers");
            Click(GetObjectEnabledAction("Special Offers With No Minimum Qty"));
            WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");

            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
                             == "Page 1 of 1; viewing 11 of 11 items");

            WaitForCss("tbody tr:nth-child(2) td:nth-child(2)");

            var row = br.FindElement(By.CssSelector("tbody tr:nth-child(2) td:nth-child(2)"));
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, "Mountain-100 Clearance Sale");
            //GeminiUrl("object?o1=___1.SpecialOffer--7");
            EditObject();
            ClearFieldThenType("#minqty1", "10");
            SaveObject();
            ClickBackButton();
            ClickBackButton();
            //ClickBackButton();
            WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");

            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
                             == "Page 1 of 1; viewing 11 of 11 items");
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
                             == "Page 1 of 1; viewing 10 of 10 items");

            //Undo to leave in original state
            GeminiUrl("object?o1=___1.SpecialOffer--7");
            EditObject();
            ClearFieldThenType("#minqty1", Keys.Backspace + Keys.Backspace + "0");
            SaveObject();
            GeminiUrl("list?m1=SpecialOfferRepository&a1=SpecialOffersWithNoMinimumQty&p1=1&ps1=20");
            WaitForView(Pane.Single, PaneType.List, "Special Offers With No Minimum Qty");
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details")).Text
                             == "Page 1 of 1; viewing 11 of 11 items");
        }

        public virtual void ReloadingListGetsUpdatedObject() {
            Url(SpecialOffersMenuUrl);
            Click(GetObjectEnabledAction("Current Special Offers"));
            WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
            WaitForCss(".reference", 16);
            var row = WaitForCssNo(".reference", 6);
            Click(row);
            WaitForView(Pane.Single, PaneType.Object);
            EditObject();
            var rand = new Random();
            var suffix = rand.Next().ToString();
            var newDescription = "Mountain-100 Clearance Sale " + suffix;
            ClearFieldThenType("#description1", newDescription);
            SaveObject();

            ClickBackButton();
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.List, "Current Special Offers");
            Reload();
            WaitForCss(".reference", 16);
            row = WaitForCssNo(".reference", 6);
            Click(row);
            WaitForView(Pane.Single, PaneType.Object);
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(1)")).Text.EndsWith(suffix));

            //Now revert
            EditObject();
            ClearFieldThenType("#description1", "Mountain-100 Clearance Sale");
            SaveObject();
        }

        public virtual void EagerlyRenderTableViewFromAction() {
            GeminiUrl("home?m1=EmployeeRepository");
            Click(GetObjectEnabledAction("List All Departments"));
            WaitForView(Pane.Single, PaneType.List, "List All Departments");
            WaitForCss(".icon.list");
            var header = WaitForCss("thead");
            var cols = header.FindElements(By.CssSelector("th")).ToArray();
            Assert.AreEqual(2, cols.Length);
            Assert.AreEqual("", cols[0].Text);
            Assert.AreEqual("Group Name", cols[1].Text);
        }



        public virtual void PagingTableView() {
            GeminiUrl("list?m1=CustomerRepository&a1=FindIndividualCustomerByName&p1=1&ps1=20&pm1_firstName=%22%22&pm1_lastName=%22a%22&c1=Table");
            Reload();
            //Confirm in Table view
            WaitForCss("thead tr th");
            WaitForCss(".icon.list");
            WaitUntilElementDoesNotExist(".icon.table");
            //Test content of collection
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 1 of"));
            GetInputButton("First").AssertIsDisabled();
            GetInputButton("Previous").AssertIsDisabled();
            var next = GetInputButton("Next").AssertIsEnabled();
            GetInputButton("Last").AssertIsEnabled();
            //Go to next page
            Click(next);
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 2 of"));
            //Confirm in Table view
            WaitForCss("thead tr th");
            WaitForCss(".icon.list");
            WaitUntilElementDoesNotExist(".icon.table");

            GetInputButton("First").AssertIsEnabled();
            GetInputButton("Previous").AssertIsEnabled();
            GetInputButton("Next").AssertIsEnabled();
            var last = GetInputButton("Last").AssertIsEnabled();
            Click(last);
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 45 of 45"));
            //Confirm in Table view
            WaitForCss("thead tr th");
            var iconList = WaitForCss(".icon.list");
            WaitUntilElementDoesNotExist(".icon.table");

            GetInputButton("First").AssertIsEnabled();
            var prev = GetInputButton("Previous").AssertIsEnabled();
            GetInputButton("Next").AssertIsDisabled();
            GetInputButton("Last").AssertIsDisabled();
            Click(prev);
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 44 of 45"));
            //Confirm in Table view
            WaitForCss("thead tr th");
            WaitForCss(".icon.list");
            WaitUntilElementDoesNotExist(".icon.table");
            var first = GetInputButton("First").AssertIsEnabled();
            GetInputButton("Previous").AssertIsEnabled();
            GetInputButton("Next").AssertIsEnabled();
            GetInputButton("Last").AssertIsEnabled();
            Click(first);
            wait.Until(dr => dr.FindElement(By.CssSelector(".list .summary .details"))
                .Text.StartsWith("Page 1 of 45"));
            //Confirm in Table view
            WaitForCss("thead tr th");
            WaitForCss(".icon.list");
            WaitUntilElementDoesNotExist(".icon.table");
        }

    }

    public abstract class ListTests : ListTestsRoot {
        [TestMethod]
        public override void ActionReturnsListView() {
            base.ActionReturnsListView();
        }

        [TestMethod]
        public override void ActionReturnsEmptyList() {
            base.ActionReturnsEmptyList();
        }

        [TestMethod]
        public override void TableViewAttributeHonoured() {
            base.TableViewAttributeHonoured();
        }

        [TestMethod]
        public override void TableViewWorksWithSubTypes() {
            base.TableViewWorksWithSubTypes();
        }

        [TestMethod]
        public override void TableViewCanIncludeCollectionSummaries() {
            base.TableViewCanIncludeCollectionSummaries();
        }

        [TestMethod]
        public override void SwitchToTableViewAndBackToList() {
            base.SwitchToTableViewAndBackToList();
        }

        [TestMethod]
        public override void NavigateToItemFromListView() {
            base.NavigateToItemFromListView();
        }

        [TestMethod]
        public override void NavigateToItemFromTableView() {
            base.NavigateToItemFromTableView();
        }

        [TestMethod]
        public override void Paging() {
            base.Paging();
        }

        [TestMethod]
        public override void PageSizeRecognised() {
            base.PageSizeRecognised();
        }

        [TestMethod]
        public override void ListDoesNotRefreshWithoutReload() {
            base.ListDoesNotRefreshWithoutReload();
        }

        [TestMethod]
        public override void ReloadingListGetsUpdatedObject() {
            base.ReloadingListGetsUpdatedObject();
        }

        [TestMethod]
        public override void EagerlyRenderTableViewFromAction() {
            base.EagerlyRenderTableViewFromAction();
        }
    }

    #region browsers specific subclasses

    public class ListTestsIe : ListTests {
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
            CleanUpTest();
        }
    }

    //[TestClass] //Firefox Individual
    public class ListTestsFirefox : ListTests {
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
            CleanUpTest();
        }
    }

    //[TestClass]
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
            CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = $"window.scrollTo({element.Location.X}, {element.Location.Y});return true;";
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion

    #region Mega tests

    public abstract class MegaListTestsRoot : ListTestsRoot {
        [TestMethod] //Mega
        [Priority(0)]
        public void ListTests() {
            ActionReturnsEmptyList();
            TableViewAttributeHonoured();
            TableViewWorksWithSubTypes();
            TableViewCanIncludeCollectionSummaries();
            SwitchToTableViewAndBackToList();
            NavigateToItemFromListView();
            NavigateToItemFromTableView();
            Paging();
            PageSizeRecognised();
            ListDoesNotRefreshWithoutReload();
            ReloadingListGetsUpdatedObject();
            EagerlyRenderTableViewFromAction();
            PagingTableView();
        }
        [TestMethod]
        [Priority(-1)]
        public void ProblematicListTests() {
            ActionReturnsListView();
        }
    }

    //[TestClass]
    public class MegaListTestsFirefox : MegaListTestsRoot {
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
            CleanUpTest();
        }
    }

    //[TestClass]
    public class MegaListTestsIe : MegaListTestsRoot {
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
            CleanUpTest();
        }
    }

   [TestClass] //toggle
    public class MegaListTestsChrome : MegaListTestsRoot {
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
            CleanUpTest();
        }
    }

    #endregion
}