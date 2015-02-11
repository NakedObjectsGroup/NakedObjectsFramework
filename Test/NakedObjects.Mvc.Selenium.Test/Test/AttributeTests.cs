// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Web.UnitTests.Selenium;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
    public abstract class AttributeTests : AWWebTest {
        public abstract void PasswordIsObscuredInAnEntryField();

        public void DoPasswordIsObscuredInAnEntryField() {
            Login();

            // Click on Create New Individual Customer button and wait for Initial Password field  
            // password field
            var pwdField = wait.ClickAndWait("#CustomerRepository-CreateNewIndividualCustomer  button", "#CustomerRepository-CreateNewIndividualCustomer-InitialPassword-Input");
            // validate is a password field 
            Assert.AreEqual("password", pwdField.GetAttribute("type"));

            IWebElement ordField = br.FindElement(By.CssSelector("#CustomerRepository-CreateNewIndividualCustomer-FirstName-Input"));
            // validate first name is not a password field 
            Assert.AreEqual("text", ordField.GetAttribute("type"));
        }

        public abstract void MultiLineInViewMode();

        public void DoMultiLineInViewMode() {
            Login();

            // click on find customer by account number button and wait for account number input field 
            var f = wait.ClickAndWait("#CustomerRepository-FindCustomerByAccountNumber button", "#CustomerRepository-FindCustomerByAccountNumber-AccountNumber-Input");

            // enter account number 
            f.Clear();
            f.SendKeys("AW00000206" + Keys.Tab);

            // click on OK and wait for Formatted Demographics field
            var demog = wait.ClickAndWait(".nof-dialog .nof-ok", "#Store-FormattedDemographics div.multiline");
            Assert.AreEqual("AnnualSales: 800000 AnnualRevenue: 80000 BankName: Primary International BusinessType: BM YearOpened: 1994 Specialty: Road SquareFeet: 20000 Brands: AW Internet: DSL NumberEmployees: 18", demog.Text);
        }

        public abstract void MultiLineInEditMode();

        public void DoMultiLineInEditMode() {
            Login();

            // click on find order and wait for order number input 
            var f = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

            // enter order number 
            f.Clear();
            f.SendKeys("SO63557" + Keys.Tab);

            // click on OK and wait for Comment field
            wait.ClickAndWait(".nof-dialog .nof-ok", "#SalesOrderHeader-Comment");

            // click on edit and wait for Comment input 
            var comment = wait.ClickAndWait(".nof-edit", "#SalesOrderHeader-Comment textarea");

            // validate text area 
            Assert.AreEqual("3", comment.GetAttribute("rows"));
            Assert.AreEqual("50", comment.GetAttribute("cols"));

            // enter value in text area 
            comment.SendKeys("Line 1\nLine 2");

            // save and wait for comment field 
            var f2 = wait.ClickAndWait(".nof-save", "#SalesOrderHeader-Comment div.multiline");

            // validate comment value 
            string txt = f2.Text;
            Assert.IsTrue(txt.StartsWith("Line 1"));
            Assert.IsTrue(txt.EndsWith("Line 2"));
        }
    }
}