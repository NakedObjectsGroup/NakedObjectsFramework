// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium
{

    public abstract class CopyAndPasteTests : AWTest
    {
        [TestMethod]
        public virtual void CopyTitleOrPropertyIntoClipboard()
        {
            GeminiUrl("object/object?object1=AdventureWorksModel.Product-990&object2=AdventureWorksModel.Customer-652");
            WaitForView(Pane.Left, PaneType.Object, "Mountain-500 Black, 42");
            WaitForView(Pane.Right, PaneType.Object, "Selected Distributors, AW00000652");

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
            target = Tab(3);
            Assert.AreEqual("Southeast", target.Text);
            CopyToClipboard(target);
        }
        [TestMethod]
        public virtual void CopyListItemIntoClipboard()
        {
            GeminiUrl("list/list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&page1=1&pageSize1=20&menu2=PersonRepository&action2=ValidCountries&page2=1&pageSize2=20");
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

        [TestMethod]
        public virtual void PasteIntoReferenceField()
        {
            GeminiUrl("object/object?object1=AdventureWorksModel.PurchaseOrderHeader-1372&edit1=true&object2=AdventureWorksModel.Employee-161");
            WaitForView(Pane.Left, PaneType.Object, "Editing - 07/01/2008 00:00:00");
            Assert.AreEqual("Annette Hill", WaitForCss("#pane1 .property:nth-child(4) .value.droppable").Text);
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Kirk Koenigsbauer", title.Text);
            title.Click();
            CopyToClipboard(title);
            PasteIntoReferenceField("#pane1 .property:nth-child(4) .value.droppable");
        }

        [TestMethod]
        public virtual void PasteIntoReferenceFieldThatAlsoHasAutoCompleteAndFindMenu()
        {
            GeminiUrl("object/object?object2=AdventureWorksModel.SalesPerson-284&object1=AdventureWorksModel.Store-740&edit1=true");
            WaitForView(Pane.Left, PaneType.Object, "Editing - Touring Services");
            Assert.AreEqual("Tsvi Reiter", WaitForCss("#pane1 input#salesperson").GetAttribute("value"));
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Tete Mensa-Annan", title.Text);
            title.Click();
            CopyToClipboard(title);
            PasteIntoInputField("#pane1 input#salesperson");
            //Now check that Auto-complete is working
            WaitForCss("#pane1 input#salesperson").Clear();
            TypeIntoField("#pane1 input#salesperson", "Ito");
            wait.Until(d => d.FindElement(By.CssSelector(".ui-menu-item")));
            Click(WaitForCss(".ui-menu-item"));
            wait.Until(dr => dr.FindElement(By.CssSelector("#pane1 input#salesperson")).GetAttribute("value") == "Shu Ito");
        }

        [TestMethod] 
        public virtual void PasteIntoDialog()
        {
            GeminiUrl("home/object?menu1=SalesRepository&dialog1=CreateNewSalesPerson&object2=AdventureWorksModel.Employee-206");
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Stuart Munson", title.Text);
            title.Click();
            CopyToClipboard(title);
            string selector = "#pane1 .parameter .value";
            var target = WaitForCss(selector);
            Assert.AreEqual("", target.Text);

            PasteIntoReferenceField("#pane1 .parameter .value.droppable");
        }

        [TestMethod]
        public void DroppableReferenceFieldWithoutAutoComplete()
        {
            GeminiUrl("object?object1=AdventureWorksModel.PurchaseOrderHeader-121");
            GetReferenceProperty("Order Placed By", "Sheela Word");
            EditObject();
            var prop = wait.Until(dr => dr.FindElements(By.CssSelector(".property"))
        .Where(we => we.FindElement(By.CssSelector(".name")).Text == "Order Placed By" + ":" &&
        we.FindElement(By.CssSelector(".value.droppable")).Text == "Sheela Word").Single()
);
        }

        [TestMethod]
        public virtual void CannotPasteWrongTypeIntoReferenceField()
        {
            GeminiUrl("object/object?object1=AdventureWorksModel.PurchaseOrderHeader-1372&edit1=true&object2=AdventureWorksModel.Product-771");
            WaitForView(Pane.Left, PaneType.Object, "Editing - 07/01/2008 00:00:00");
            var fieldCss = "#pane1 .property:nth-child(4) .value.droppable";
            Assert.AreEqual("Annette Hill", WaitForCss(fieldCss).Text);
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("Mountain-100 Silver, 38", title.Text);
            title.Click();
            CopyToClipboard(title);

            var target = WaitForCss(fieldCss);
            var copying = WaitForCss(".footer .currentcopy .reference").Text;
            target.Click();
            target.SendKeys(Keys.Control + "v");
            Assert.AreEqual("Annette Hill", WaitForCss(fieldCss).Text); //i.e. no change
        }

        [TestMethod]
        public virtual void CanClearADroppableReferenceField()
        {
            GeminiUrl("object?object1=AdventureWorksModel.PurchaseOrderHeader-561&edit1=true");
            WaitForView(Pane.Single, PaneType.Object, "Editing - 15/09/2007 00:00:00");
            var fieldCss = ".property:nth-child(4) .value.droppable";
            var field = WaitForCss(fieldCss);
            Assert.AreEqual("Ben Miller", field.Text);
            field.SendKeys(Keys.Delete);
            Assert.AreEqual("", WaitForCss(fieldCss).Text);

        }
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class CopyAndPasteTestsIe : CopyAndPasteTests
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
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    //[TestClass]
    public class CopyAndPasteTestsFirefox : CopyAndPasteTests
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

        protected override void ScrollTo(IWebElement element)
        {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }
    }

    //[TestClass, Ignore]
    public class CopyAndPasteTestsChrome : CopyAndPasteTests
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
    }

    #endregion
}