// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.IE;

namespace NakedObjects.Mvc.Selenium.Test.InternetExplorer {
    //[TestClass]
    public class AjaxTestsIE : AjaxTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            AjaxTests.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new InternetExplorerDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUpTest();
        }

        //[TestMethod]
        public override void RemoteValidationProperty() {
            DoRemoteValidationProperty();
        }

        //[TestMethod]
        public override void RemoteValidationParameterNoPopup() {
            DoRemoteValidationParameterNoPopup();
        }

        //[TestMethod]
        public override void RemoteValidationParameterPopup() {
            DoRemoteValidationParameterPopup();
        }

        //[TestMethod]
        public override void ActionChoicesNoPopup() {
            DoActionChoicesNoPopup();
        }

        //[TestMethod]
        public override void ActionChoicesPopup() {
            DoActionChoicesPopup();
        }

        //[TestMethod]
        public override void ActionMultipleChoices() {
            DoActionMultipleChoices();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesEnum() {
            DoActionMultipleChoicesEnum();
        }

        //[TestMethod]
        public override void ActionConditionalMultipleChoices() {
            DoActionConditionalMultipleChoices();
        }

        //[TestMethod]
        public override void ActionCrossValidateFailNoPopup() {
            DoActionCrossValidateFailNoPopup();
        }

        //[TestMethod]
        public override void ActionCrossValidateFailPopup() {
            DoActionCrossValidateFailPopup();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesPopup() {
            DoActionMultipleChoicesPopup();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesPopupDefaults() {
            DoActionMultipleChoicesPopupDefaults();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesNoPopUpEnum() {
            DoActionMultipleChoicesNoPopupEnum();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesPopupEnum() {
            DoActionMultipleChoicesPopupEnum();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesPopupConditionalEnum() {
            DoActionMultipleChoicesPopupConditionalEnum();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesNoPopUpDomainObject() {
            DoActionMultipleChoicesNoPopupDomainObject();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesPopupDomainObject() {
            DoActionMultipleChoicesPopupDomainObject();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesValidateFail() {
            DoActionMultipleChoicesValidateFail();
        }

        //[TestMethod]
        public override void ActionMultipleChoicesPopupValidateFail() {
            DoActionMultipleChoicesPopupValidateFail();
        }

        //[TestMethod]
        public override void ClientSideValidation() {
            DoClientSideValidation();
        }

        //[TestMethod]
        public override void CanGoBackToDialog() {
            DoCanGoBackToDialog();
        }

        //[TestMethod]
        public override void GoingBackToDialogPreservesEnteredValues() {
            DoGoingBackToDialogPreservesEnteredValues();
        }
    }
}