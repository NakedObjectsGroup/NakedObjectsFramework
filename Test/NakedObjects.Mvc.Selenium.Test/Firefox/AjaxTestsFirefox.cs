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

    public class AjaxTestsFirefox : AjaxTests {
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
        public override void RemoteValidationProperty() {
            DoRemoteValidationProperty();
        }

        [TestMethod]
        public override void RemoteValidationParameter() {
            DoRemoteValidationParameter();
        }

        [TestMethod]
        public override void ActionChoices() {
            DoActionChoices();
        }

        [TestMethod]
        public override void ActionMultipleChoices() {
            DoActionMultipleChoices();
        }

        [TestMethod]

        public override void ActionConditionalMultipleChoices() {
            DoActionConditionalMultipleChoices();
        }

        [TestMethod]

        public override void ActionCrossValidateFail() {
            DoActionCrossValidateFail();
        }

        [TestMethod]

        public override void ActionMultipleChoicesDefaults() {
            DoActionMultipleChoicesDefaults();
        }

        [TestMethod]
        public override void ActionMultipleChoicesEnum() {
            DoActionMultipleChoicesEnum();
        }

        [TestMethod]

        public override void ActionMultipleChoicesConditionalEnum() {
            DoActionMultipleChoicesConditionalEnum();
        }

        [TestMethod]
        public override void ActionMultipleChoicesValidateFail() {
            DoActionMultipleChoicesValidateFail();
        }

        [TestMethod]

        public override void ActionMultipleChoicesDomainObject() {
            DoActionMultipleChoicesDomainObject();
        }

        [TestMethod]
        public override void ClientSideValidation() {
            DoClientSideValidation();
        }
    }
}