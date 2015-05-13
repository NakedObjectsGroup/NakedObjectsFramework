// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium.Firefox;

namespace NakedObjects.Mvc.Selenium.Test.Firefox {
    [TestClass]

    public class ObjectEditTestsFirefox : ObjectEditTests {
        [ClassInitialize]
        public static void InitialiseClass(TestContext context) {
            Reset(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            wait = new SafeWebDriverWait(br, DefaultTimeOut);
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUp();
        }

        [TestMethod]
        public override void EditPersistedObject() {
            DoEditPersistedObject();
        }

        [TestMethod]
        public override void EditTableHeader() {
            DoEditTableHeader();
        }

        [TestMethod]
        public override void CancelButtonOnObjectEdit() {
            DoCancelButtonOnObjectEdit();
        }

        [TestMethod]
        [Ignore] // todo fix

        public override void SaveWithNoChanges() {
            DoSaveWithNoChanges();
        }

        [TestMethod]
        [Ignore] // todo fix

        public override void ChangeStringField() {
            DoChangeStringField();
        }

        [TestMethod]
        [Ignore] // todo fix
        public override void ChangeDropDownField() {
            DoChangeDropDownField();
        }

        [TestMethod]
        [Ignore] // todo fix

        public override void ChangeReferencePropertyViaRecentlyViewed() {
            DoChangeReferencePropertyViaRecentlyViewed();
        }

        [TestMethod]
        [Ignore] // todo fix

        public override void ChangeReferencePropertyViaRemove() {
            DoChangeReferencePropertyViaRemove();
        }

        [TestMethod]
        [Ignore] // todo fix

        public override void ChangeReferencePropertyViaAFindAction() {
            DoChangeReferencePropertyViaAFindAction();
        }

        [TestMethod]
        public override void ChangeReferencePropertyViaNewAction() {
            DoChangeReferencePropertyViaANewAction();
        }

        [TestMethod]
        public override void ChangeReferencePropertyViaAutoComplete() {
            DoChangeReferencePropertyViaAutoComplete();
        }

        [TestMethod]
        public override void ChangeReferencePropertyViaANewActionFailMandatory() {
            DoChangeReferencePropertyViaANewActionFailMandatory();
        }

        [TestMethod]
        public override void ChangeReferencePropertyViaANewActionFailInvalid() {
            DoChangeReferencePropertyViaANewActionFailInvalid();
        }

        [TestMethod]
        public override void CheckDefaultsOnFindAction() {
            DoCheckDefaultsOnFindAction();
        }

        [TestMethod]
        public override void NoEditButtonWhenNoEditableFields() {
            DoNoEditButtonWhenNoEditableFields();
        }

        [TestMethod]
        public override void Refresh() {
            DoRefresh();
        }

        [TestMethod]
        [Ignore] // todo fix

        public override void NoValidationOnTransientUntilSave() {
            DoNoValidationOnTransientUntilSave();
        }
    }
}