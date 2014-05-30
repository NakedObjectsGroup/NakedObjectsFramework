// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Web.UnitTests.Selenium {
    public class SafeWebDriverWait : IWait<IWebDriver> {
        private readonly WebDriverWait wait;

        public SafeWebDriverWait(IWebDriver driver, TimeSpan timeout) {
            wait = new WebDriverWait(driver, timeout);
        }

        public void IgnoreExceptionTypes(params Type[] exceptionTypes) {
            wait.IgnoreExceptionTypes(exceptionTypes);
        }

        public TResult Until<TResult>(Func<IWebDriver, TResult> condition) {
            return wait.Until(d => {
                try {
                    return condition(d);
                }
                catch (NoSuchElementException) {}
                return default(TResult);
            });
        }

        public TimeSpan Timeout {
            get { return wait.Timeout; }
            set { wait.Timeout = value; }
        }

        public TimeSpan PollingInterval {
            get { return wait.PollingInterval; }
            set { wait.PollingInterval = value; }
        }

        public string Message {
            get { return wait.Message; }
            set { wait.Message = value; }
        }
    }


    [TestClass]
    public abstract class SpiroTest {
        #region overhead

        protected const string url = "http://mvc.nakedobjects.net:1081/UnitTestSpiroNg/index.html";
        protected const string server = @"Saturn\SqlExpress";
        protected const string database = "AdventureWorks";
        protected const string backup = "AdventureWorks";

        protected const string customerServiceUrl = url + "#/services/AdventureWorksModel.CustomerRepository";
        protected const string orderServiceUrl = url + "#/services/AdventureWorksModel.OrderRepository";
        protected const string productServiceUrl = url + "#/services/AdventureWorksModel.ProductRepository";
        protected const string salesServiceUrl = url + "#/services/AdventureWorksModel.SalesRepository";

        protected const string store555Url = url + "#/objects/AdventureWorksModel.Store/555";
        protected const string product968Url = url + "#/objects/AdventureWorksModel.Product/968";

        //protected const string url = "http://localhost:53103/";
        //protected const string server = @".\SQLEXPRESS";
        //protected const string database = "AdventureWorks";
        //protected const string backup = "AdventureWorksInitialState";

        protected IWebDriver br;
        protected SafeWebDriverWait wait;

        protected const int TimeOut = 20; 

        [ClassInitialize]
        public static void InitialiseClass(TestContext context) {
            // DatabaseUtils.RestoreDatabase(database, backup, server);
        }

        public virtual void CleanUpTest() {
            if (br != null) {
                try {
                    br.Manage().Cookies.DeleteAllCookies();
                    br.Quit();
                    br.Dispose();
                    br = null;
                }
                catch {
                    // to suppress error 
                }
            }
        }

        protected void InitFirefoxDriver() {
            br = new FirefoxDriver();
            wait = new SafeWebDriverWait(br, TimeSpan.FromSeconds(TimeOut));
            br.Manage().Window.Maximize();
        }

        protected void InitIeDriver() {
            br = new InternetExplorerDriver();
            wait = new SafeWebDriverWait(br, TimeSpan.FromSeconds(TimeOut));
            br.Manage().Window.Maximize();
        }

        protected void InitChromeDriver() {
            const string cacheDir = @"C:\SeleniumTestFolder";

            var crOptions = new ChromeOptions();
            crOptions.AddArgument(@"--user-data-dir=" + cacheDir);
            br = new ChromeDriver(crOptions);
            wait = new SafeWebDriverWait(br, TimeSpan.FromSeconds(TimeOut));
            br.Manage().Window.Maximize();

            // test workaround for chromedriver problem https://groups.google.com/forum/#!topic/selenium-users/nJ0NF1UJ3WU
            Thread.Sleep(5000);
        }

        #endregion

        #region Helpers

        protected void WaitUntilGone<TResult>(Func<IWebDriver, TResult> condition) {

            wait.Until(d => {
                try {
                    condition(d);
                    return false;
                }
                catch (NoSuchElementException) {
                    return true;
                }
            });
            
        }


        protected virtual void Maximize() {
            const string script = "window.moveTo(0, 0); window.resizeTo(screen.availWidth, screen.availHeight);";
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }

        protected virtual void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }

        protected virtual void Click(IWebElement element) {
            ScrollTo(element);
            element.Click();
        }


        protected virtual void GoToServiceFromHomePage(string serviceName) {
            wait.Until(d => d.FindElements(By.ClassName("service")).Count == 12);
            ReadOnlyCollection<IWebElement> services = br.FindElements(By.CssSelector("div.service > a"));
            IWebElement service = services.FirstOrDefault(s => s.Text == serviceName);
            if (service != null) {
                Click(service);
                wait.Until(d => d.FindElements(By.CssSelector(".actions-pane .actions")).Count > 0);
            }
            else {
                throw new ObjectNotFoundException(string.Format("service not found {0}", serviceName));
            }
        }

        protected void Login() {
            Thread.Sleep(2000);
        }

        #endregion

        #region chrome helper

        protected static string FilePath(string resourcename) {
            string fileName = resourcename.Remove(0, resourcename.IndexOf(".") + 1);

            string newFile = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            if (File.Exists(newFile)) {
                File.Delete(newFile);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("Spiro.Angular.Selenium.Test." + resourcename)) {
                using (FileStream fileStream = File.Create(newFile, (int) stream.Length)) {
                    var bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }
            }

            return newFile;
        }

        #endregion
    }
}