// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
    public abstract class HistoryAndClearTests : AWWebTest {
        public abstract void HistoryOnIndex();

        public void DoHistoryOnIndex() {
            Login();
            // title goes to 'Home Page' the empty - may change when #1602 is fixed - in meantime wait a bit so page test is consistent
            Thread.Sleep(2000);
            // br.AssertPageTitleEquals("Home Page"); currently IIS7 - to do fix home page title
            //IWebElement history = br.FindElement(By.ClassName("History"));
            br.AssertElementExists(By.ClassName("nof-history"));
        }

        public abstract void ClearHistoryOnIndex();

        public void DoClearHistoryOnIndex() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            br.GoHome();
            br.AssertElementExists(By.CssSelector("[Title=Clear]"));
            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);
            br.ClickClearHistory();
            br.AssertElementDoesNotExist(By.CssSelector("[Title=Clear]"));
            Assert.AreEqual(0, br.GetHistory().FindElements(By.TagName("a")).Count);
        }

        public abstract void CumulativeHistory();

        public void DoCumulativeHistory() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            br.AssertElementExists(By.CssSelector("[Title=Clear]"));

            // 1st object
            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            Assert.AreEqual(2, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            // 3rd object
            br.ClickOnObjectLinkInField("SalesPerson-SalesTerritory");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            //Go back to second object
            br.GoBackViaHistoryBy(1);
            br.AssertPageTitleEquals("José Saraiva");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            //Go back to first object
            br.GoBackViaHistoryBy(2);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            //Click on last object has no effect
            br.GoBackViaHistoryBy(0);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            //Go back to first object
            br.GoBackViaHistoryBy(2);
            br.AssertPageTitleEquals("Canada");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a")).Last().Text);
        }

        public abstract void TransientObjectsDoNotShowUpInHistory();

        public void DoTransientObjectsDoNotShowUpInHistory() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);

            br.ClickAction("CustomerRepository-CreateNewStoreCustomer");
            br.AssertContainsObjectEditTransient();

            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);

            br.GetField("Store-Name").TypeText("Foo Bar", br);
            br.ClickSave();

            Assert.AreEqual(2, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("Foo Bar, AW00029484", br.GetHistory().FindElements(By.TagName("a")).Last().Text);
        }

        public abstract void CollectionsDoNotShowUpInHistory();

        public void DoCollectionsDoNotShowUpInHistory() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);

            br.ClickAction("OrderRepository-HighestValueOrders");
            br.AssertPageTitleEquals("20 Sales Orders");

            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);
        }

        public abstract void ClearButton();

        public void DoClearButton() {
            Login();
            FindCustomerByAccountNumber("AW00000067");
            FindCustomerByAccountNumber("AW00000066");
            FindCustomerByAccountNumber("AW00000065");
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");

            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);

            br.ClickClearHistory();

            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a")).First().Text);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }
    }

    // Replaced by tabbed history - keep tests until old history is removed 
}