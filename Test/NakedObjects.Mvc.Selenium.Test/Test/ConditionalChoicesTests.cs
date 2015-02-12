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
}