// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
    public abstract class ExceptionTests : AWWebTest {
        public abstract void DomainException();

        public void DoDomainException() {
            Login();
            br.ClickAction("CustomerRepository-ThrowDomainException");
            string text = br.FindElement(By.ClassName("error")).FindElement(By.TagName("h2")).Text;
            Assert.AreEqual("An exception was thrown within the application code", text);
        }
    }

    [TestClass, Ignore]
    public class ExceptionTestsIE : ExceptionTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            AWWebTest.InitialiseClass(context);
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
        public override void DomainException() {
            DoDomainException();
        }
    }

    [TestClass]
    public class ExceptionTestsFirefox : ExceptionTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            br.Navigate().GoToUrl(url);
        }


        [TestMethod]
        public override void DomainException() {
            DoDomainException();
        }
    }

    [TestClass]
    public class ExceptionTestsChrome : ExceptionTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            AWWebTest.InitialiseClass(context);
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
        public override void DomainException() {
            DoDomainException();
        }
    }
}