// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {
    [TestClass]
    public abstract class ServicePageTests : SpiroTest {
        [TestMethod]
        public virtual void MenuBar() {
            br.Navigate().GoToUrl(CustomerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("app-bar")).Count == 1);

            Assert.IsTrue(br.FindElement(By.ClassName("home")).Displayed);
            Assert.IsTrue(br.FindElement(By.ClassName("back")).Displayed);
            Assert.IsTrue(br.FindElement(By.ClassName("forward")).Displayed);
            Assert.IsFalse(br.FindElement(By.ClassName("refresh")).Displayed);
            Assert.IsFalse(br.FindElement(By.ClassName("edit")).Displayed);
            Assert.IsFalse(br.FindElement(By.ClassName("help")).Displayed);
        }

        [TestMethod]
        public virtual void Actions() {
            br.Navigate().GoToUrl(CustomerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.ClassName("action"));

            Assert.AreEqual("Find Customer By Account Number", actions[0].Text);
            Assert.AreEqual("Customer Dashboard", actions[1].Text);
            Assert.AreEqual("Throw Domain Exception", actions[2].Text);
            Assert.AreEqual("Find Individual Customer By Name", actions[3].Text);
            Assert.AreEqual("Create New Individual Customer", actions[4].Text);
            Assert.AreEqual("Random Individual", actions[5].Text);          
            Assert.AreEqual("Find Store By Name", actions[6].Text);
            Assert.AreEqual("Create New Store Customer", actions[7].Text);
            Assert.AreEqual("Random Store", actions[8].Text);            
        }

        [TestMethod]
        public virtual void DialogAction() {
            br.Navigate().GoToUrl(CustomerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.ClassName("action"));

            // click on action to open dialog 
            Click(actions[0]); // Find customer by account number

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find Customer By Account Number", title);

            // cancel dialog 
            Click(br.FindElement(By.CssSelector("div.action-dialog  .cancel")));

            wait.Until(d => {
                try {
                    br.FindElement(By.ClassName("action-dialog"));
                    return false;
                }
                catch (NoSuchElementException) {
                    return true;
                }
            });
        }

        [TestMethod]
        public virtual void DialogActionShow() {
            br.Navigate().GoToUrl(CustomerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);

            var showObject = new Action(() => {
                // click on action to open dialog 
                Click(br.FindElements(By.ClassName("action"))[0]); // Find customer by account number

                wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
                string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

                Assert.AreEqual("Find Customer By Account Number", title);

                br.FindElement(By.CssSelector(".parameter-value  input")).SendKeys("00022262");

                Click(br.FindElement(By.ClassName("show")));

                wait.Until(d => d.FindElement(By.ClassName("nested-object")));
            });

            var cancelObject = new Action(() => {
                // cancel object 
                Click(br.FindElement(By.CssSelector("div.nested-object .cancel")));

                wait.Until(d => {
                    try {
                        br.FindElement(By.ClassName("nested-object"));
                        return false;
                    }
                    catch (NoSuchElementException) {
                        return true;
                    }
                });
            });

            var cancelDialog = new Action(() => {
                Click(br.FindElement(By.CssSelector("div.action-dialog  .cancel")));

                wait.Until(d => {
                    try {
                        br.FindElement(By.ClassName("action-dialog"));
                        return false;
                    }
                    catch (NoSuchElementException) {
                        return true;
                    }
                });
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
            br.Navigate().GoToUrl(CustomerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[0]); // Find customer by account number

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find Customer By Account Number", title);

            br.FindElement(By.CssSelector(".parameter-value  input")).SendKeys("00022262");

            Click(br.FindElement(By.ClassName("go")));

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            // dialog should be closed

            wait.Until(d => {
                try {
                    br.FindElement(By.ClassName("action-dialog"));
                    return false;
                }
                catch (NoSuchElementException) {
                    return true;
                }
            });
        }


        [TestMethod, Ignore]
        public virtual void ObjectAction() {
            br.Navigate().GoToUrl(CustomerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);

            IWebElement action = br.FindElements(By.ClassName("action"))[8];

            // click on action to get object 
            Click(action); // random store 

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            // cancel object 
            Click(br.FindElement(By.CssSelector("div.nested-object .cancel")));

            wait.Until(d => {
                try {
                    br.FindElement(By.ClassName("nested-object"));
                    return false;
                }
                catch (NoSuchElementException) {
                    return true;
                }
            });
        }

        [TestMethod, Ignore]
        public virtual void ObjectActionExpand() {
            br.Navigate().GoToUrl(CustomerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);
            IWebElement action = br.FindElements(By.ClassName("action"))[8];

            // click on action to get object 
            Click(action); // random store 

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            // expand object
            Click(br.FindElement(By.CssSelector("div.nested-object .expand")));

            wait.Until(d => br.FindElement(By.ClassName("object-properties")));
        }

        [TestMethod]
        public virtual void CollectionAction() {
            br.Navigate().GoToUrl(OrderServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == OrderServiceActions);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.ClassName("action"));

            // click on action to get collection 
            Click(actions[2]); // highest value orders

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            // cancel collection 
            Click(br.FindElement(By.CssSelector("div.list-view .cancel")));

            wait.Until(d => {
                try {
                    br.FindElement(By.ClassName("list-view"));
                    return false;
                }
                catch (NoSuchElementException) {
                    return true;
                }
            });
        }

        [TestMethod]
        public virtual void CollectionActionSelectItem() {
            br.Navigate().GoToUrl(OrderServiceUrl);


            var selectItem = new Action(() => {
                wait.Until(d => d.FindElements(By.ClassName("action")).Count == OrderServiceActions);
                ReadOnlyCollection<IWebElement> actions = br.FindElements(By.ClassName("action"));

                // click on action to get object 
                Click(actions[2]); // highest value orders

                wait.Until(d => d.FindElement(By.ClassName("list-view")));

                // select item
                Click(br.FindElement(By.CssSelector("div.list-item a")));

                wait.Until(d => br.FindElement(By.ClassName("nested-object")));
            });

            // cancel object 

            var cancelObject = new Action(() => {
                Click(br.FindElement(By.CssSelector("div.nested-object .cancel")));
                wait.Until(d => {
                    try {
                        br.FindElement(By.ClassName("nested-object"));
                        return false;
                    }
                    catch (NoSuchElementException) {
                        return true;
                    }
                });
            });

            // cancel collection 
            var cancelCollection = new Action(() => {
                Click(br.FindElement(By.CssSelector("div.list-view .cancel")));

                wait.Until(d => {
                    try {
                        br.FindElement(By.ClassName("list-view"));
                        return false;
                    }
                    catch (NoSuchElementException) {
                        return true;
                    }
                });
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
            br.Navigate().GoToUrl(OrderServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == OrderServiceActions);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.ClassName("action"));

            // click on action to get object 
            Click(actions[2]); // highest value orders

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            // select item
            Click(br.FindElement(By.CssSelector("div.list-item a")));

            wait.Until(d => br.FindElement(By.ClassName("nested-object")));

            // expand object
            Click(br.FindElement(By.CssSelector("div.nested-object .expand")));

            wait.Until(d => br.FindElement(By.ClassName("object-properties")));
        }
    }

    #region browsers specific subclasses

    [TestClass, Ignore]
    public class ServicePageTestsIe : ServicePageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            SpiroTest.InitialiseClass(context);
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
    public class ServicePageTestsFirefox : ServicePageTests {
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
    }

    [TestClass, Ignore]
    public class ServicePageTestsChrome : ServicePageTests {
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

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}