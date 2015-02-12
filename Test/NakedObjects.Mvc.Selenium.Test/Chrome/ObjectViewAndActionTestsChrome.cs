// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.Mvc.Selenium.Test.Chrome {
    //[TestClass]
    public class ObjectViewAndActionTestsChrome : ObjectViewAndActionTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = InitChromeDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        //[TestMethod]
        [Ignore] // fails randomly on server 
        public override void ViewPersistedObject() {
            DoViewPersistedObject();
        }

        //[TestMethod]
        public override void ViewTableHeader() {
            DoViewTableHeader();
        }

        //[TestMethod]
        public override void ViewViewModel() {
            DoViewViewModel();
        }

        //[TestMethod]
        public override void InvokeActionNoParmsNoReturn() {
            DoInvokeActionNoParmsNoReturn();
        }

        //[TestMethod]
        public override void InvokeActionParmsReturn() {
            DoInvokeActionParmsReturn();
        }

        //[TestMethod]
        public override void InvokeActionParmsReturnPopup() {
            DoInvokeActionParmsReturnPopup();
        }

        //[TestMethod]
        public override void ShowActionParmsReturnPopup() {
            DoShowActionParmsReturnPopup();
        }

        //[TestMethod]
        public override void InvokeActionOnViewModel() {
            DoInvokeActionOnViewModel();
        }

        //[TestMethod]
        public override void InvokeActionOnViewModelPopup() {
            DoInvokeActionOnViewModelPopup();
        }

        [TestMethod, Ignore]
        public override void InvokeActionOnViewModelReturnCollection() {
            DoInvokeActionOnViewModelReturnCollection();
        }

        [TestMethod, Ignore]
        public override void InvokeActionOnViewModelReturnCollectionPopup() {
            DoInvokeActionOnViewModelReturnCollectionPopup();
        }

        //[TestMethod]
        public override void InvokeActionParmsMandatory() {
            DoInvokeActionParmsMandatory();
        }

        //[TestMethod]
        public override void InvokeActionParmsInvalid() {
            DoInvokeActionParmsInvalid();
        }

        //[TestMethod]
        public override void InvokeActionParmsMandatoryPopup() {
            DoInvokeActionParmsMandatoryPopup();
        }

        //[TestMethod]
        [Ignore] // still failing  on chrome
        public override void InvokeActionParmsInvalidPopup() {
            DoInvokeActionParmsInvalidPopup();
        }

        //[TestMethod]
        public override void InvokeContributedActionNoParmsReturnTransient() {
            DoInvokeContributedActionNoParmsReturnTransient();
        }

        //[TestMethod]
        public override void InvokeContributedActionNoParmsReturnPersistent() {
            DoInvokeContributedActionNoParmsReturnPersistent();
        }

        //[TestMethod]
        public override void InvokeContributedActionParmsReturn() {
            DoInvokeContributedActionParmsReturn();
        }

        //[TestMethod]
        [Ignore] // failing again on chrome
        public override void InvokeContributedActionParmsReturnPopup() {
            DoInvokeContributedActionParmsReturnPopup();
        }

        //[TestMethod]
        public override void CancelActionDialog() {
            DoCancelActionDialog();
        }

        //[TestMethod]
        public override void CancelActionDialogPopup() {
            DoCancelActionDialogPopup();
        }

        //[TestMethod]
        public override void EmptyCollectionDoesNotShowListOrTableButtons() {
            DoEmptyCollectionDoesNotShowListOrTableButtons();
        }

        //[TestMethod]
        public override void RemoveFromActionDialog() {
            DoRemoveFromActionDialog();
        }

        //[TestMethod]
        public override void RemoveFromActionDialogPopup() {
            DoRemoveFromActionDialogPopup();
        }

        //[TestMethod]
        public override void RecentlyViewedOnActionDialog() {
            DoRecentlyViewedOnActionDialog();
        }

        //[TestMethod]
        [Ignore] // fails randomly on server 
        public override void RecentlyViewedOnActionDialogWithSelect() {
            DoRecentlyViewedOnActionDialogWithSelect();
        }

        //[TestMethod]
        public override void ActionFindOnActionDialog() {
            DoActionFindOnActionDialog();
        }

        [TestMethod, Ignore]
        public override void NewObjectOnActionDialog() {
            DoNewObjectOnActionDialog();
        }

        //[TestMethod]
        public override void AutoCompleteOnActionDialog() {
            DoAutoCompleteOnActionDialog();
        }

        //[TestMethod]
        public override void NewObjectOnActionDialogFailMandatory() {
            DoNewObjectOnActionDialogFailMandatory();
        }

        //[TestMethod]
        public override void NewObjectOnActionDialogFailInvalid() {
            DoNewObjectOnActionDialogFailInvalid();
        }

        //[TestMethod]
        public override void RecentlyViewedOnActionDialogPopup() {
            DoRecentlyViewedOnActionDialogPopup();
        }

        //[TestMethod]
        [Ignore] // fails randomly on server 
        public override void RecentlyViewedOnActionDialogWithSelectPopup() {
            DoRecentlyViewedOnActionDialogWithSelectPopup();
        }

        //[TestMethod]
        public override void ActionFindOnActionDialogPopup() {
            DoActionFindOnActionDialogPopup();
        }

        //[TestMethod]
        [Ignore] // failing again on chrome
        public override void NewObjectOnActionDialogPopup() {
            DoNewObjectOnActionDialogPopup();
        }

        //[TestMethod]
        public override void AutoCompleteOnActionDialogPopup() {
            DoAutoCompleteOnActionDialogPopup();
        }

        //[TestMethod]
        [Ignore] // failing again on chrome
        public override void NewObjectOnActionDialogFailMandatoryPopup() {
            DoNewObjectOnActionDialogFailMandatoryPopup();
        }

        //[TestMethod]
        [Ignore] // failing again on chrome
        public override void NewObjectOnActionDialogFailInvalidPopup() {
            DoNewObjectOnActionDialogFailInvalidPopup();
        }
    }
}