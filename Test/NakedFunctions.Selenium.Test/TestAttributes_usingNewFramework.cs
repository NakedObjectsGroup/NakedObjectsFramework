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
            MultiLine();
            Named();
            Optionally();
            PageSize();
            Password();
            RegEx();
            RenderEagerly();
            TableView();
            ValueRangeInt();
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
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Store--670")
                .GetObjectView().AssertTitleIs("Fitness Cycling")
                .AssertPropertiesAre("Store Name", "Demographics", "Sales Person", "Modified Date");
        }


        //[TestMethod]
        public void MultiLine()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SalesOrderHeader--51131")
                .GetObjectView().AssertTitleIs("SO51131")
                .GetProperty("Comment").AssertIsMultiLine();
        }

        // [TestMethod]
        public void Named()
        {
            helper.GotoHome().AssertMainMenusAre("Employees", "Addresses", "Persons", "Products", 
                "Work Orders", "Purchase Orders", "Vendors", "Customers", "Orders", "Sales", "Cart", "Special Offers");
        }

        //TestMethod]
        public void Optionally()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Employees")
                .GetActionWithDialog("Find Employee By Name").AssertIsEnabled().Open();
            dialog.GetTextField("Last Name").AssertHasPlaceholder("* ").AssertIsEmpty();
            dialog.GetTextField("First Name").AssertHasPlaceholder("").AssertIsEmpty();
            dialog.AssertOKIsDisabled("Missing mandatory fields: Last Name; ");
        }

        //[TestMethod]
        public void PageSize()
        {
            helper.GotoHome().OpenMainMenu("Employees").GetActionWithoutDialog("All Employees").ClickToViewList()
                .AssertDetails("Page 1 of 20; viewing 15 of 290 items");
        }

        //[TestMethod]
        public void Password()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Person--11714")
                .GetObjectView().AssertTitleIs("Marshall Black")
                .OpenActions().GetActionWithDialog("Change Password").AssertIsEnabled().Open()
                .GetTextField("Old Password").AssertIsPassword();
        }

        //[TestMethod]
        public void RegEx()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Customers")
                .GetActionWithDialog("Find Customer By Account Number").Open();

            dialog.GetTextField("Account Number").AssertHasValidationError("Invalid entry")
                .Clear().Enter("12345").AssertHasValidationError("Invalid entry")
                 .Clear().Enter("AW12345").AssertHasValidationError("Invalid entry")
                 .Clear().Enter("AW00012345").AssertNoValidationError();
        }

        //[TestMethod]
        public void RenderEagerly()
        {
            helper.GotoHome().OpenMainMenu("Employees")
                .GetActionWithoutDialog("List All Departments").ClickToViewList()
                .AssertIsTable().AssertTableHeaderHasColumns("", "Group Name");
        }

        //[TestMethod]
        public void TableView()
        {
            helper.GotoHome().OpenMainMenu("Purchase Orders")
                .GetActionWithoutDialog("All Open Purchase Orders").ClickToViewList()
                .ClickTableView()
                .AssertTableHeaderHasColumns("", "Vendor", "Order Date", "Total Due");
        }

       //[TestMethod]
        public void ValueRangeInt()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Product--890")
                .GetObjectView().AssertTitleIs("HL Touring Frame - Blue, 46")
                .OpenActions().GetActionWithDialog("Best Special Offer").AssertIsEnabled().Open()
                .GetTextField("Quantity")
                .AssertIsEmpty().AssertNoValidationError()
                .Clear().Enter("1000").AssertHasValidationError("Value is outside the range 1 to 999")
                .Clear().Enter("999").AssertNoValidationError()
                .Clear().Enter("0").AssertHasValidationError("Value is outside the range 1 to 999")
                 .Clear().Enter("1").AssertNoValidationError()
                .Clear().Enter("-1").AssertHasValidationError("Value is outside the range 1 to 999");
        }
    }
}