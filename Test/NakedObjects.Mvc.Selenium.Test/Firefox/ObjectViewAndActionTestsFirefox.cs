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
    public class ObjectViewAndActionTestsFirefox : ObjectViewAndActionTests {
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
        public override void ViewPersistedObject() {
            DoViewPersistedObject();
        }

        [TestMethod]
        public override void ViewTableHeader() {
            DoViewTableHeader();
        }

        [TestMethod]
        public override void ViewViewModel() {
            DoViewViewModel();
        }

        [TestMethod]
        public override void InvokeActionNoParmsNoReturn() {
            DoInvokeActionNoParmsNoReturn();
        }

        [TestMethod]
        public override void InvokeActionParmsReturn() {
            DoInvokeActionParmsReturn();
        }

        [TestMethod] // intermittent failing on ff only
        public override void InvokeActionParmsReturnPopup() {
            DoInvokeActionParmsReturnPopup();
        }

        [TestMethod] // intermittent failing on ff only
        public override void ShowActionParmsReturnPopup() {
            DoShowActionParmsReturnPopup();
        }

        [TestMethod]
        public override void InvokeActionOnViewModel() {
            DoInvokeActionOnViewModel();
        }

        [TestMethod]
        public override void InvokeActionOnViewModelPopup() {
            DoInvokeActionOnViewModelPopup();
        }

        [TestMethod]
        public override void InvokeActionOnViewModelReturnCollection() {
            DoInvokeActionOnViewModelReturnCollection();
        }

        [TestMethod]
        public override void InvokeActionOnViewModelReturnCollectionPopup() {
            DoInvokeActionOnViewModelReturnCollectionPopup();
        }

        [TestMethod]
        public override void InvokeActionParmsMandatory() {
            DoInvokeActionParmsMandatory();
        }

        [TestMethod]
        public override void InvokeActionParmsInvalid() {
            DoInvokeActionParmsInvalid();
        }

        [TestMethod]
        public override void InvokeActionParmsMandatoryPopup() {
            DoInvokeActionParmsMandatoryPopup();
        }

        [TestMethod]
        public override void InvokeActionParmsInvalidPopup() {
            DoInvokeActionParmsInvalidPopup();
        }

        [TestMethod]
        public override void InvokeContributedActionNoParmsReturnTransient() {
            DoInvokeContributedActionNoParmsReturnTransient();
        }

        [TestMethod]
        public override void InvokeContributedActionNoParmsReturnPersistent() {
            DoInvokeContributedActionNoParmsReturnPersistent();
        }

        [TestMethod] //This is repeatedly failing on server, even though it runs fine on RP's machine
        public override void InvokeContributedActionParmsReturn() {
            DoInvokeContributedActionParmsReturn();
        }

        [TestMethod]
        public override void InvokeContributedActionParmsReturnPopup() {
            DoInvokeContributedActionParmsReturnPopup();
        }

        [TestMethod]
        public override void CancelActionDialog() {
            DoCancelActionDialog();
        }

        [TestMethod]
        public override void CancelActionDialogPopup() {
            DoCancelActionDialogPopup();
        }

        [TestMethod]
        public override void EmptyCollectionDoesNotShowListOrTableButtons() {
            DoEmptyCollectionDoesNotShowListOrTableButtons();
        }

        [TestMethod]
        public override void RemoveFromActionDialog() {
            DoRemoveFromActionDialog();
        }

        [TestMethod]
        public override void RemoveFromActionDialogPopup() {
            DoRemoveFromActionDialogPopup();
        }

        [TestMethod]
        public override void RecentlyViewedOnActionDialog() {
            DoRecentlyViewedOnActionDialog();
        }

        [TestMethod]
        public override void RecentlyViewedOnActionDialogWithSelect() {
            DoRecentlyViewedOnActionDialogWithSelect();
        }

        [TestMethod]
        public override void ActionFindOnActionDialog() {
            DoActionFindOnActionDialog();
        }

        [TestMethod]
        public override void NewObjectOnActionDialog() {
            DoNewObjectOnActionDialog();
        }

        [TestMethod]
        public override void AutoCompleteOnActionDialog() {
            DoAutoCompleteOnActionDialog();
        }

        [TestMethod]
        public override void NewObjectOnActionDialogFailMandatory() {
            DoNewObjectOnActionDialogFailMandatory();
        }

        [TestMethod]
        public override void NewObjectOnActionDialogFailInvalid() {
            DoNewObjectOnActionDialogFailInvalid();
        }

        [TestMethod]
        public override void RecentlyViewedOnActionDialogPopup() {
            DoRecentlyViewedOnActionDialogPopup();
        }

        [TestMethod]
        public override void RecentlyViewedOnActionDialogWithSelectPopup() {
            DoRecentlyViewedOnActionDialogWithSelectPopup();
        }

        [TestMethod]
        public override void ActionFindOnActionDialogPopup() {
            DoActionFindOnActionDialogPopup();
        }

        [TestMethod]
        public override void NewObjectOnActionDialogPopup() {
            DoNewObjectOnActionDialogPopup();
        }

        [TestMethod]
        public override void AutoCompleteOnActionDialogPopup() {
            DoAutoCompleteOnActionDialogPopup();
        }

        [TestMethod]
        public override void NewObjectOnActionDialogFailMandatoryPopup() {
            DoNewObjectOnActionDialogFailMandatoryPopup();
        }

        [TestMethod]
        public override void NewObjectOnActionDialogFailInvalidPopup() {
            DoNewObjectOnActionDialogFailInvalidPopup();
        }
    }
}