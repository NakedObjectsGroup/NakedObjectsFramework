// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
    // 3. Make MyTestsIE implement the abstract MyTests class.  Each created method should be
    //    annotated [TestMethod] and simply delegate to the inherited 'Do' method.
    //    The following Regex will do this automatically:
    //    Find:     ^{.*}public override void {.*}\n{.*\{}\n{.*}{throw .*}$
    //    Replace:  \1\[TestMethod\]\n\1public override void \2\n\3\n\4Do\2;
    // 4. When IE tests run OK, uncomment the Firefox and/or Chrome classes and repeat step 3
    public abstract class EnumTests : AWWebTest {
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        public abstract void ViewEnumProperty();

        protected void DoViewEnumProperty() {
            Login();
            FindOrder("SO67861");
            br.AssertPageTitleEquals("SO67861");
            IWebElement status = br.GetField("SalesOrderHeader-Status");
            status.AssertValueEquals("Shipped");
        }

        public abstract void EditEnumProperty();

        protected void DoEditEnumProperty() {
            Login();
            FindOrder("SO67862");
            br.AssertPageTitleEquals("SO67862");
            br.ClickEdit();
            IWebElement status = br.GetField("SalesOrderHeader-Status");
            status.SelectDropDownItem("Cancelled", br);
            ////Must adjust due date or else save fails
            //IWebElement due = br.GetField("SalesOrderHeader-ShipDate");
            //string tomorrow = DateTime.Today.ToShortDateString();
            //due.TypeText(tomorrow, br);

            br.ClickSave();
            status = br.GetField("SalesOrderHeader-Status");
            status.AssertValueEquals("Cancelled");
        }
    }

    [TestClass]
    public class EnumTestsIE : EnumTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            EnumTests.InitialiseClass(context);
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
        public override void ViewEnumProperty() {
            DoViewEnumProperty();
        }

        [TestMethod] //This one seems to cause a lot of failures on the server
        public override void EditEnumProperty() {
            DoEditEnumProperty();
        }
    }

    [TestClass]
    public class EnumTestsFirefox : EnumTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            MyTests.InitialiseClass(context);
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
        public override void ViewEnumProperty() {
            DoViewEnumProperty();
        }

        [TestMethod] //This one seems to cause a lot of failures on the server
        public override void EditEnumProperty() {
            DoEditEnumProperty();
        }
    }

    [TestClass]
    public class EnumTestsChrome : EnumTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            MyTests.InitialiseClass(context);
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
        public override void ViewEnumProperty() {
            DoViewEnumProperty();
        }

        [TestMethod] //This one seems to cause a lot of failures on the server
        public override void EditEnumProperty() {
            DoEditEnumProperty();
        }
    }
}