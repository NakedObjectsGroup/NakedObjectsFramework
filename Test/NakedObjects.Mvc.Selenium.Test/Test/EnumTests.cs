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
    //    annotated //[TestMethod] and simply delegate to the inherited 'Do' method.
    //    The following Regex will do this automatically:
    //    Find:     ^{.*}public override void {.*}\n{.*\{}\n{.*}{throw .*}$
    //    Replace:  \1\[TestMethod\]\n\1public override void \2\n\3\n\4Do\2;
    // 4. When IE tests run OK, uncomment the Firefox and/or Chrome classes and repeat step 3
    public abstract class EnumTests : AWWebTest {
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        public abstract void ViewEnumProperty();

        protected void DoViewEnumProperty() {
            Login();
            FindOrder("SO67861");
            br.AssertPageTitleEquals("SO67861");
            IWebElement status = br.GetField("SalesOrderHeader-Status");
            status.AssertValueEquals("Shipped");
        }

        public abstract void EditEnumProperty();

        protected void DoEditEnumProperty() {
            Login();
            FindOrder("SO67862");
            br.AssertPageTitleEquals("SO67862");
            br.ClickEdit();
            IWebElement status = br.GetField("SalesOrderHeader-Status");
            status.SelectDropDownItem("Cancelled", br);
            ////Must adjust due date or else save fails
            //IWebElement due = br.GetField("SalesOrderHeader-ShipDate");
            //string tomorrow = DateTime.Today.ToShortDateString();
            //due.TypeText(tomorrow, br);

            br.ClickSave();
            status = br.GetField("SalesOrderHeader-Status");
            status.AssertValueEquals("Cancelled");
        }
    }
}