// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
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


        //[TestMethod]
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

    [TestClass, Ignore]
    public class ImageAndAttachmentTestsIE : ImageAndAttachmentTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new InternetExplorerDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        [TestMethod]
        public override void ViewImage() {
            DoViewImage();
        }
    }

    [TestClass]
    public class ImageAndAttachmentTestsFirefox : ImageAndAttachmentTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            br.Navigate().GoToUrl(url);
        }


        [TestMethod]
        public override void ViewImage() {
            DoViewImage();
        }
    }

    [TestClass, Ignore]
    public class ImageAndAttachmentTestsChrome : ImageAndAttachmentTests {
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

        [TestMethod]
        public override void ViewImage() {
            DoViewImage();
        }
    }
}