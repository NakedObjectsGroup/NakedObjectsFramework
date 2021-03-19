// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Selenium.Helpers.Tests;
using NakedFrameworkClient.TestFramework;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedFunctions.Selenium.Test.FunctionTests
{

    [TestClass]
    public class TestAttributes_usingNewFramework
    {
        #region Overhead
        private string baseUrl = "http://nakedfunctionstest.azurewebsites.net/";
        private Helper helper;

        [ClassInitialize]
        public static void InitialiseClass(TestContext context)
        {
            Helper.FilePath(@"drivers.chromedriver.exe");
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            helper = new Helper(baseUrl);
        }

        [TestCleanup]
        public virtual void CleanUpTest() => helper.CleanUp();
        #endregion

        [TestMethod]
        public void AllWorkingAttributes()
        {
            Bounded();
            DefaultValue();
            DescribedAsFunction();
            DescribedAsParameter();
            Hidden();
            Mask();
            MaxLength();
            MemberOrder();
            //MultiLine();
            //Named();
            //Optionally();
            //PageSize();
            //Password();
            //RegEx();
            //RenderEagerly();
            //TableView();
            //ValueRangeInt();
        }

        //[TestMethod]
        public void Bounded()
        {
            //Change Department Or Shift on Employee. Both params are of Bounded types
            var dialog = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Employee--7&c1_DepartmentHistory=Table")
                .GetObjectView().AssertTitleIs("Dylan Miller")
                .OpenActions().GetActionWithDialog("Change Department Or Shift").Open();
            dialog.GetSelectionField("Department")
                .AssertOptionIs(0, "Engineering")
                .AssertOptionIs(1, "Tool Design");
        }

        //[TestMethod]
        public void DefaultValue()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Special Offers").GetActionWithDialog("Create New Special Offer").AssertIsEnabled().Open();
            dialog.GetTextField("Type").AssertDefaultValueIs("Promotion");
            dialog.GetTextField("Min Qty").AssertDefaultValueIs("10");
            var tomorrow = DateTime.Today.AddDays(1).ToString("d MMM yyyy");
            dialog.GetTextField("Start Date").AssertDefaultValueIs(tomorrow);
        }


        //[TestMethod]
        public void DescribedAsFunction()
        {
            helper.GotoHome().OpenMainMenu("Sales").OpenSubMenu("Sales")
                .GetActionWithDialog("Create New Sales Person").AssertHasTooltip("... from an existing Employee");
        }

        //[TestMethod]
        public void DescribedAsParameter()
        {
            var dialog = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Person--16315").GetObjectView().AssertTitleIs("Casey Nath")
                .OpenActions().GetActionWithDialog("Create New Credit Card").Open();
            dialog.GetTextField("Card Number").AssertHasPlaceholder("* No spaces");
            dialog.GetTextField("Expires").AssertHasPlaceholder("* mm/yy");
        }


        //[TestMethod]
        public void Hidden()
        {
            var shift = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Shift--1").GetObjectView().AssertTitleIs("Day");
            shift.AssertPropertiesAre("Name", "Start Time", "End Time", "Modified Date");
            //i.e. no 'Shift ID' field showing
        }

         //[TestMethod]
        public void Mask()
        {
            var prod = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Product--497").GetObjectView().AssertTitleIs("Pinch Bolt");
            prod.GetProperty("List Price").AssertValueIs("£0.00");
            prod.GetProperty("Sell Start Date").AssertValueIs("1 Jun 2002");
        }

        //[TestMethod]
        public void MaxLength()
        {
            string longText = "Now is the time for all good men to come to the aid of the party.";

            var desc = helper.GotoHome().OpenMainMenu("Special Offers").GetActionWithDialog("Create New Special Offer")
                .AssertIsEnabled().Open().GetTextField("Description")
                .Clear().AssertNoValidationError();

            desc.Enter(longText).AssertHasValidationError("Too long");

            desc.Clear().Enter(longText.Substring(0, 50)).AssertNoValidationError();
        }

        //[TestMethod]
        public void MemberOrder()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Store--670").GetObjectView().AssertTitleIs("Fitness Cycling")
                .AssertPropertiesAre("Store Name", "Demographics", "Sales Person", "Modified Date");
        }


        //  //[TestMethod]
        //  public void MultiLine()
        //  {
        //      GeminiUrl("object?i1=View&o1=AW.Types.SalesOrderHeader--51131");
        //      WaitForTitle("SO51131");
        //      var comment = GetProperty("Comment");
        //      comment.FindElement(By.CssSelector(".multiline"));
        //  }

        //  // [TestMethod]
        //  public void Named()
        //  {
        //      Home();
        //      var employee_menuFunctions = WaitForCssNo("nof-menu-bar nof-action input", 0);
        //      Assert.AreEqual("Employees", employee_menuFunctions.GetAttribute("value"));
        //  }

        //  //[TestMethod]
        //  public void Optionally()
        //  {
        //      GeminiUrl("home?m1=Employee_MenuFunctions&d1=FindEmployeeByName");
        //      var lastName = WaitForCss("input#lastname1");
        //      Assert.IsTrue(lastName.GetAttribute("placeholder").Contains("*"));
        //      Assert.AreEqual("", lastName.GetAttribute("value"));
        //      var firstName = WaitForCss("input#firstname1");
        //      var placeholder = firstName.GetAttribute("placeholder");
        //      Assert.IsTrue(placeholder is null || placeholder == "");
        //      Assert.AreEqual("", firstName.GetAttribute("value"));
        //      Assert.AreEqual("Missing mandatory fields: Last Name; ", OKButton().GetAttribute("title"));
        //  }

        //  //[TestMethod]
        //  public void PageSize()
        //  {
        //      GeminiUrl("home?m1=Employee_MenuFunctions");
        //      WaitForTitle("Home");
        //      Click(WaitForCss("input[value=\"All Employees\""));
        //      WaitForTitle("All Employees");
        //      var page = WaitForCss(".summary .details");
        //      Assert.AreEqual("Page 1 of 20; viewing 15 of 290 items", page.Text);
        //  }

        //  //[TestMethod]
        //  public void Password()
        //  {
        //      GeminiUrl("object?i1=View&o1=AW.Types.Person--11714&as1=open&d1=ChangePassword");
        //      WaitForTitle("Marshall Black");
        //      var oldPWField = WaitForCss("input#oldpassword1");
        //      Assert.AreEqual("password", oldPWField.GetAttribute("type"));
        //  }

        //  //[TestMethod]
        //  public void RegEx()
        //  {
        //      GeminiUrl("home?m1=Customer_MenuFunctions&d1=FindCustomerByAccountNumber");
        //      WaitForTitle("Home");
        //      string invalid = "Invalid entry";
        //      var input = "input#accountnumber1";
        //      Assert.AreEqual("AW", WaitForCss(input).GetAttribute("value"));
        //      var validation = WaitForCss("nof-edit-parameter .validation");
        //      Assert.AreEqual(invalid, validation.Text);

        //      ClearFieldThenType(input, "12345");
        //      Assert.AreEqual(invalid, validation.Text);

        //      ClearFieldThenType(input, "AW12345");
        //      Assert.AreEqual(invalid, validation.Text);

        //      ClearFieldThenType(input, "AW00012345");
        //      Assert.AreEqual("", validation.Text);

        //  }

        //  //[TestMethod]
        //  public void RenderEagerly()
        //  {
        //      GeminiUrl("home");
        //      WaitForTitle("Home");
        //      OpenMainMenuAction("Employees", "List All Departments");
        //      WaitForTitle("List All Departments");
        //      wait.Until(dr => dr.FindElements(By.CssSelector("th")).Where(el => el.Text == "Group Name").Single());
        //  }

        //  //[TestMethod]
        //  public void TableView()
        //  {
        //      GeminiUrl("home");
        //      WaitForTitle("Home");
        //      OpenMainMenuAction("Purchase Orders","All Open Purchase Orders");
        //      Click(WaitForCss(".summary .icon.table"));
        //      Assert.AreEqual("Total Due", WaitForCssNo("thead th", 3).Text);
        //      Assert.AreEqual("Order Date", WaitForCssNo("thead th", 2).Text);
        //      Assert.AreEqual("Vendor", WaitForCssNo("thead th", 1).Text);
        //      Assert.AreEqual("", WaitForCssNo("thead th", 0).Text);
        //  }

        //  //[TestMethod]
        //  public void ValueRangeInt()
        //  {
        //      GeminiUrl("object?i1=View&o1=AW.Types.Product--890&as1=open&d1=BestSpecialOffer");
        //      WaitForTitle("HL Touring Frame - Blue, 46");
        //      var qty = "input#quantity1";
        //      var val = WaitForCss("nof-edit-parameter .validation");
        //      Assert.AreEqual("", val.Text);
        //      ClearFieldThenType(qty, "1000");
        //      Assert.AreEqual("Value is outside the range 1 to 999", val.Text);
        //      ClearFieldThenType(qty, "10");
        //      Assert.AreEqual("", val.Text);
        //      ClearFieldThenType(qty, "1000");
        //      Assert.AreEqual("Value is outside the range 1 to 999", val.Text);
        //  }
    }
}