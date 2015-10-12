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
            br.Navigate().GoToUrl(CustomersMenuUrl);
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
            br.Navigate().GoToUrl(OrdersMenuUrl);
            WaitFor(Pane.Single, PaneType.Home, "Home");
            RightClick(GetObjectAction("Highest Value Orders"));
            WaitFor(Pane.Left, PaneType.Home, "Home");
            WaitFor(Pane.Right, PaneType.Query, "Highest Value Orders");
        }

        [TestMethod]
        public virtual void RightClickReferenceFromQuerySingle()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
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
            br.Navigate().GoToUrl(StoreDetailsTwinCyclesActionsOpen);
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
            var reference = FindElementByCss(".property .reference", 0);
            Assert.AreEqual("Lynn Tsoflias", reference.Text);
            RightClick(reference);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "Lynn Tsoflias");
        }

        [TestMethod]
        public virtual void RightClickActionFromObjectSingle()
        {
            br.Navigate().GoToUrl(StoreDetailsTwinCyclesActionsOpen);
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
            RightClick(GetObjectAction("Last Order"));
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
        }

        [TestMethod]
        public virtual void RightClickHomeIconFromObjectSingle()
        {
            br.Navigate().GoToUrl(StoreDetailsTwinCyclesActionsOpen);
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
            RightClick(HomeIcon());
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Home, "Home");
        }

        [TestMethod]
        public virtual void SwapPanesIconFromSingleOpensHomeOnLeft()
        {
            br.Navigate().GoToUrl(StoreDetailsTwinCyclesActionsOpen);
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
            Click(SwapIcon());
            WaitFor(Pane.Left, PaneType.Home, "Home");
            WaitFor(Pane.Right, PaneType.Object, "Twin Cycles, AW00000555");
        }
        #endregion

        #region Actions within split panes
        private const string twoObjects = Url + "#/gemini/object/object?object1=AdventureWorksModel.Store-350&actions1=open&object2=AdventureWorksModel.SalesOrderHeader-71926&actions2=open";

        [TestMethod]
        public virtual void RightClickReferenceInLeftPaneObject()
        {
            br.Navigate().GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            var reference = FindElementByCss(".property .reference", 1);
            Assert.AreEqual("Australia", reference.Text);
            RightClick(reference);

            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "Australia");
        }

        [TestMethod]
        public virtual void ClickReferenceInLeftPaneObject()
        {
            br.Navigate().GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            var reference = FindElementByCss(".property .reference", 1);
            Assert.AreEqual("Australia", reference.Text);
            Click(reference);

            WaitFor(Pane.Left, PaneType.Object, "Australia");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
        }

        [TestMethod]
        public virtual void ClickReferenceInRightPaneObject()
        {
            br.Navigate().GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            var reference = FindElementByCss(".property .reference", 3);
            Assert.AreEqual("Sandra Altamirano, Owner", reference.Text);
            Click(reference);

            WaitFor(Pane.Right, PaneType.Object, "Sandra Altamirano, Owner");
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
        }

        [TestMethod]
        public virtual void RightClickReferenceInRightPaneObject()
        {
            br.Navigate().GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            var reference = FindElementByCss(".property .reference", 3);
            Assert.AreEqual("Sandra Altamirano, Owner", reference.Text);
            RightClick(reference);

            WaitFor(Pane.Right, PaneType.Object, "SO71926");
            WaitFor(Pane.Left, PaneType.Object, "Sandra Altamirano, Owner");
        }

        [TestMethod]
        public virtual void LeftClickHomeIconFromSplitObjectObject()
        {
            br.Navigate().GoToUrl(twoObjects);
            Click(HomeIcon());
            WaitFor(Pane.Left, PaneType.Home, "Home");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
        }

        [TestMethod]
        public virtual void RightClickHomeIconFromSplitObjectObject()
        {
            br.Navigate().GoToUrl(twoObjects);
            RightClick(HomeIcon());
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Home, "Home");
        }

        [TestMethod]
        public virtual void ActionDialogOpensInCorrectPane()
        {
            br.Navigate().GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            OpenActionDialog("Search For Orders", Pane.Left);
            CancelDialog(Pane.Left);
            OpenActionDialog("Add New Sales Reasons", Pane.Right);
            CancelDialog(Pane.Right);
        }

        [TestMethod]
        public virtual void RightClickIsSameAsLeftClickForDialogActions() {
            br.Navigate().GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");

            RightClick(GetObjectAction("Search For Orders", Pane.Left));
            var selector = CssSelectorFor(Pane.Left) + " .dialog ";
            var dialog = wait.Until(d => d.FindElement(By.CssSelector(selector)));
        }

        [TestMethod]
        public virtual void SwapPanes()
        {
            br.Navigate().GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
            Click(SwapIcon());
            WaitFor(Pane.Left, PaneType.Object, "SO71926");
            WaitFor(Pane.Right, PaneType.Object, "Twin Cycles, AW00000555");
        }

        [TestMethod]
        public virtual void FullPaneFromLeft()
        {
            br.Navigate().GoToUrl(twoObjects);
            WaitFor(Pane.Left, PaneType.Object, "Twin Cycles, AW00000555");
            WaitFor(Pane.Right, PaneType.Object, "SO71926");
            Click(FullIcon());
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
        }

        [TestMethod]
        public virtual void FullPaneFromRight()
        {
            br.Navigate().GoToUrl(twoObjects);
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
            br.Navigate().GoToUrl(Url);
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