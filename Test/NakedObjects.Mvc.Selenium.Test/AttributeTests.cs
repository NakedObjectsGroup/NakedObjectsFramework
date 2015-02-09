// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
    public abstract class AttributeTests : AWWebTest {
        public abstract void PasswordIsObscuredInAnEntryField();


        private IWebElement ClickAndWait(string actionId, string fieldId) {
            IWebElement action = br.FindElement(By.Id(actionId));

            action.Click();

            IWebElement field = null;
            wait.Until(wd => (field = wd.FindElement(By.Id(fieldId))) != null);

            Assert.IsNotNull(field);

            return field; 
        }


        public void DoPasswordIsObscuredInAnEntryField() {
            Login();

            var field = ClickAndWait("CustomerRepository-CreateNewIndividualCustomer", "CustomerRepository-CreateNewIndividualCustomer-InitialPassword");
          
            IWebElement ip = field.FindElement(By.TagName("input"));
            Assert.AreEqual("password", ip.GetAttribute("type"));

            //Ordinary field
            IWebElement fn = br.FindElement(By.Id("CustomerRepository-CreateNewIndividualCustomer-FirstName"));
            field = fn.FindElement(By.TagName("input"));
            Assert.AreEqual("text", field.GetAttribute("type"));
        }


        public abstract void MultiLineInViewMode();

        public void DoMultiLineInViewMode() {
            Login();
            FindCustomerByAccountNumber("AW00000206");
            IWebElement demog = null;

            wait.Until(wd => (demog = wd.GetField("Store-FormattedDemographics").FindElement(By.CssSelector("div.multiline"))) != null);

            Assert.AreEqual("AnnualSales: 800000 AnnualRevenue: 80000 BankName: Primary International BusinessType: BM YearOpened: 1994 Specialty: Road SquareFeet: 20000 Brands: AW Internet: DSL NumberEmployees: 18", demog.Text);
        }

        public abstract void MultiLineInEditMode();

        public void DoMultiLineInEditMode() {
            Login();
            FindOrder("SO63557");
            br.ClickEdit();
            IWebElement comment = br.GetField("SalesOrderHeader-Comment").FindElement(By.TagName("textarea"));

            Assert.AreEqual("SalesOrderHeader-Comment-Input", comment.GetAttribute("id"));
            //Assert.AreEqual("false", comment.GetAttribute("readOnly")); // now fails FF and IE but not Chrome 
            Assert.AreEqual("3", comment.GetAttribute("rows"));
            Assert.AreEqual("50", comment.GetAttribute("cols"));

            comment.SendKeys("Line 1\nLine 2");
            br.ClickSave();
            string txt = br.GetField("SalesOrderHeader-Comment").FindElement(By.ClassName("nof-value")).Text;
            Assert.IsTrue(txt.StartsWith("Line 1"));
            Assert.IsTrue(txt.EndsWith("Line 2"));
            //char[] chars = txt.ToCharArray();
            //Assert.AreEqual('\n', chars[6]); unpredictable results on different browsers 
        }
    }

    [TestClass, Ignore]
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

    [TestClass, Ignore]
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

    [TestClass, Ignore]
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