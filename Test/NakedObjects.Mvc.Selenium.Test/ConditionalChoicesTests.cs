// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
    /// <summary>
    ///     Tests use of a drop down list that is dependent upon selection or entry made in another field.
    /// </summary>
    public abstract class ConditionalChoicesTests : AWWebTest {
        /// <summary>
        ///     Requires the user to attempt to Save in order to get conditional drop-downs updated.
        /// </summary>
        public abstract void TestWithoutRelyingOnAjax();

        public void DoTestWithoutRelyingOnAjax() {
            Login();
            FindCustomerByAccountNumber("AW00000546");
            br.AssertContainsObjectView();
            br.AssertPageTitleEquals("Field Trip Store, AW00000546");

            br.ClickAction("Store-CreateNewAddress");

            IWebElement country = br.GetField("Address-CountryRegion").AssertIsEmpty();

            IWebElement province = br.GetField("Address-StateProvince").AssertIsEmpty();

            country.SelectDropDownItem("Australia", br);

            br.ClickSave();

            province = br.GetField("Address-StateProvince").AssertIsEmpty();
            province.SelectDropDownItem("Queensland", br);

            br.ClickSave();

            country = br.GetField("Address-CountryRegion").AssertIsEmpty();
            country.SelectDropDownItem("United Kingdom", br);

            br.ClickSave();

            province = br.GetField("Address-StateProvince").AssertIsEmpty();
            province.SelectDropDownItem("England", br);

            br.ClickSave();
        }
    }

    [TestClass]
    public class ConditionalChoicesTestsIE : ConditionalChoicesTests {
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
        public override void TestWithoutRelyingOnAjax() {
            DoTestWithoutRelyingOnAjax();
        }
    }

    [TestClass]
    public class ConditionalChoicesTestsFirefox : ConditionalChoicesTests {
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
        public override void TestWithoutRelyingOnAjax() {
            DoTestWithoutRelyingOnAjax();
        }
    }

    [TestClass]
    public class ConditionalChoicesTestsChrome : ConditionalChoicesTests {
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
        public override void TestWithoutRelyingOnAjax() {
            DoTestWithoutRelyingOnAjax();
        }
    }
}