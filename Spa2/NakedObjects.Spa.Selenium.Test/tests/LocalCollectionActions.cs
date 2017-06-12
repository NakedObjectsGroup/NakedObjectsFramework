// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    public abstract class LocalCollectionActionsTestsRoot : AWTest {
        public virtual void LocalCollectionActionsHonourMemberOrder() {
            GeminiUrl("object?i1=View&r1=1&o1=___1.SalesOrderHeader--71105&c1_Details=Table");
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[0].FindElements(By.CssSelector("nof-action-bar nof-action")).Count >= 2);
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[0].FindElements(By.CssSelector("nof-action-bar nof-action input"))[0].GetAttribute("value") == "Add New Details");
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[0].FindElements(By.CssSelector("nof-action-bar nof-action input"))[1].GetAttribute("value") == "Add New Detail");
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[0].FindElements(By.CssSelector("nof-action-bar nof-action input"))[2].GetAttribute("value") == "Remove Details");
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[0].FindElements(By.CssSelector("nof-action-bar nof-action input"))[3].GetAttribute("value") == "Adjust Quantities");
        }

        public virtual void CheckBoxesVisibleAndCanBeSelected() {
            GeminiUrl("object?i1=View&r1=1&o1=___1.SalesOrderHeader--44284&c1_Details=List");
            WaitForCss("input[type='checkbox']", 17); // 16 lines plus all
            WaitForSelectedCheckboxes(0);

            br.SwitchTo().ActiveElement().SendKeys(Keys.PageDown + Keys.PageDown);
            SelectCheckBox("input[type = 'checkbox']#details1-0");
            SelectCheckBox("input[type = 'checkbox']#details1-2");
            SelectCheckBox("input[type = 'checkbox']#details1-7");
            WaitForSelectedCheckboxes(3);
            SelectCheckBox("input[type = 'checkbox']#details1-2", true);
            WaitForSelectedCheckboxes(2);
            SelectCheckBox("input[type='checkbox']#details1-all");
            WaitForSelectedCheckboxes(17);
            SelectCheckBox("input[type='checkbox']#details1-all", true);
            WaitForSelectedCheckboxes(0);
        }

        public virtual void SelectionsPreservedIfNavigatingAwayAndBack() {
            GeminiUrl("object?i1=View&r1=1&o1=___1.SalesOrderHeader--67298&c1_Details=List");
            WaitForCss("input[type='checkbox']", 28);
            WaitForSelectedCheckboxes(0);
            SelectCheckBox("input[type = 'checkbox']#details1-0");
            SelectCheckBox("input[type = 'checkbox']#details1-3");
            SelectCheckBox("input[type = 'checkbox']#details1-4");
            WaitForSelectedCheckboxes(3);
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.Object);
            WaitForSelectedCheckboxes(3);
        }

        public virtual void SelectionsOnMultipleOpenCollectionsAreIndependent() {
            GeminiUrl("object?i1=View&o1=___1.SalesOrderHeader--53175&c1_SalesOrderHeaderSalesReason=List&c1_Details=List&s1_salesorderheadersalesreason=2&s1_details=5");
            WaitForSelectedCheckboxes(3); //2 in the first collection, one in the second 
        }

        public virtual void ActionsAvailableOnEmptyCollections() {
            GeminiUrl("object?i1=View&r1=1&o1=___1.SalesOrderHeader--70589");
            WaitForTextEquals(".collection", 1, "Reasons:\r\nEmpty");
            Click(WaitForCssNo(".icon.list", 1));

            //Add new sales reason;  remove sales reasons
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[1].FindElements(By.CssSelector("nof-action-bar nof-action input"))[0].GetAttribute("value") == "Add New Sales Reason");
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[1].FindElements(By.CssSelector("nof-action-bar nof-action input"))[1].GetAttribute("value") == "Remove Sales Reasons");
            Click(WaitForCss(".icon.summary"));
            WaitUntilElementDoesNotExist(".collection .actions");
        }

        public virtual void CannotInvokeZeroParamSelectionActionWithNothingSelected() {
            GeminiUrl("object?i1=View&r1=1&o1=___1.SalesOrderHeader--63023&c1_SalesOrderHeaderSalesReason=List");
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[1].FindElements(By.CssSelector("nof-action"))[1].Text == "Remove Sales Reasons");
            Click(GetObjectEnabledAction("Remove Sales Reasons"));
            WaitForTextEquals(".messages", 2, "Must select items for collection contributed action");
        }

        public virtual void CannotInvokeDialogSelectionActionWithNothingSelected() {
            GeminiUrl("object?i1=View&r1=1&o1=___1.SalesOrderHeader--63023&c1_SalesOrderHeaderSalesReason=Summary&c1_Details=List");

            Thread.Sleep(4000);
            var action = wait.Until(dr => dr.FindElement(By.CssSelector("nof-action input[value='Adjust Quantities'")));

            Click(action);

            WaitForTextEquals(".collection .dialog .title", "Adjust Quantities");
            TypeIntoFieldWithoutClearing("#newquantity1", "7");
            Click(OKButton());
            WaitForTextEquals(".collection .dialog .co-validation", "Must select items for collection contributed action");
        }

        public virtual void ZeroAndOneParamActionInvoked() {
            GeminiUrl("object?i1=View&r1=1&o1=___1.SalesOrderHeader--63074&c1_SalesOrderHeaderSalesReason=List");
            WaitForTextEquals(".collection .summary", 1, "Reasons:\r\n1 Item");
            SelectCheckBox("#salesorderheadersalesreason1-0");
            Click(GetObjectEnabledAction("Remove Sales Reasons"));
            WaitUntilElementDoesNotExist("#salesorderheadersalesreason1-0");
            WaitForTextEquals(".collection .summary", 1, "Reasons:\r\nEmpty");
            Click(GetObjectEnabledAction("Add New Sales Reason"));
            //Confirm that dialog has opened inside collection
            WaitForTextEquals(".collection .dialog .title", "Add New Sales Reason");
            SelectDropDownOnField("#reason1", "Price");
            Click(OKButton());
            WaitForTextEquals(".collection .summary", 1, "Reasons:\r\n1 Item");
        }
    }

    public abstract class LocalCollectionActionsTests : LocalCollectionActionsTestsRoot {
        [TestMethod]
        public override void LocalCollectionActionsHonourMemberOrder() {
            base.LocalCollectionActionsHonourMemberOrder();
        }

        [TestMethod]
        public override void CheckBoxesVisibleAndCanBeSelected() {
            base.CheckBoxesVisibleAndCanBeSelected();
        }

        [TestMethod]
        public override void SelectionsPreservedIfNavigatingAwayAndBack() {
            base.SelectionsPreservedIfNavigatingAwayAndBack();
        }

        [TestMethod]
        public override void ActionsAvailableOnEmptyCollections() {
            base.ActionsAvailableOnEmptyCollections();
        }

        [TestMethod]
        public override void CannotInvokeZeroParamSelectionActionWithNothingSelected() {
            base.CannotInvokeZeroParamSelectionActionWithNothingSelected();
        }

        [TestMethod]
        public override void CannotInvokeDialogSelectionActionWithNothingSelected() {
            base.CannotInvokeDialogSelectionActionWithNothingSelected();
        }

        [TestMethod]
        public override void ZeroAndOneParamActionInvoked() {
            base.ZeroAndOneParamActionInvoked();
        }

        [TestMethod]
        public override void SelectionsOnMultipleOpenCollectionsAreIndependent() {
            base.SelectionsOnMultipleOpenCollectionsAreIndependent();
        }
    }

    #region browsers specific subclasses

    public class LocalCollectionActionsTestsIe : LocalCollectionActionsTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
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

    //[TestClass] //Firefox Individual
    public class LocalCollectionActionsFirefox : LocalCollectionActionsTests {
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
            CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = $"window.scrollTo({element.Location.X}, {element.Location.Y});return true;";
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    public class LocalCollectionActionsTestsChrome : LocalCollectionActionsTests {
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
            CleanUpTest();
        }
    }

    #endregion

    #region Mega tests

    public abstract class MegaLocalCollectionActionsTestsRoot : LocalCollectionActionsTestsRoot {
        [TestMethod] //Mega
        [Priority(0)]
        public void LocalCollectionActionsTest() {
            LocalCollectionActionsHonourMemberOrder();
            CheckBoxesVisibleAndCanBeSelected();
            SelectionsPreservedIfNavigatingAwayAndBack();
            CannotInvokeDialogSelectionActionWithNothingSelected();
            ActionsAvailableOnEmptyCollections();
            SelectionsOnMultipleOpenCollectionsAreIndependent();
        }

        [TestMethod]
        [Priority(-1)]
        public void ProblematicLocalCollectionActionsTests() {
            CannotInvokeZeroParamSelectionActionWithNothingSelected();
            ZeroAndOneParamActionInvoked();
        }
    }

    //[TestClass]
    public class MegaLocalCollectionActionsTestsFirefox : MegaLocalCollectionActionsTestsRoot {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
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

    public class MegaLocalCollectionActionsTestsIe : MegaLocalCollectionActionsTestsRoot {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
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
    public class MegaLocalCollectionActionsTestsChrome : MegaLocalCollectionActionsTestsRoot {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
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