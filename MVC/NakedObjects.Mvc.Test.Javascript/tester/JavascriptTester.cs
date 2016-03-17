// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Mvc.Test.Javascript {
    [TestClass]
    public class JavaScriptTester {
     

        private static string FilePath(string resourcename) {

            string fileName = resourcename.Remove(0, resourcename.IndexOf(".") + 1); 

            string newFile = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            if (File.Exists(newFile)) {
                File.Delete(newFile);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("NakedObjects.Mvc.Test.Javascript." + resourcename)) {
                using (FileStream fileStream = File.Create(newFile, (int)stream.Length)) {
                    var bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }
            }

            return newFile;
        }

        private static void DeployQunit() {
            FilePath("qunit.qunit.js");
            FilePath("qunit.qunit.css");
            FilePath("qunit.NakedObjects-Ajax.js");
            FilePath("qunit.jquery.mockjax.js");
            FilePath("qunit.jquery.address-1.6.js");
            FilePath("qunit.jquery.json-2.2.js");
            FilePath("qunit.jstorage.js");
            FilePath("qunit.NakedObjectsTestHelpers.js");
            FilePath("qunit.chromedriver.exe");
            FilePath("qunit.IEDriverServer.exe");
        }

        [TestInitialize]
        public void TestInit() {
         
            DeployQunit();
        }

        private static void Clean(ref IWebDriver iwd) {
            if (iwd != null) {
                iwd.Close();
                iwd = null;
            }
        }


        [TestCleanup]
        public void TestClean() {
         
        }
   
        private void RunQUnitTests(IWebDriver iwd, string browser) {
            string fileName = FilePath("qunit.NakedObjectsTest.htm");

            iwd.Navigate().GoToUrl(string.Format("file:///{0}", fileName));

            int timeOut = 200;
            while (iwd.Title != "finished") {
                Thread.Sleep(1000);
                if (timeOut-- <= 0) {
                    break;
                }
            }

            AssertQUnitTestResults(iwd, browser);
        }


        //[TestMethod] 
        //public void RunFfQUnitTests() {
        //    using (var ff = new FirefoxDriver()) {
        //        // if don't do this focus won't go to element in browser window
        //        ff.Keyboard.SendKeys(Keys.Tab);
        //        ff.Keyboard.SendKeys(Keys.Tab);

        //        RunQUnitTests(ff, "ff");
        //    }
        //}

        [TestMethod] 
        public void RunIeQUnitTests() {
            using (var ie = new InternetExplorerDriver()) {
              
                RunQUnitTests(ie, "ie");
                ie.Quit();
            }
        }

        //[TestMethod]
        //public void RunCrQUnitTests() {
        //    using (var cr = new ChromeDriver()) {
        //        RunQUnitTests(cr, "cr");
        //        cr.Quit();
        //    }
        //}
       
        private void AssertQUnitTestResults(IWebDriver iwd, string browser) {

            var liElements = iwd.FindElements(By.CssSelector("#qunit-tests > li"));    

            if (!liElements.Any()) {
                Assert.Fail("No test results");
            }

            // write a testresults file 

            var results = iwd.FindElement(By.Id("qunit-testresult"));

            var passed = results.FindElement(By.ClassName("passed")).Text;
            var total = results.FindElement(By.ClassName("total")).Text;
            var failures =results.FindElement(By.ClassName("failed")).Text; 
            var tests = liElements.Count();
            var failed = liElements.Count(x => x.GetAttribute("class") != "pass");
            var name = string.Format("javascript-test-{0}", browser);

            if (failed > 0) {
                var failedTests = liElements.Where(li => li.GetAttribute("class") != "pass").Select(li => li.Text);
                var failedMessage = failedTests.Aggregate("Failed Tests", (s, t) => s + " : " + t);
                throw new AssertFailedException(failedMessage);
            }



            //var xml = new XElement("test-results",
            //                       new XAttribute("name", name),
            //                       new XAttribute("total", tests),
            //                       new XAttribute("errors", 0),
            //                       new XAttribute("failures", failed),
            //                       new XAttribute("not-run", 0),
            //                       new XAttribute("inconclusive", 0),
            //                       new XAttribute("ignored", 0),
            //                       new XAttribute("skipped", 0),
            //                       new XAttribute("invalid", 0),
            //                       new XAttribute("date", DateTime.Now.Date.ToShortDateString()),
            //                       new XAttribute("time", DateTime.Now.ToShortTimeString()),
            //                       new XElement("environment",
            //                                    new XAttribute("nunit-version", "na"),
            //                                    new XAttribute("clr-version", "na"),
            //                                    new XAttribute("os-version", "na"),
            //                                    new XAttribute("platform", "na"),
            //                                    new XAttribute("cwd", "na"),
            //                                    new XAttribute("machine-name", "na"),
            //                                    new XAttribute("user", "na"),
            //                                    new XAttribute("user-domain", "na")),
            //                       new XElement("culture-info",
            //                                    new XAttribute("current-culture", "na"),
            //                                    new XAttribute("current-uiculture", "na")),
            //                       new XElement("test-suite",
            //                                    new XAttribute("type", "Assembly"),
            //                                    new XAttribute("name", name),
            //                                    new XAttribute("executed", "True"),
            //                                    new XAttribute("result", failed > 0 ? "Failure" : "Success"),
            //                                    new XAttribute("success", failed > 0 ? "False" : "True"),
            //                                    new XAttribute("time", 0),
            //                                    new XAttribute("asserts", 0),
            //                                    new XElement("results",
            //                                                   new XElement("test-suite",
            //                                                                new XAttribute("type", "NameSpace"),
            //                                                                new XAttribute("name", "Javascript"),
            //                                                                new XAttribute("executed", "True"),
            //                                                                new XAttribute("result", failed > 0 ? "Failure" : "Success"),
            //                                                                new XAttribute("success", failed > 0 ? "False" : "True"),
            //                                                                new XAttribute("time", 0),
            //                                                                new XAttribute("asserts", 0),
            //                                                                new XElement("results",
            //                                                             new XElement("test-suite",
            //                                                                          new XAttribute("type", "TestFixture"),
            //                                                                          new XAttribute("name", name),
            //                                                                          new XAttribute("executed", "True"),
            //                                                                          new XAttribute("result", failed > 0 ? "Failure" : "Success"),
            //                                                                          new XAttribute("success", failed > 0 ? "False" : "True"),
            //                                                                          new XAttribute("time", 0),
            //                                                                          new XAttribute("asserts", 1),
            //                                                                          new XElement("results",
            //                                                                                       from liElement in liElements
            //                                                                                       select new XElement("test-case",
            //                                                                                                           new XAttribute("name", liElement.FindElement(By.CssSelector(".test-name")).Text),
            //                                                                                                           new XAttribute("executed", "True"),
            //                                                                                                           new XAttribute("result", liElement.GetAttribute("class") == "pass" ? "Success" : "Failure"),
            //                                                                                                           new XAttribute("success", liElement.GetAttribute("class") == "pass" ? "True" : "False"),
            //                                                                                                           new XAttribute("time", 0),
            //                                                                                                           new XAttribute("asserts", 1),
            //                                                                                                           liElement.GetAttribute("class") == "pass" ? null : new XElement("Failure", 
            //                                                                                                               new XElement("message", "failed"), 
            //                                                                                                               new XElement("stack-trace", "no trace"))))))))));

            //var doc = new XDocument(xml);

           

            //WriteXmlFile(doc, Path.Combine(GetTestResultsDirectory(), string.Format("javascript-test-{0}.xml", browser)));
        }

        private static void WriteXmlFile(XNode doc, string fileName) {
            var fileInfo = new FileInfo(fileName);

            using (FileStream fileStream = fileInfo.OpenWrite()) {
                using (XmlWriter xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings {Indent = true})) {
                    doc.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                }
            }
        }

        private static string GetTestResultsDirectory() {

            var di = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (di != null && di.Name != "test-results") {
                di = di.Parent;
            }
            return   di == null ? Directory.GetCurrentDirectory() :  di.FullName; 
        }

    }
}