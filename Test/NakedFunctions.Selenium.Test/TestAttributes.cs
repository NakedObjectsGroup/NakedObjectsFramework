// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Selenium.Helpers.Tests;
using OpenQA.Selenium;
using System.Linq;

namespace NakedFunctions.Selenium.Test.FunctionTests
{

    [TestClass]
    public class TestAttributes : GeminiTest
    {
        protected override string BaseUrl => TestConfig.BaseFunctionalUrl;

        #region initialization
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitChromeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            CleanUpTest();
        }
        #endregion

        #region Helpers
        protected virtual void OpenMainMenu(string menuName)
        {
            ClickHomeButton();
            WaitForView(Pane.Single, PaneType.Home, "Home");
            string menuSelector = $"nof-menu-bar nof-action input[title=\"{menuName}\"";
            wait.Until(dr => dr.FindElement(By.CssSelector(menuSelector)));
            IWebElement menu = br.FindElement(By.CssSelector($"nof-menu-bar nof-action input[title={menuName}"));
            Click(menu);
        }
        protected void OpenMainMenuAction(string menuName, string actionName)
        {
            OpenMainMenu(menuName);
            string actionSelector = $"nof-action-list nof-action input[value=\"{actionName}\"]";
            Click(WaitForCss(actionSelector));
        }
        #endregion 

        [TestMethod]
        public void AllWorkingAttributes()
        {
            Bounded();
            DefaultValueInt();
            ValueRangeInt();
            Hidden();
            DescribedAsFunction();
            Mask();
            Named();
            Optionally();
            PageSize();
        }

       //[TestMethod]
        public void Bounded()
        {
            //Change Department Or Shift on Employee. Both params are of Bounded types
            GeminiUrl("object?i1=View&o1=AW.Types.Employee--7&c1_DepartmentHistory=Table&as1=open&d1=ChangeDepartmentOrShift");
            WaitForTextEquals(".title", "Dylan Miller");
            wait.Until(d => d.FindElements(By.CssSelector("#department1 option")).Count() >= 3);
            var options = br.FindElements(By.CssSelector("#department1 option")).Select(e => e.Text).ToArray();
            Assert.AreEqual("Engineering", options[0]);
            Assert.AreEqual("Tool Design", options[1]);
        }

        //[TestMethod]
        public void DefaultValueInt()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--9&as1=open&d1=EditQuantities");
            WaitForTextEquals(".title", "Road-650 Overstock");
            var minQty = WaitForCss("input#minqty1");
            Assert.AreEqual("1", minQty.GetAttribute("value"));
        }

        //[TestMethod]
        public void DefaultValueString()
        {
            //Not yet working. Use Find (Customer) By Account Number
        }

        //[TestMethod]
        public void DefaultValueDate()
        {
            //Not yet working. Use Special Offer - Edit Dates.
        }

        //[TestMethod]
        public void ValueRangeInt()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.Product--890&as1=open&d1=BestSpecialOffer");
            WaitForTextEquals(".title", "HL Touring Frame - Blue, 46");
            var qty = "input#quantity1";
            var val = WaitForCss("nof-edit-parameter .validation");
            Assert.AreEqual("", val.Text);
            ClearFieldThenType(qty, "1000");
            Assert.AreEqual("Value is outside the range 1 to 999", val.Text);
            ClearFieldThenType(qty, "10");
            Assert.AreEqual("", val.Text);
            ClearFieldThenType(qty, "1000");
            Assert.AreEqual("Value is outside the range 1 to 999", val.Text);
        }

        //[TestMethod]
        public void Hidden()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.Shift--1");
            WaitForTextEquals(".title", "Day");
            Assert.AreEqual("Name:", WaitForCssNo("nof-view-property .name", 0).Text);
            Assert.AreEqual("Start Time:", WaitForCssNo("nof-view-property .name", 1).Text);
            Assert.AreEqual("End Time:", WaitForCssNo("nof-view-property .name", 2).Text);
            Assert.AreEqual("Modified Date:", WaitForCssNo("nof-view-property .name", 3).Text);
            Assert.AreEqual(4, br.FindElements(By.CssSelector("nof-view-property")).Count);
            //i.e. no 'Shift ID' field showing
        }

        //[TestMethod]
        public void DescribedAsFunction()
        {
            GeminiUrl("home?m1=Sales_MenuFunctions");
            WaitForTextEquals(".title", "Home");
            var action1 = WaitForCssNo("nof-action-list nof-action input",0);
            Assert.AreEqual("Create New Sales Person", action1.GetAttribute("value"));
            Assert.AreEqual("... from an existing Employee", action1.GetAttribute("title"));
        }

        //[TestMethod]
        public void DescribedAsParameter()
        {
            //TODO
        }

        //[TestMethod]
        public void Mask()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.Product--497");
            WaitForTextEquals(".title", "Pinch Bolt");
            var prop4 = WaitForCssNo("nof-view-property",4);
            Assert.AreEqual("List Price:", prop4.FindElement(By.CssSelector(".name")).Text);
            Assert.AreEqual("£0.00", prop4.FindElement(By.CssSelector(".value")).Text);
            var prop17 = WaitForCssNo("nof-view-property", 17);
            Assert.AreEqual("Sell Start Date:", prop17.FindElement(By.CssSelector(".name")).Text);
            Assert.AreEqual("1 Jun 2002", prop17.FindElement(By.CssSelector(".value")).Text);
        }

        //[TestMethod]
        public void Named()
        {
            Home();
            var employee_menuFunctions = WaitForCssNo("nof-menu-bar nof-action", 0);
            Assert.AreEqual("Employees", employee_menuFunctions.GetAttribute("value"));

        }

        //[TestMethod]
        public void Optionally()
        {
            GeminiUrl("home?m1=Employee_MenuFunctions&d1=FindEmployeeByName");
            var lastName = WaitForCss("input#lastname1");
            Assert.AreEqual("*", lastName.GetAttribute("placeholder"));
            Assert.AreEqual("", lastName.GetAttribute("value"));
            var firstName = WaitForCss("input#firstname1");
            Assert.IsNull(firstName.GetAttribute("placeholder"));
            Assert.AreEqual("", firstName.GetAttribute("value"));
            Assert.AreEqual("Missing mandatory fields: Last Name; ",OKButton().GetAttribute("title"));
        }

        [TestMethod]
        public void PageSize()
        {

        }
}