// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
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

            var f = wait.ClickAndWait("#CustomerRepository-FindCustomerByAccountNumber button", "#CustomerRepository-FindCustomerByAccountNumber-AccountNumber-Input");

            f.Clear();
            f.SendKeys("AW00000546" + Keys.Tab);

            var action = wait.ClickAndWait(".nof-ok", "#Store-CreateNewAddress button");

            br.AssertPageTitleEquals("Field Trip Store, AW00000546");

            var country = wait.ClickAndWait(action, "#Address-CountryRegion");

            country.AssertIsEmpty();

            IWebElement province = br.FindElement(By.CssSelector("#Address-StateProvince"));

            province.AssertIsEmpty();

            country.SelectDropDownItem("Australia", br);

            br.FindElement(By.CssSelector(".nof-save")).Click();

            province = br.FindElement(By.CssSelector("#Address-StateProvince"));
            province.AssertIsEmpty();
            province.SelectDropDownItem("Queensland", br);

            br.FindElement(By.CssSelector(".nof-save")).Click();

            country = br.FindElement(By.CssSelector("#Address-CountryRegion"));
            country.AssertIsEmpty();
            country.SelectDropDownItem("United Kingdom", br);

            br.FindElement(By.CssSelector(".nof-save")).Click();

            province = br.FindElement(By.CssSelector("#Address-StateProvince"));
            province.AssertIsEmpty();
            province.SelectDropDownItem("England", br);

            br.FindElement(By.CssSelector(".nof-save")).Click();
        }
    }
}