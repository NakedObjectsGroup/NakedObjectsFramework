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

namespace NakedObjects.Web.UnitTests.Selenium {

    public abstract class SplitPaneTests : GeminiTest {

        #region Actions that go from single to split panes
        [TestMethod]
        public virtual void RightClickActionReturningObjectFromHomeSingle()
        {
            GoToUrl(CustomersMenuUrl);
            WaitFor(Pane.Single, PaneType.Home, "Home");
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            FindElementByCss(".value  input").SendKeys("00022262");
            RightClick(OKButton());
            WaitFor(Pane.Left, PaneType.Home, "Home");
            WaitFor(Pane.Right, PaneType.Object, "Marcus Collins, AW00022262");
        }

        [TestMethod]
        public virtual void RightClickActionReturningQueryFromHomeSingle()
        {
            GoToUrl(OrdersMenuUrl);
            WaitFor(Pane.Single, PaneType.Home, "Home");
            RightClick(GetObjectAction("Highest Value Orders"));
            WaitFor(Pane.Left, PaneType.Home, "Home");
            WaitFor(Pane.Right, PaneType.Query, "Highest Value Orders");
        }

        [TestMethod]
        public virtual void RightClickReferenceFromQuerySingle()
        {
            GoToUrl(OrdersMenuUrl);
            Click(GetObjectAction("Highest Value Orders"));
            WaitFor(Pane.Single, PaneType.Query, "Highest Value Orders");
            var row = wait.Until(dr => dr.FindElement(By.CssSelector("table .reference")));
            Assert.AreEqual("SO51131", row.Text);
            RightClick(row);
            WaitFor(Pane.Left, PaneType.Query, "Highest Value Orders");
            WaitFor(Pane.Right, PaneType.Object, "SO51131");
        }

        [TestMethod]
        public virtual void RightClickReferencePropertyFromObjectSingle()
        {
            GoToUrl(StoreDetailsTwinCyclesActionsOpen);
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles");
            var reference = FindElementByCss(".property .reference", 0);
            Assert.AreEqual("Lynn Tsoflias", reference.Text);
            RightClick(reference);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitFor(Pane.Right, PaneType.Object, "Lynn Tsoflias");
        }

        [TestMethod]
        public virtual void RightClickActionFromObjectSingle()
        {
            GoToUrl(CustomerTechnicalPartsActionsOpen);
            WaitFor(Pane.Single, PaneType.Object, "Technical Parts Manufacturing, AW00030116");
            RightClick(GetObjectAction("Last Order"));
            WaitFor(Pane.Left, PaneType.Object, "Technical Parts Manufacturing, AW00030116");
            WaitFor(Pane.Right, PaneType.Object, "SO67279");
        }

        [TestMethod]
        public virtual void RightClickHomeIconFromObjectSingle()
        {
            GoToUrl(StoreDetailsTwinCyclesActionsOpen);
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles");
            RightClick(HomeIcon());
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitFor(Pane.Right, PaneType.Home, "Home");
        }

        [TestMethod]
        public virtual void SwapPanesIconFromSingleOpensHomeOnLeft()
        {
            GoToUrl(StoreDetailsTwinCyclesActionsOpen);
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles");
            Click(SwapIcon());
            WaitFor(Pane.Left, PaneType.Home, "Home");
            WaitFor(Pane.Right, PaneType.Object, "Twin Cycles");
        }
        #endregion

        #region Actions within split panes
        private const string twoObjects = BaseUrl + "#/gemini/object/object?object1=AdventureWorksModel.Customer-555&actions1=open&object2=AdventureWorksModel.SalesOrderHeader-71926&actions2=open";
        private const string twoObjectsB = BaseUrl + "#/gemini/object/object?object1=AdventureWorksModel.Store-350&actions1=open&object2=AdventureWorksModel.SalesOrderHeader-71926&actions2=open";

        [TestMethod]
        public virtual void RightClickReferenceInLeftPaneObject()
        {
            GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            var reference = FindElementByCss(".property .reference", 0);
            Assert.AreEqual("Australia", reference.Text);
            RightClick(reference);

            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "Australia");
        }

        [TestMethod]
        public virtual void ClickReferenceInLeftPaneObject()
        {
            GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            var reference = FindElementByCss(".property .reference", 0);
            Assert.AreEqual("Australia", reference.Text);
            Click(reference);

            WaitFor(Pane.Left, PaneType.Object, "Australia");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
        }

        [TestMethod]
        public virtual void ClickReferenceInRightPaneObject()
        {
            GoToUrl(twoObjectsB);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            var reference = FindElementByCss(".property .reference", 2);
            Assert.AreEqual("2253-217 Palmer Street ...", reference.Text);
            Click(reference);

            WaitFor(Pane.Right, PaneType.Object, "2253-217 Palmer Street ...");
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles");
        }

        [TestMethod]
        public virtual void RightClickReferenceInRightPaneObject()
        {
            GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            var reference = FindElementByCss(".property .reference", 5);
            Assert.AreEqual("CARGO TRANSPORT 5", reference.Text);
            RightClick(reference);

            WaitFor(Pane.Right, PaneType.Object, "SO71926");
            WaitFor(Pane.Left, PaneType.Object, "CARGO TRANSPORT 5");
        }

        [TestMethod]
        public virtual void LeftClickHomeIconFromSplitObjectObject()
        {
            GoToUrl(twoObjects);
            Click(HomeIcon());
            WaitFor(Pane.Left, PaneType.Home, "Home");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
        }

        [TestMethod]
        public virtual void RightClickHomeIconFromSplitObjectObject()
        {
            GoToUrl(twoObjects);
            RightClick(HomeIcon());
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Home, "Home");
        }

        [TestMethod]
        public virtual void ActionDialogOpensInCorrectPane()
        {
            GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            OpenActionDialog("Search For Orders", Pane.Left);
            CancelDialog(Pane.Left);
            OpenActionDialog("Add New Sales Reasons", Pane.Right);
            CancelDialog(Pane.Right);
        }

        [TestMethod]
        public virtual void RightClickIsSameAsLeftClickForDialogActions() {
            GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            RightClick(GetObjectAction("Search For Orders", Pane.Left));
            var selector = CssSelectorFor(Pane.Left) + " .dialog ";
            var dialog = wait.Until(d => d.FindElement(By.CssSelector(selector)));
        }

        [TestMethod]
        public virtual void SwapPanes()
        {
            GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
            Click(SwapIcon());
            WaitFor(Pane.Left, PaneType.Object, "SO71926");
            WaitFor(Pane.Right, PaneType.Object, "Twin Cycles, AW00000555");
        }

        [TestMethod]
        public virtual void FullPaneFromLeft()
        {
            GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
            Click(FullIcon());
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
        }

        [TestMethod]
        public virtual void FullPaneFromRight()
        {
            GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
            RightClick(FullIcon());
            WaitFor(Pane.Single, PaneType.Object, "SO71926");
        }


        #endregion

    }

    #region browser specific subclasses

    //[TestClass, Ignore]
    public class SplitPaneTestsIe : SplitPaneTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            GoToUrl(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class SplitPaneTestsFirefox : SplitPaneTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
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

    //[TestClass, Ignore]
    public class SplitPaneTestsChrome : SplitPaneTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
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
}