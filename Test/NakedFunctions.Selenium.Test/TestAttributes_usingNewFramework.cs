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
            //DefaultValue();
            //DescribedAsFunction();
            //Hidden();
            //Mask();
            //MaxLength();
            //MemberOrder();
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
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Employee--7&c1_DepartmentHistory=Table")
                .GetObjectView().AssertTitleIs("Dylan Miller")
                .OpenActions().GetActionWithDialog("Change Department Or Shift").Open();

            //TODO: GetSelectorField("Department")

            //wait.Until(d => d.FindElements(By.CssSelector("#department1 option")).Count() >= 3);
            //var options = br.FindElements(By.CssSelector("#department1 option")).Select(e => e.Text).ToArray();
            //Assert.AreEqual("Engineering", options[0]);
            //Assert.AreEqual("Tool Design", options[1]);
        }

      //  //[TestMethod]
      //  public void DefaultValue()
      //  {
      //      GeminiUrl("home?m1=SpecialOffer_MenuFunctions&d1=CreateNewSpecialOffer");
      //      var type = WaitForCss("input#type1");
      //      Assert.AreEqual("Promotion", type.GetAttribute("value"));
      //      var minQty = WaitForCss("input#minqty1");
      //      Assert.AreEqual("10", minQty.GetAttribute("value"));
      //      var start = WaitForCss("input#startdate1");
      //      var tomorrow = DateTime.Today.AddDays(1).ToString("d MMM yyyy");
      //      Assert.AreEqual(tomorrow, start.GetAttribute("value"));
      //  }


      // // [TestMethod]
      //  public void DescribedAsFunction()
      //  {
      //      GeminiUrl("home?m1=Sales_MenuFunctions");
      //      WaitForTitle("Home");
      //      OpenSubMenu("Sales");
      //      var action5 = WaitForCssNo("nof-action-list nof-action input", 5);
      //      Assert.AreEqual("Random Sales Person", action5.GetAttribute("value"));
      //      var action0 = WaitForCssNo("nof-action-list nof-action input", 0);
      //      Assert.AreEqual("Create New Sales Person", action0.GetAttribute("value"));
      //      Assert.AreEqual("... from an existing Employee", action0.GetAttribute("title"));
      //  }

      //  //[TestMethod]
      //  public void DescribedAsParameter()
      //  {
      //      GeminiUrl("object?i1=View&o1=AW.Types.Person--16315&as1=open&d1=CreateNewCreditCard");
      //      WaitForTitle("Casey Nath");
      //      var number = WaitForCss("input#cardnumber1").GetAttribute("placeholder");
      //      Assert.IsTrue(number.Contains("No spaces"));
      //      var expires = WaitForCss("input#expires1").GetAttribute("placeholder");
      //      Assert.IsTrue(expires.Contains("mm/yy"));
      //  }


      //  //[TestMethod]
      //  public void Hidden()
      //  {
      //      GeminiUrl("object?i1=View&o1=AW.Types.Shift--1");
      //      WaitForTitle("Day");
      //      Assert.AreEqual("Name:", WaitForCssNo("nof-view-property .name", 0).Text);
      //      Assert.AreEqual("Start Time:", WaitForCssNo("nof-view-property .name", 1).Text);
      //      Assert.AreEqual("End Time:", WaitForCssNo("nof-view-property .name", 2).Text);
      //      Assert.AreEqual("Modified Date:", WaitForCssNo("nof-view-property .name", 3).Text);
      //      Assert.AreEqual(4, br.FindElements(By.CssSelector("nof-view-property")).Count);
      //      //i.e. no 'Shift ID' field showing
      //  }

      //// [TestMethod]
      //  public void Mask()
      //  {
      //      GeminiUrl("object?i1=View&o1=AW.Types.Product--497");
      //      WaitForTitle("Pinch Bolt");
      //      var listPrice = GetPropertyValue("List Price");
      //      Assert.AreEqual("£0.00", listPrice);
      //      var startDate = GetPropertyValue("Sell Start Date");
      //      Assert.AreEqual("1 Jun 2002", startDate);
      //  }

      //  //[TestMethod]
      //  public void MaxLength()
      //  {
      //      GeminiUrl("home?m1=SpecialOffer_MenuFunctions&d1=CreateNewSpecialOffer");
      //      var description = "input#description1";
      //      string longText = "Now is the time for all good men to come to the aid of the party.";
      //      string invalid = "Too long";
      //      var validation = WaitForCssNo("nof-edit-parameter .validation", 0);
      //      Assert.AreEqual("", validation.Text);

      //      TypeIntoFieldWithoutClearing(description, longText);
      //      Assert.AreEqual(invalid, validation.Text);

      //      ClearFieldThenType(description, longText.Substring(0, 50));
      //      Assert.AreEqual("", validation.Text);
      //  }

      //  //[TestMethod]
      //  public void MemberOrder()
      //  {
      //      GeminiUrl("object?i1=View&o1=AW.Types.Store--670");
      //      WaitForTitle("Fitness Cycling");
      //      Assert.AreEqual("Modified Date:", WaitForCssNo("nof-view-property .name", 3).Text);
      //      Assert.AreEqual("Sales Person:", WaitForCssNo("nof-view-property .name", 2).Text);
      //      Assert.AreEqual("Demographics:", WaitForCssNo("nof-view-property .name", 1).Text);
      //      Assert.AreEqual("Store Name:", WaitForCssNo("nof-view-property .name", 0).Text);
      //  }


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