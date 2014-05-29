// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
    // Use this template to create new test classes, following these steps:
    // 1. Copy this template and replace 'MyTests' with new test class name throughout
    // 2. Write all test logic in the abstract class, in the form of an abstract method 
    //    (no annotation) and a corresponding 'Do' method (see comment in class)
    // 3. Make MyTestsIE implement the abstract MyTests class.  Each created method should be
    //    annotated [TestMethod] and simply delegate to the inherited 'Do' method.
    //    The following Regex will do this automatically:
    //    Find:     ^{.*}public override void {.*}\n{.*\{}\n{.*}{throw .*}$
    //    Replace:  \1\[TestMethod\]\n\1public override void \2\n\3\n\4Do\2;
    // 4. When IE tests run OK, uncomment the Firefox and/or Chrome classes and repeat step 3
    public abstract class MyTests : AWWebTest {
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        //public abstract void Test1();

        //protected void DoTest1() { 
        //  Common test logic here
        //}
    }

    [TestClass]
    public class MyTestsIE : MyTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            MyTests.InitialiseClass(context);
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

        //[TestMethod]
        //public override void Test1()
        //{
        //    DoTest1();
        //}
    }

    [TestClass]
    public class MyTestsFirefox : MyTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            MyTests.InitialiseClass(context);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            br.Navigate().GoToUrl(url);
        }
    }

    [TestClass]
    public class MyTestsChrome : MyTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            MyTests.InitialiseClass(context);
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
    }
}