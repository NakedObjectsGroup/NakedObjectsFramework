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

namespace NakedFunctions.Selenium.Test.FunctionTests {

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

        //[TestMethod]
        public void AllAttributes()
        {
            Bounded();
            DefaultValueInt();
        }

        [TestMethod]
        public void Bounded()
        {
            //Change Department Or Shift on Employee. Both params are of Bounded types
            GeminiUrl("object?i1=View&o1=AW.Types.Employee--7&c1_DepartmentHistory=Table&as1=open&d1=ChangeDepartmentOrShift");
            wait.Until(d => d.FindElements(By.CssSelector("#department1 option")).Count() >= 3);
            var options = br.FindElements(By.CssSelector("#department1 option")).Select(e => e.Text).ToArray();
            Assert.AreEqual("*", options[0]);
            Assert.AreEqual("Engineering", options[1]);
            Assert.AreEqual("Tool Design", options[2]);
        }

        [TestMethod] 
        public void DefaultValueInt()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--9&as1=open&d1=EditQuantities");
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
    }
}