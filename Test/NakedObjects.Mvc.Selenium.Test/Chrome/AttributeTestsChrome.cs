using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Web.UnitTests.Selenium;

namespace NakedObjects.Mvc.Selenium.Test.Chrome {
    [TestClass]
    public class AttributeTestsChrome : AttributeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = InitChromeDriver();
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