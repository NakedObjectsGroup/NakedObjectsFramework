using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Web.UnitTests.Selenium;
using OpenQA.Selenium.Firefox;

namespace NakedObjects.Mvc.Selenium.Test.Firefox {
    [TestClass]
    public class AttributeTestsFirefox : AttributeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            wait = new SafeWebDriverWait(br, DefaultTimeOut);
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public override void CleanUpTest() {
            base.CleanUpTest();
        }

        [TestMethod]
        public override void PasswordIsObscuredInAnEntryField() {
            DoPasswordIsObscuredInAnEntryField();
        }

        [TestMethod]
        public override void MultiLineInViewMode() {
            DoMultiLineInViewMode();
        }

        [TestMethod]
        public override void MultiLineInEditMode() {
            DoMultiLineInEditMode();
        }
    }
}