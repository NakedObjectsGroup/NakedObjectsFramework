// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Helper;

namespace NakedObjects.Mvc.Selenium.Test.Chrome {
    //[TestClass] //no longer working - investigate later
    public class TabbedHistoryAndClearTestsChrome : TabbedHistoryAndClearTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = InitChromeDriver();
            br.Navigate().GoToUrl(url);
            FindCustomerByAccountNumber("AW00000065");
            br.ClickClearAll(0);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        //[TestMethod]
        public override void CumulativeHistory() {
            DoCumulativeHistory();
        }

        //[TestMethod]
        public override void ClearSingleItem() {
            DoClearSingleItem();
        }

        //[TestMethod]
        public override void ClearSingleCollectionItem() {
            DoClearSingleCollectionItem();
        }

        //[TestMethod]
        public override void ClearActiveItem() {
            DoClearActiveItem();
        }

        //[TestMethod]
        public override void CollectionKeepsPage() {
            DoCollectionKeepsPage();
        }

        //[TestMethod]
        public override void CollectionKeepsFormat() {
            DoCollectionKeepsFormat();
        }

        //[TestMethod]
        public override void ClearActiveCollectionItem() {
            DoClearActiveCollectionItem();
        }

        //[TestMethod]
        public override void ClearActiveMultipleCollectionItems() {
            DoClearActiveMultipleCollectionItems();
        }

        //[TestMethod]
        public override void ClearInActiveItem() {
            DoClearInActiveItem();
        }

        //[TestMethod]
        public override void ClearInActiveCollectionItem() {
            DoClearInActiveCollectionItem();
        }

        //[TestMethod]
        public override void ClearInActiveCollectionMultipleItems() {
            DoClearInActiveCollectionMultipleItems();
        }

        //[TestMethod]
        public override void ClearOthersSingleItem() {
            DoClearOthersSingleItem();
        }

        //[TestMethod]
        public override void ClearOthersSingleCollectionItem() {
            DoClearOthersSingleCollectionItem();
        }

        //[TestMethod]
        public override void ClearOthersActiveItem() {
            DoClearOthersActiveItem();
        }

        //[TestMethod]
        public override void ClearOthersActiveCollectionItem() {
            DoClearOthersActiveCollectionItem();
        }

        //[TestMethod]
        public override void ClearOthersInActiveItem() {
            DoClearOthersInActiveItem();
        }

        //[TestMethod]
        public override void ClearOthersInActiveCollectionItem() {
            DoClearOthersInActiveCollectionItem();
        }

        //[TestMethod]
        public override void ClearAllSingleItem() {
            DoClearAllSingleItem();
        }

        //[TestMethod]
        public override void ClearAllSingleCollectionItem() {
            DoClearAllSingleCollectionItem();
        }

        //[TestMethod]
        public override void ClearAllActiveItem() {
            DoClearAllActiveItem();
        }

        //[TestMethod]
        public override void ClearAllActiveCollectionItem() {
            DoClearAllActiveCollectionItem();
        }

        //[TestMethod]
        public override void ClearAllInActiveItem() {
            DoClearAllInActiveItem();
        }

        //[TestMethod]
        public override void ClearAllInActiveCollectionItem() {
            DoClearAllInActiveCollectionItem();
        }

        //[TestMethod]
        public override void TransientObjectsDoNotShowUpInHistory() {
            DoTransientObjectsDoNotShowUpInHistory();
        }

        //[TestMethod]
        public override void CollectionsShowUpInHistory() {
            DoCollectionsShowUpInHistory();
        }
    }
}