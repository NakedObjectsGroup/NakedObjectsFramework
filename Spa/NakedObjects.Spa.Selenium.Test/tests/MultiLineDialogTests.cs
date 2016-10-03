// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    public abstract class MultiLineDialogTestsRoot : AWTest {
        public virtual void MultiLineMenuAction()
        {
            GeminiUrl("home?m1=SpecialOfferRepository");
            Click(GetObjectAction("Create Multiple Special Offers"));
            WaitForView(Pane.Single, PaneType.MultiLineDialog, "Create Multiple Special Offers");
            WaitForTextEquals(".count", "with 0 lines submitted.");
            //line 0
            OKButtonOnLine(0).AssertIsDisabled("Missing mandatory fields: Description; Discount Pct; Type; Category; Min Qty; ");
            ClearFieldThenType("#description0", "x");
            ClearFieldThenType("#discountpct0", ".03");
            ClearFieldThenType("#type0", "Promotion");
            SelectDropDownOnField("#category0", "Reseller");
            ClearFieldThenType("#minqty0", "10");
            OKButtonOnLine(0).AssertIsEnabled();
            Click(OKButtonOnLine(0));
            WaitForTextEquals(".co-validation", 0, "Submitted");
            WaitForTextEquals(".count", "with 1 lines submitted.");
            WaitUntilElementDoesNotExist("#description0");
            //line 1
            OKButtonOnLine(1).AssertIsDisabled("Missing mandatory fields: Description; Discount Pct; Type; Category; Min Qty; ");
            ClearFieldThenType("#description1", "y");
            ClearFieldThenType("#discountpct1", ".04");
            ClearFieldThenType("#type1", "Promotion");
            SelectDropDownOnField("#category1", "Reseller");
            ClearFieldThenType("#minqty1", "5");
            OKButtonOnLine(1).AssertIsEnabled();
            Click(OKButtonOnLine(1));
            WaitForTextEquals(".co-validation", 1, "Submitted");
            WaitUntilElementDoesNotExist("#description1");
            WaitForTextEquals(".count", "with 2 lines submitted.");

            //Check that new empty line (2) has been created
            WaitForCss("#description2");
            OKButtonOnLine(2).AssertIsDisabled("Missing mandatory fields: Description; Discount Pct; Type; Category; Min Qty; ");

            //Close the MLD
            Click(WaitForCss(".close"));
            WaitForView(Pane.Single, PaneType.Home);
        }

        public virtual void MultiLineObjectAction() {
            GeminiUrl("object?i1=View&r=1&o1=___1.Customer--29562&as1=open&d1=CreateNewOrder");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Sales Order");
            SaveObject();
            GetButton("Edit"); //Waiting for save
            WaitForTextEquals(".summary .details", 0, "Empty");
            OpenObjectActions();
            Click(GetObjectAction("Add New Details"));
            WaitForView(Pane.Single, PaneType.MultiLineDialog);
            wait.Until(dr => dr.FindElement(By.CssSelector(".title")).Text.Contains("Add New Details"));
            WaitForCss(".lineDialog", 6);
            WaitForTextEquals(".count", "with 0 lines submitted.");

            //line0
            OKButtonOnLine(0).AssertIsDisabled("Missing mandatory fields: Product; ");
            //Auto-complete
            ClearFieldThenType("#product0", "Dissolver");
            wait.Until(d => d.FindElements(By.CssSelector(".ui-menu-item")).Count > 0);
            //As the match has not yet been selected,the field is invalid, so...
            WaitForTextEquals(".validation",0, "Pending auto-complete...");
            OKButtonOnLine(0).AssertIsDisabled().AssertHasTooltip("Invalid fields: Product; ");
            Click(WaitForCss(".ui-menu-item"));
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
            WaitForTextEquals(".co-validation",0,"Submitted");
            WaitForTextEquals(".count", "with 1 lines submitted.");
            WaitUntilElementDoesNotExist("#description0");

            //line1
            OKButtonOnLine(1).AssertIsDisabled("Missing mandatory fields: Product; ");
            //Auto-complete
            ClearFieldThenType("#product1", "vest, S");
            wait.Until(d => d.FindElements(By.CssSelector(".ui-menu-item")).Count > 0);
            Click(WaitForCss(".ui-menu-item"));
            WaitForCss("#product1.link-color4");
            Click(OKButtonOnLine(1));
            WaitForTextEquals(".co-validation", 1, "Submitted");
            WaitForTextEquals(".count", "with 2 lines submitted.");
            WaitUntilElementDoesNotExist("#product1");

            WaitForCss("#product2");
            OKButtonOnLine(2);

            //Close
            Click(WaitForCss(".close"));
            WaitForTextStarting(".title", "SO"); //back to order
            WaitForTextEquals(".summary .details", 0, "2 Items");
        }
    }

    public abstract class MultiLineDialogsTests : MultiLineDialogTestsRoot {
     
        [TestMethod]
        public override void MultiLineMenuAction()
        {
            base.MultiLineMenuAction();
        }
        [TestMethod]
        public override void MultiLineObjectAction()
        {
            base.MultiLineObjectAction();
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
            //MultiLineMenuAction();
            MultiLineObjectAction();
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
    public class MegaMultiLineDialogTestsChrome : MegaMultiLineDialogTestsRoot
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