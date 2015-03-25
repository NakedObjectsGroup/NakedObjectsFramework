// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            f.TypeText("AW00000546" + Keys.Tab);

            var action = wait.ClickAndWait(".nof-ok", "#Store-CreateNewAddress button");

            Assert.AreEqual("Field Trip Store, AW00000546", br.Title);

            var country = wait.ClickAndWait(action, "#Address-CountryRegion");

            Assert.AreEqual(0, country.FindElement(By.ClassName("nof-object")).FindElements(By.TagName("a")).Count());

            var province = br.FindElement(By.CssSelector("#Address-StateProvince"));

            Assert.AreEqual(0, province.FindElement(By.ClassName("nof-object")).FindElements(By.TagName("a")).Count());

            country.SelectDropDownItem("Australia", br);

            br.FindElement(By.CssSelector(".nof-save")).Click();

            province = br.FindElement(By.CssSelector("#Address-StateProvince"));
            Assert.AreEqual(0, province.FindElement(By.ClassName("nof-object")).FindElements(By.TagName("a")).Count());
            province.SelectDropDownItem("Queensland", br);

            br.FindElement(By.CssSelector(".nof-save")).Click();

            country = br.FindElement(By.CssSelector("#Address-CountryRegion"));
            Assert.AreEqual(0, country.FindElement(By.ClassName("nof-object")).FindElements(By.TagName("a")).Count());
            country.SelectDropDownItem("United Kingdom", br);

            br.FindElement(By.CssSelector(".nof-save")).Click();

            province = br.FindElement(By.CssSelector("#Address-StateProvince"));
            Assert.AreEqual(0, province.FindElement(By.ClassName("nof-object")).FindElements(By.TagName("a")).Count());
            province.SelectDropDownItem("England", br);

            br.FindElement(By.CssSelector(".nof-save")).Click();
        }
    }
}