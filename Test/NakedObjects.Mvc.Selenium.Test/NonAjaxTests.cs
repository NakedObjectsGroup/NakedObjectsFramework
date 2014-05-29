// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
    public abstract class NonAjaxTests : AWWebTest {
        protected new const string url = "http://mvc.nakedobjects.net:1081/unittest";

        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }


        public abstract void ClientSideValidation();

        public void DoClientSideValidation() {
            Login();
            FindProduct("LW-1000");
            br.ClickEdit();
            IWebElement days = br.GetField("Product-DaysToManufacture");
            days.AssertInputValueEquals("0");
            days.TypeText("100", br);
            days.AppendText(Keys.Tab, br);
            br.WaitForAjaxComplete();
            IWebElement valMsg = days.FindElement(By.ClassName("field-validation-error"));
            Assert.AreEqual("Value is outside the range 1 to 90", valMsg.Text);
        }
    }

    [TestClass, Ignore]
    public class NonAjaxTestsIE : NonAjaxTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            AjaxTests.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new InternetExplorerDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }


        [TestMethod]
        public override void ClientSideValidation() {
            DoClientSideValidation();
        }
    }

    [TestClass, Ignore]
    public class NonAjaxTestsFirefox : NonAjaxTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AjaxTests.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }


        [TestMethod]
        public override void ClientSideValidation() {
            DoClientSideValidation();
        }
    }

    [TestClass, Ignore]
    public class NonAjaxTestsChrome : NonAjaxTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            AjaxTests.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = InitChromeDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }


        [TestMethod]
        public override void ClientSideValidation() {
            DoClientSideValidation();
        }
    }
}