// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
    /// <summary>
    ///     Tests use of a drop down list that is dependent upon selection or entry made in another field.
    /// </summary>
    public abstract class ImageAndAttachmentTests : AWWebTest {
        public abstract void ViewImage();

        public void DoViewImage() {
            Login();
            FindProduct("FR-M94B-38");
            Assert.AreEqual("HL Mountain Frame - Black, 38", br.Title);
            var photoField = br.FindElement(By.CssSelector("#Product-Photo"));

            var alink = photoField.FindElement(By.CssSelector("a"));
            Assert.AreEqual(url + "/Product/GetFile/frame_black_large.gif?Id=AdventureWorksModel.Product%3B1%3BSystem.Int32%3B747%3BFalse%3B%3B0&PropertyId=Photo",
                alink.GetAttribute("href"));

            var img = photoField.FindElement(By.CssSelector("img"));
            Assert.AreEqual("frame_black_large.gif", img.GetAttribute("alt"));
            Assert.AreEqual(url + "/Product/GetFile/frame_black_large.gif?Id=AdventureWorksModel.Product%3B1%3BSystem.Int32%3B747%3BFalse%3B%3B0&PropertyId=Photo",
                img.GetAttribute("src"));
        }

        public void DoDownloadImage() {
            Login();
            FindProduct("FR-M94B-38");
            Assert.AreEqual("HL Mountain Frame - Black, 38", br.Title);
            var photoLink = br.FindElement(By.CssSelector("#Product-Photo a"));

            photoLink.Click();

            Assert.AreEqual("(GIF Image, 240 × 149 pixels)", br.Title);
            Assert.IsTrue(br.Url.StartsWith("blob"));
        }

        public void DoUploadImage() {
            Login();
            FindProduct("FR-M94B-38");
            Assert.AreEqual("HL Mountain Frame - Black, 38", br.Title);

            var fileinput = wait.ClickAndWait("#Product-AddOrChangePhoto button", "#Product-AddOrChangePhoto-NewImage-Input");

            var file = Path.Combine(Directory.GetCurrentDirectory(), "testimage.jpg");

            fileinput.SendKeys(file);

            wait.ClickAndWaitGone(".nof-ok", ".nof-ok");

            var alink = br.FindElement(By.CssSelector("#Product-Photo a"));
            Assert.AreEqual(url + "/Product/GetFile/testimage.jpg?Id=AdventureWorksModel.Product%3B1%3BSystem.Int32%3B747%3BFalse%3B%3B0&PropertyId=Photo",
                alink.GetAttribute("href"));
        }
    }
}