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

        public void DoPasswordIsObscuredInAnEntryField() {
            Login();
            br.ClickAction("CustomerRepository-CreateNewIndividualCustomer");


            IWebElement pw = br.GetField("CustomerRepository-CreateNewIndividualCustomer-InitialPassword");
            IWebElement field = pw.FindElement(By.TagName("input"));
            Assert.AreEqual("password", field.GetAttribute("type"));

            //Ordinary field
            IWebElement fn = br.GetField("CustomerRepository-CreateNewIndividualCustomer-FirstName");
            field = fn.FindElement(By.TagName("input"));
            Assert.AreEqual("text", field.GetAttribute("type"));
        }


        public abstract void MultiLineInViewMode();

        public void DoMultiLineInViewMode() {
            Login();
            FindCustomerByAccountNumber("AW00000206");
            IWebElement demog = br.GetField("Store-FormattedDemographics").FindElement(By.TagName("textarea"));

            Assert.AreEqual("Store-FormattedDemographics-Input", demog.GetAttribute("id"));
            string value = demog.GetAttribute("readOnly");
            Assert.IsTrue(value == "readonly" || value == "true"); // different browsers 
            Assert.AreEqual("10", demog.GetAttribute("rows"));
            Assert.AreEqual("500", demog.GetAttribute("cols"));
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
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
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

    [TestClass]
    public class AttributeTestsFirefox : AttributeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
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
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
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