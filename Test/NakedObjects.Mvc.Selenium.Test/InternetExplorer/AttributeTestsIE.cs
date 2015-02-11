using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Web.UnitTests.Selenium;
using OpenQA.Selenium.IE;

namespace NakedObjects.Mvc.Selenium.Test.InternetExplorer {
    [TestClass]
    public class AttributeTestsIE : AttributeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new InternetExplorerDriver();
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