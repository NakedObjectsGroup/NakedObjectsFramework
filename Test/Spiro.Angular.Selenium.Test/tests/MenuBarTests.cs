// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {
    [TestClass]
    public abstract class MenuBarTests : SpiroTest {
        [TestMethod]
        public virtual void Home() {
            br.Navigate().GoToUrl(customerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);
            Click(br.FindElement(By.ClassName("home")));
            wait.Until(d => d.FindElements(By.ClassName("service")).Count == 12);
        }

        [TestMethod]
        public virtual void BackAndForward() {
            br.Navigate().GoToUrl(url);
            wait.Until(d => d.FindElements(By.ClassName("service")).Count == 12);
            GoToServiceFromHomePage("Customers");
            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);
            Click(br.FindElement(By.ClassName("back")));
            wait.Until(d => d.FindElements(By.ClassName("service")).Count == 12);
            Click(br.FindElement(By.ClassName("forward")));
            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);
        }


    }

    #region browsers specific subclasses 

    [TestClass, Ignore]
    public class MenuBarTestsIe : MenuBarTests {
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
    public class MenuBarTestsFirefox : MenuBarTests {
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
    public class MenuBarTestsChrome : MenuBarTests {
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
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}