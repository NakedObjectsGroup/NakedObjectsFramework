// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Firefox;

namespace NakedObjects.Mvc.Selenium.Test.Firefox {
    [TestClass]
    public class StandaloneCollectionTestsFirefox : StandaloneCollectionTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        [TestMethod]
        public override void ViewStandaloneCollection() {
            DoViewStandaloneCollection();
        }

        [TestMethod]
        public override void ViewStandaloneCollectionTable() {
            DoViewStandaloneCollectionTable();
        }

        [TestMethod]
        public override void ViewStandaloneCollectionDefaultToTable() {
            DoViewStandaloneCollectionDefaultToTable();
        }

        [TestMethod]
        public override void SelectDeselectAll() {
            DoSelectDeselectAll();
        }

        [TestMethod]
        public override void SelectAndUnselectIndividually() {
            DoSelectAndUnselectIndividually();
        }

        [TestMethod]
        public override void InvokeContributedActionNoParmsNoReturn() {
            DoInvokeContributedActionNoParmsNoReturn();
        }

        [TestMethod]
        public override void InvokeContributedActionParmsNoReturn() {
            DoInvokeContributedActionParmsNoReturn();
        }

        [TestMethod]
        public override void InvokeContributedActionParmsValidateFail() {
            DoInvokeContributedActionParmsValidateFail();
        }

        [TestMethod]
        public override void InvokeContributedActionNoSelections() {
            DoInvokeContributedActionNoSelections();
        }

        [TestMethod]
        public override void PagingWithDefaultPageSize() {
            DoPagingWithDefaultPageSize();
        }

        [TestMethod] // fails on server too often
        public override void PagingWithOverriddenPageSize() {
            DoPagingWithOverriddenPageSize();
        }

        [TestMethod]
        public override void PagingWithFormat() {
            DoPagingWithFormat();
        }
    }
}