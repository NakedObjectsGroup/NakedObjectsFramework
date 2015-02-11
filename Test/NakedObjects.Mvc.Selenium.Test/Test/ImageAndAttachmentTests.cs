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
            br.AssertPageTitleEquals("HL Mountain Frame - Black, 38");
            IWebElement photoField = br.GetField("Product-Photo");

            IWebElement alink = photoField.FindElement(By.CssSelector("a"));
            Assert.AreEqual(Path.Combine(url, "Product/GetFile/frame_black_large.gif?Id=AdventureWorksModel.Product%3B1%3BSystem.Int32%3B747%3BFalse%3B%3B0&PropertyId=Photo"),
                alink.GetAttribute("href"));

            IWebElement img = photoField.FindElement(By.CssSelector("img"));
            Assert.AreEqual("frame_black_large.gif", img.GetAttribute("alt"));
            Assert.AreEqual(Path.Combine(url, "Product/GetFile/frame_black_large.gif?Id=AdventureWorksModel.Product%3B1%3BSystem.Int32%3B747%3BFalse%3B%3B0&PropertyId=Photo"),
                img.GetAttribute("src"));
        }

        ////[TestMethod]
        //public void UploadImage() {
        //    FindProduct("FR-M94B-38");
        //    br.AssertPageTitleEquals("HL Mountain Frame - Black, 38");
        //    var img = br.GetField("Product-Photo").FindElement(By.CssSelector("img"));
        //    Assert.AreEqual("frame_black_large.gif", img.GetAttribute("alt"));

        //    var change = br.ClickAction("Product-AddOrChangePhoto");
        //    change.GetField("Product-AddOrChangePhoto-NewImage").TypeText(@"Z:\FilesUsedByTests\small.jpg", br);
        //    br.ClickOk();

        //    img = br.GetField("Product-Photo").FindElement(By.CssSelector("img"));
        //    img = br.FindElement(By.CssSelector("img"));
        //    Assert.AreEqual("small.jpg", img.GetAttribute("alt"));

        //}
    }
}