// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
    // 3. Make MyTestsIE implement the abstract MyTests class.  Each created method should be
    //    annotated [TestMethod] and simply delegate to the inherited 'Do' method.
    //    The following Regex will do this automatically:
    //    Find:     ^{.*}public override void {.*}\n{.*\{}\n{.*}{throw .*}$
    //    Replace:  \1\[TestMethod\]\n\1public override void \2\n\3\n\4Do\2;
    // 4. When IE tests run OK, uncomment the Firefox and/or Chrome classes and repeat step 3
    public abstract class EnumTests : AWWebTest {
        public abstract void ViewEnumProperty();

        protected void DoViewEnumProperty() {
            Login();

            var orderNumber = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

            orderNumber.Clear();
            orderNumber.SendKeys("SO67861" + Keys.Tab);

            var status = wait.ClickAndWait(".nof-ok", "#SalesOrderHeader-Status .nof-value");
            Assert.AreEqual("Shipped", status.Text);
            br.AssertPageTitleEquals("SO67861");
        }

        public abstract void EditEnumProperty();

        protected void DoEditEnumProperty() {
            Login();

            var orderNumber = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

            orderNumber.Clear();
            orderNumber.SendKeys("SO67862" + Keys.Tab);

            var edit = wait.ClickAndWait(".nof-ok", ".nof-edit");
            wait.ClickAndWait(edit, "#SalesOrderHeader-Status-Input");

            var status = br.FindElement(By.CssSelector("#SalesOrderHeader-Status"));

            status.SelectDropDownItem("Cancelled", br);

            wait.ClickAndWait(".nof-save", ".nof-objectview");

            status = br.FindElement(By.CssSelector("#SalesOrderHeader-Status div.nof-value"));

            Assert.AreEqual("Cancelled", status.Text);
        }
    }
}