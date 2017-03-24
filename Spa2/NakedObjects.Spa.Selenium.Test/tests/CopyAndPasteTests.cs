// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    public abstract class CopyAndPasteTestsRoot : AWTest {
        public virtual void CopyTitleOrPropertyIntoClipboard() {
            GeminiUrl("object/object?o1=___1.Product--990&o2=___1.Customer--13179");
            WaitForView(Pane.Left, PaneType.Object, "Mountain-500 Black, 42");
            WaitForView(Pane.Right, PaneType.Object, "Adrian Sanchez, AW00013179");

            //Copy title from left pane
            var title = WaitForCss("#pane1 .header .title");
            title.Click();
            CopyToClipboard(title);

            //Copy title right pane
            title = WaitForCss("#pane2 .header .title");
            title.Click();
            CopyToClipboard(title);

            //Copy embedded reference from left pane
            WaitForCss("#pane1 .header .title").Click();
            var target = Tab(5);
            Assert.AreEqual("Mountain-500", target.Text);
            CopyToClipboard(target);

            //Copy embedded reference from right pane
            WaitForCss("#pane2 .header .title").Click();
            target = Tab(4);
            Assert.AreEqual("Canada", target.Text);
            CopyToClipboard(target);

            //Finish somewhere else
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
        }

        public virtual void CopyListItemIntoClipboard() {
            GeminiUrl("list/list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&m2=PersonRepository&a2=ValidCountries&p2=1&ps2=20");
            Reload(Pane.Left);
            var item = wait.Until(dr => dr.FindElements(By.CssSelector("#pane1 td"))[1]);
            Assert.AreEqual("No Discount", item.Text);
            CopyToClipboard(item);

            //Copy item from list, right pane
            Reload(Pane.Right);
            item = wait.Until(dr => dr.FindElements(By.CssSelector("#pane2 td"))[3]);
            Assert.AreEqual("Australia", item.Text);
            CopyToClipboard(item);
        }

        public virtual void PasteIntoReferenceField() {
            GeminiUrl("object/object?o1=___1.PurchaseOrderHeader--1372&i1=Edit&o2=___1.Employee--161");
            WaitForView(Pane.Left, PaneType.Object);
            Assert.AreEqual("Annette Hill", WaitForCss("#pane1 .property:nth-child(4) .value.droppable").GetAttribute("value"));
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Kirk Koenigsbauer", title.Text);
            title.Click();
            CopyToClipboard(title);
            PasteIntoReferenceField("#pane1 .property:nth-child(4) .value.droppable");
            Click(HomeIcon());
            WaitForView(Pane.Single, PaneType.Home);
        }

        public virtual void PasteIntoReferenceFieldThatAlsoHasAutoCompleteAndFindMenu() {
            GeminiUrl("object/object?o2=___1.SalesPerson--284&o1=___1.Store--740&i1=Edit");
            WaitForView(Pane.Left, PaneType.Object, "Editing - Touring Services");
            Assert.AreEqual("Tsvi Reiter", WaitForCss("input#salesperson1").GetAttribute("value"));
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Tete Mensa-Annan", title.Text);
            title.Click();
            Thread.Sleep(500);
            CopyToClipboard(title);
            PasteIntoInputField("input#salesperson1");
            //Now check that Auto-complete is working
            WaitForCss("input#salesperson1").Clear();
            ClearFieldThenType("input#salesperson1", "Ito");
            wait.Until(dr => dr.FindElement(By.CssSelector("md-option")).Text == "Shu Ito");
            var item = br.FindElement(By.CssSelector("md-option"));
            Click(item);
            wait.Until(dr => dr.FindElement(By.CssSelector("input#salesperson1")).GetAttribute("value") == "Shu Ito");
            //Finish somewhere else
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
        }

        public virtual void PasteIntoDialog() {
            GeminiUrl("home/object?m1=SalesRepository&d1=CreateNewSalesPerson&o2=___1.Employee--206");
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Stuart Munson", title.Text);
            title.Click();
            CopyToClipboard(title);
            string selector = "#pane1 .parameter .value input";
            var target = WaitForCss(selector);
            Assert.AreEqual("* (drop here)", target.GetAttribute("placeholder"));

            PasteIntoReferenceField("#pane1 .parameter .value.droppable");
            //Test that color has changed
            WaitForCss(selector + ".link-color5");
        }

        public virtual void PasteAnImplementationOfAnInterface() {
            GeminiUrl("object/object?i1=View&o1=___1.Employee--88&i2=View&o2=___1.Employee--203&as1=open&d1=SpecifyManager&f1_manager=null");
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Ken Myer", title.Text);
            title.Click();
            CopyToClipboard(title);
            string selector = "#pane1 .parameter .value input";
            var target = WaitForCss(selector);
            Assert.AreEqual("* (drop here)", target.GetAttribute("placeholder"));
            PasteIntoReferenceField("#pane1 .parameter .value.droppable");
        }

        public virtual void PasteIntoAutoCompleteField() {
            GeminiUrl("home/object?m1=CustomerRepository&i2=View&o2=___1.Customer--29929&d1=FindCustomer&f1_customer=null");
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Many Bikes Store, AW00029929", title.Text);
            title.Click();
            CopyToClipboard(title);
            string selector = "#pane1 .parameter .value";
            WaitForCss(selector);
            PasteIntoInputField("#pane1 .parameter .value.droppable");
        }

        public virtual void DroppableReferenceFieldWithoutAutoComplete() {
            GeminiUrl("object?o1=___1.PurchaseOrderHeader--121");
            GetReferenceProperty("Order Placed By", "Sheela Word");
            EditObject();
            wait.Until(dr => dr.FindElements(By.CssSelector(".property")).
                    Single(we => we.FindElement(By.CssSelector(".name")).Text == "Order Placed By" + ":" &&
                                 we.FindElement(By.CssSelector(".value.droppable")).GetAttribute("value") == "Sheela Word")
            );
            //Finish somewhere else
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
        }

        public virtual void CannotPasteWrongTypeIntoReferenceField() {
            GeminiUrl("object/object?o1=___1.PurchaseOrderHeader--1372&i1=Edit&o2=___1.Product--771");
            WaitForView(Pane.Left, PaneType.Object);
            //CancelDatePicker("#orderdate1");
            var fieldCss = "#pane1 .property:nth-child(4) .value.droppable";
            Assert.AreEqual("Annette Hill", WaitForCss(fieldCss).GetAttribute("value"));
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Mountain-100 Silver, 38", title.Text);
            title.Click();
            CopyToClipboard(title);

            var target = WaitForCss(fieldCss);
            WaitForCss(".footer .currentcopy .reference");
            target.Click();
            target.SendKeys(Keys.Control + "v");
            Assert.AreEqual("Annette Hill", WaitForCss(fieldCss).GetAttribute("value")); //i.e. no change
            //Finish somewhere else
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
        }

        public virtual void DroppingRefIntoDialogIsKeptWhenRightPaneIsClosed() {
            GeminiUrl("home/object?m1=EmployeeRepository&d1=CreateNewEmployeeFromContact&f1_contactDetails=null&o2=___1.Person--10895");
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Arthur Kapoor", title.Text);
            title.Click();
            CopyToClipboard(title);

            string selector = "#pane1 .parameter .value";
            var target = WaitForCss(selector);
           
            // this has changed with new autocomplete 
            //Assert.AreEqual("", target.Text);

            PasteIntoInputField("#pane1 .parameter .value.droppable");
            Click(FullIcon());
            WaitUntilGone(d => d.FindElement(By.CssSelector("#pane2")));
            var input = WaitForCss("#contactdetails1.droppable");
            Assert.AreEqual("Arthur Kapoor", input.GetAttribute("value"));
            CancelDialog();
        }
    }

    public abstract class CopyAndPasteTests : CopyAndPasteTestsRoot {
        [TestMethod]
        public override void CopyTitleOrPropertyIntoClipboard() {
            base.CopyTitleOrPropertyIntoClipboard();
        }

        [TestMethod]
        public override void CopyListItemIntoClipboard() {
            base.CopyListItemIntoClipboard();
        }

        [TestMethod]
        public override void PasteIntoReferenceField() {
            base.PasteIntoReferenceField();
        }

        [TestMethod]
        public override void PasteIntoReferenceFieldThatAlsoHasAutoCompleteAndFindMenu() {
            base.PasteIntoReferenceFieldThatAlsoHasAutoCompleteAndFindMenu();
        }

        [TestMethod]
        public override void PasteIntoDialog() {
            base.PasteIntoDialog();
        }

        [TestMethod]
        public override void PasteIntoAutoCompleteField() {
            base.PasteIntoAutoCompleteField();
        }

        [TestMethod]
        public override void PasteAnImplementationOfAnInterface() {
            base.PasteAnImplementationOfAnInterface();
        }

        [TestMethod]
        public override void DroppableReferenceFieldWithoutAutoComplete() {
            base.DroppableReferenceFieldWithoutAutoComplete();
        }

        [TestMethod]
        public override void CannotPasteWrongTypeIntoReferenceField() {
            base.CannotPasteWrongTypeIntoReferenceField();
        }

        [TestMethod]
        public override void DroppingRefIntoDialogIsKeptWhenRightPaneIsClosed() {
            base.DroppingRefIntoDialogIsKeptWhenRightPaneIsClosed();
        }
    }

    #region browsers specific subclasses

    //[TestClass]
    public class CopyAndPasteTestsIe : CopyAndPasteTests {
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

    //[TestClass] //Firefox Individual
    public class CopyAndPasteTestsFirefox : CopyAndPasteTests {
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

        protected override void ScrollTo(IWebElement element) {
            string script = $"window.scrollTo({element.Location.X}, {element.Location.Y});return true;";
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    //[TestClass]
    public class CopyAndPasteTestsChrome : CopyAndPasteTests {
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
    }

    #endregion

    #region Mega tests

    public abstract class MegaCopyAndPasteTestsRoot : CopyAndPasteTestsRoot {
        [TestMethod] //Mega
        public void MegaCopyAndPasteTest() {
            CopyTitleOrPropertyIntoClipboard();
            CopyListItemIntoClipboard();
            PasteIntoReferenceField();
            PasteIntoReferenceFieldThatAlsoHasAutoCompleteAndFindMenu();
            PasteIntoDialog();
            PasteAnImplementationOfAnInterface();
            PasteIntoAutoCompleteField();
            DroppableReferenceFieldWithoutAutoComplete();
            CannotPasteWrongTypeIntoReferenceField();
            DroppingRefIntoDialogIsKeptWhenRightPaneIsClosed();

            // Moved to LocallRunTests as fail on server 
            //CanClearADroppableReferenceField();
            //IfNoObjectInClipboardCtrlVRevertsToBrowserBehaviour(); 
        }
    }

    //[TestClass]
    public class MegaCopyAndPasteTestsFirefox : MegaCopyAndPasteTestsRoot {
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

    [TestClass]
    public class MegaCopyAndPasteTestsChrome : MegaCopyAndPasteTestsRoot {
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

    //[TestClass]
    public class MegaCopyAndPasteTestsIe : MegaCopyAndPasteTestsRoot {
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

    #endregion
}