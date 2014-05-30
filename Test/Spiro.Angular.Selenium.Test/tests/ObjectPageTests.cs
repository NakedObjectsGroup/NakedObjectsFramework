// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {
    [TestClass]
    public abstract class ObjectPageTests : SpiroTest {
        [TestMethod]
        public virtual void MenuBar() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("app-bar")).Count == 1);

            Assert.IsTrue(br.FindElement(By.ClassName("home")).Displayed);
            Assert.IsTrue(br.FindElement(By.ClassName("back")).Displayed);
            Assert.IsTrue(br.FindElement(By.ClassName("forward")).Displayed);
            Assert.IsFalse(br.FindElement(By.ClassName("refresh")).Displayed);
           
            Assert.IsFalse(br.FindElement(By.ClassName("help")).Displayed);

            wait.Until(d => d.FindElement(By.ClassName("edit")).Displayed);

        }

        [TestMethod]
        public virtual void Actions() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.ClassName("action"));

            Assert.AreEqual("Create New Address", actions[0].Text);
            Assert.AreEqual("Create New Contact", actions[1].Text);
            Assert.AreEqual("Create New Order", actions[2].Text);
            Assert.AreEqual("Quick Order", actions[3].Text);
            Assert.AreEqual("Search For Orders", actions[4].Text);
            Assert.AreEqual("Last Order", actions[5].Text);
            Assert.AreEqual("Open Orders", actions[6].Text);
            Assert.AreEqual("Recent Orders", actions[7].Text);
        }

        [TestMethod]
        public virtual void Properties() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("property")).Count == 8);
            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.ClassName("property"));

            Assert.AreEqual("Store Name:\r\nTwin Cycles", properties[0].Text);
            Assert.AreEqual("Demographics:\r\nAnnualSales: 800000 AnnualRevenue: 80000 BankName: International Security BusinessType: BM YearOpened: 1988 Specialty: Touring SquareFeet: 21000 Brands: AW Internet: T1 NumberEmployees: 11", properties[1].Text);
            Assert.AreEqual("Sales Person:\r\nLynn Tsoflias", properties[2].Text);
            Assert.AreEqual("Last Modified:\r\n2004-10-13T10:15:07.497Z", properties[3].Text);
            Assert.AreEqual("Account Number:\r\nAW00000555", properties[4].Text);
            Assert.AreEqual("Sales Territory:\r\nAustralia", properties[5].Text);
            Assert.AreEqual("Addresses:\r\n1-Customer Addresses", properties[6].Text);
            Assert.AreEqual("Contacts:\r\n1-Store Contacts", properties[7].Text);
        }

        [TestMethod]
        public virtual void ClickReferenceProperty() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("property")).Count == 8);
            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector("div.property a"));

            Click(properties[0]);

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            // cancel object 
            Click(br.FindElement(By.CssSelector("div.nested-object .cancel")));

            WaitUntilGone(d => d.FindElement(By.ClassName("nested-object")));
        }

        [TestMethod]
        public virtual void ClickCollectionProperty() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("property")).Count == 8);
            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector("div.property a"));

            Click(properties[3]);

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            // cancel object 
            Click(br.FindElement(By.CssSelector("div.list-view .cancel")));

            WaitUntilGone(d => d.FindElement(By.ClassName("list-view")));
        }

        [TestMethod]
        public virtual void AttachmentProperty() {
            br.Navigate().GoToUrl(product968Url);

            wait.Until(d => d.FindElements(By.ClassName("property")).Count == 25);
            wait.Until(d => d.FindElements(By.CssSelector("div.property  a > img")).Count == 1);

            Assert.AreEqual(25053, br.FindElements(By.CssSelector("div.property  a > img")).Single().GetAttribute("src").Length, "expect data in data uri"); 
        }


        [TestMethod]
        public virtual void DialogAction() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.CssSelector("div.action-button a"));

            // click on action to open dialog 
            Click(actions[4]); // Search For Orders

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Search For Orders", title);

            // cancel dialog 
            Click(br.FindElement(By.CssSelector("div.action-dialog  .cancel")));

            WaitUntilGone(d => d.FindElement(By.ClassName("action-dialog")));
        }

        [TestMethod]
        public virtual void DialogActionShow() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);

            var showObject = new Action(() => {
                // click on action to open dialog 
                Click(br.FindElements(By.CssSelector("div.action-button a"))[4]); // Search for orders

                wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
                string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

                Assert.AreEqual("Search For Orders", title);

                br.FindElements(By.CssSelector(".parameter-value input"))[0].SendKeys("1 Jan 2003");
                br.FindElements(By.CssSelector(".parameter-value input"))[1].SendKeys("1 Dec 2003" + Keys.Escape);

                Thread.Sleep(2000); // need to wait for datepicker :-(

                wait.Until(d => br.FindElement(By.ClassName("show")));

                Click(br.FindElement(By.ClassName("show")));

                wait.Until(d => d.FindElement(By.ClassName("list-view")));
            });

            var cancelObject = new Action(() => {
                // cancel object 
                Click(br.FindElement(By.CssSelector("div.list-view .cancel")));

                WaitUntilGone(d => d.FindElement(By.ClassName("list-view")));
            });

            var cancelDialog = new Action(() => {
                Click(br.FindElement(By.CssSelector("div.action-dialog  .cancel")));

                WaitUntilGone(d => d.FindElement(By.ClassName("action-dialog")));
            });

            showObject();
            cancelObject();
            cancelDialog();

            showObject();
            cancelDialog();
            cancelObject();
        }

        [TestMethod]
        public virtual void DialogActionGo() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);

            // click on action to open dialog 
            Click(br.FindElements(By.CssSelector("div.action-button a"))[4]); // Fisearch for orders

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Search For Orders", title);

            br.FindElements(By.CssSelector(".parameter-value input"))[0].SendKeys("1 Jan 2003");
            br.FindElements(By.CssSelector(".parameter-value input"))[1].SendKeys("1 Dec 2003" + Keys.Escape);

            Thread.Sleep(2000); // need to wait for datepicker :-(

            wait.Until(d => br.FindElement(By.ClassName("go")));

            Click(br.FindElement(By.ClassName("go")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            // dialog should be closed

            WaitUntilGone(d => d.FindElement(By.ClassName("action-dialog")));
        }


        [TestMethod]
        public virtual void ObjectAction() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);

            IWebElement action = br.FindElements(By.CssSelector("div.action-button a"))[5];

            // click on action to get object 
            Click(action); // last order

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            // cancel object 
            Click(br.FindElement(By.CssSelector("div.nested-object .cancel")));

            WaitUntilGone(d => d.FindElement(By.ClassName("nested-object")));
        }

        [TestMethod]
        public virtual void ObjectActionExpand() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);
            IWebElement action = br.FindElements(By.ClassName("action"))[5];

            // click on action to get object 
            Click(action); //last order

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            // expand object
            Click(br.FindElement(By.CssSelector("div.nested-object .expand")));

            wait.Until(d => br.FindElement(By.ClassName("object-properties")));
        }

        [TestMethod]
        public virtual void CollectionAction() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.CssSelector("div.action-button a"));

            // click on action to get collection 
            Click(actions[7]); // recent orders

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            // cancel collection 
            Click(br.FindElement(By.CssSelector("div.list-view .cancel")));

            WaitUntilGone(d => d.FindElement(By.ClassName("list-view")));
        }

        [TestMethod]
        public virtual void CollectionActionSelectItem() {
            br.Navigate().GoToUrl(store555Url);

            var selectItem = new Action(() => {
                wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);
                ReadOnlyCollection<IWebElement> actions = br.FindElements(By.CssSelector("div.action-button a"));

                // click on action to get object 
                Click(actions[7]); // recent orders

                wait.Until(d => d.FindElement(By.ClassName("list-view")));

                // select item
                Click(br.FindElement(By.CssSelector("div.list-item a")));

                wait.Until(d => br.FindElement(By.ClassName("nested-object")));
            });

            // cancel object 

            var cancelObject = new Action(() => {
                Click(br.FindElement(By.CssSelector("div.nested-object .cancel")));
                WaitUntilGone(d => d.FindElement(By.ClassName("nested-object")));
            });

            // cancel collection 
            var cancelCollection = new Action(() => {
                Click(br.FindElement(By.CssSelector("div.list-view .cancel")));

                WaitUntilGone(d => d.FindElement(By.ClassName("list-view")));
            });


            selectItem();
            cancelObject();
            cancelCollection();

            // repeat but first cancel collection 
            selectItem();
            cancelCollection();
            cancelObject();
        }

        [TestMethod]
        public virtual void CollectionActionSelectItemExpand() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.CssSelector("div.action-button a"));

            // click on action to get object 
            Click(actions[7]); //recent orders

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            // select item
            Click(br.FindElement(By.CssSelector("div.list-item a")));

            wait.Until(d => br.FindElement(By.ClassName("nested-object")));

            // expand object
            Click(br.FindElement(By.CssSelector("div.nested-object .expand")));

            wait.Until(d => br.FindElement(By.ClassName("object-properties")));
        }

        [TestMethod]
        public virtual void ObjectEdit() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);

            wait.Until(d => d.FindElement(By.ClassName("edit")).Displayed);

            Click(br.FindElement(By.ClassName("edit")));

            wait.Until(d => br.FindElement(By.ClassName("save")));
        }

    }

    #region browsers specific subclasses

    [TestClass, Ignore]
    public class ObjectPageTestsIe : ObjectPageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class ObjectPageTestsFirefox : ObjectPageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            SpiroTest.InitialiseClass(context);
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

    [TestClass, Ignore]
    public class ObjectPageTestsChrome : ObjectPageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            SpiroTest.InitialiseClass(context);
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