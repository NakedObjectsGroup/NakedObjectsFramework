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

    public class ImageAndAttachmentTestsFirefox : ImageAndAttachmentTests {
        [ClassInitialize]
        public static void InitialiseClass(TestContext context) {
            Reset(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            FilePath("testimage.jpg");
            br = new FirefoxDriver();
            wait = new SafeWebDriverWait(br, DefaultTimeOut);
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUp();
        }

        [TestMethod]
        public override void ViewImage() {
            DoViewImage();
        }

        [TestMethod]

        public void DownloadImage() {
            DoDownloadImage();
        }

        [TestMethod]

        public void ZzUploadImage() {
            DoUploadImage();
        }
    }
}