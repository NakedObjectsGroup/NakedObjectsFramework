// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.ObjectModel;
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

        public TimeSpan Timeout
        {
            get { return wait.Timeout; }
            set { wait.Timeout = value; }
        }

        public TimeSpan PollingInterval
        {
            get { return wait.PollingInterval; }
            set { wait.PollingInterval = value; }
        }

        public string Message
        {
            get { return wait.Message; }
            set { wait.Message = value; }
        }
    }

    public abstract class GeminiTest {
        #region overhead

        protected const string Url = "http://localhost:49998/index.html";

        protected const string Server = @"Saturn\SqlExpress";
        protected const string Database = "AdventureWorks";
        protected const string Backup = "AdventureWorks";

        protected const string CustomersMenuUrl = Url + "#/home?menu1=CustomerRepository";
        protected const string OrdersMenuUrl = Url + "#/home?menu1=OrderRepository";
        protected const string SpecialOffersMenuUrl = Url + "#/home?menu1=SpecialOfferRepository";
        protected const string ProductServiceUrl = Url + "#/home?menu1=ProductRepository";
        protected const string SalesServiceUrl = Url + "#/home?menu1=SalesRepository";

        protected const int MainMenusCount = 11; //TODO: Should be 10 as Empty menu should not show

        protected const int CustomerServiceActions = 9;
        protected const int OrderServiceActions = 6;
        protected const int ProductServiceActions = 12;
        protected const int SalesServiceActions = 4;

        protected const string Store555UrlWithActionsMenuOpen = Url + "#/object?object1=AdventureWorksModel.Store-555&actions1=open";
        protected const string Product968Url = Url + "#/object?object1=AdventureWorksModel.Product-968";
        protected const string Product469Url = Url + "#/object?object1=AdventureWorksModel.Product-469";
        protected const string Product870Url = Url + "#/object?object1=AdventureWorksModel.Product-870";

        protected const int StoreActions = 8;
        protected const int StoreProperties = 6;
        protected const int StoreCollections = 2;
        protected const int ProductActions = 6;
        protected const int ProductProperties = 23;

        //protected const string url = "http://localhost:53103/";
        //protected const string server = @".\SQLEXPRESS";
        //protected const string database = "AdventureWorks";
        //protected const string backup = "AdventureWorksInitialState";

        protected IWebDriver br;
        protected SafeWebDriverWait wait;

        protected static int timeOut = 0;

        protected static int TimeOut {
            get {
                if (timeOut != 0) { return timeOut; }
                timeOut = 20;
                return 40;
            }
        }

        [ClassInitialize]
        public static void InitialiseClass(TestContext context) {
            //DatabaseUtils.RestoreDatabase(Database, Backup, Server);
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

        protected virtual void RightClick(IWebElement element)
        {
            throw new NotImplementedException();
            //var webDriver = wait.Driver;
            //ScrollTo(element);
            //var loc = (ILocatable)element;
            //var mouse = ((IHasInputDevices)webDriver).Mouse;
            //mouse.ContextClick(loc.Coordinates);
        }

        protected virtual IWebElement WaitForCss(string cssSelector)
        {
            wait.Until(d => d.FindElement(By.CssSelector(cssSelector)));
            return br.FindElement(By.CssSelector(cssSelector));
        }

        protected virtual IWebElement FindElementByCss(string cssSelector)
        {
            wait.Until(d => d.FindElement(By.CssSelector(cssSelector)));
            return br.FindElement(By.CssSelector(cssSelector));
        }

        protected virtual IWebElement FindElementByCss(string cssSelector, int number)
        {
            wait.Until(d => d.FindElements(By.CssSelector(cssSelector)).Count >= number+1);
            return br.FindElements(By.CssSelector(cssSelector))[number];
        }

        protected virtual void GoToMenuFromHomePage(string menuName) {
            WaitForSingleHome();
            ReadOnlyCollection<IWebElement> menus = br.FindElements(By.CssSelector(".menu"));
            IWebElement menu = menus.FirstOrDefault(s => s.Text == menuName);
            if (menu != null) {
                Click(menu);
                wait.Until(d => d.FindElements(By.CssSelector(".actions .action")).Count > 0);
            }
            else {
                throw new NotFoundException(string.Format("menu not found {0}", menuName));
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

            using (Stream stream = assembly.GetManifestResourceStream("NakedObjects.Spa.Selenium.Test." + resourcename)) {
                using (FileStream fileStream = File.Create(newFile, (int) stream.Length)) {
                    var bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }
            }

            return newFile;
        }

        #endregion

        #region Resulting page view
        protected virtual void WaitForSingleObject(string title = null)
        {
            var titleEl = wait.Until(dr => dr.FindElement(By.CssSelector(".single .object .header .title")));
            if (title != null)
            {
                Assert.AreEqual(title, titleEl.Text);
            }
            wait.Until(dr => dr.FindElement(By.CssSelector(".single .object .properties")));
            Assert.AreEqual("Actions", FindElementByCss(".single .object .header .menu").Text);
            AssertFooterExists();
        }

        protected virtual void AssertFooterExists()
        {
            wait.Until(d => d.FindElement(By.CssSelector(".footer")));
            Assert.IsTrue(br.FindElement(By.CssSelector(".footer .icon-home")).Displayed);
            Assert.IsTrue(br.FindElement(By.CssSelector(".footer .icon-back")).Displayed);
            Assert.IsTrue(br.FindElement(By.CssSelector(".footer .icon-forward")).Displayed);
        }

        protected void WaitForSingleQuery(string title = null)
        {
            var titleEl = wait.Until(dr => dr.FindElement(By.CssSelector(".single .query .header .title")));
            if (title != null)
            {
                Assert.AreEqual(title, titleEl.Text);
            }
            wait.Until(dr => dr.FindElement(By.CssSelector(".single .query .collection")));
            Assert.AreEqual("Actions", FindElementByCss(".single .query .header .menu").Text);
            AssertFooterExists();
        }

        protected void AssertTopItemInListIs(string title)
        {
            string topItem = FindElementByCss(".collection tr td.reference").Text;

            Assert.AreEqual(title, topItem);
        }

        protected void WaitForSingleHome()
        {
            var titleEl = wait.Until(dr => dr.FindElement(By.CssSelector(".single .home .header .title")));
            wait.Until(d => d.FindElements(By.CssSelector(".menu")).Count == MainMenusCount);
            Assert.AreEqual("Home", titleEl.Text);
            Assert.IsNotNull(br.FindElement(By.CssSelector(".main-column")));

            ReadOnlyCollection<IWebElement> menus = br.FindElements(By.CssSelector(".menu"));
            Assert.AreEqual("Customers", menus[0].Text);
            Assert.AreEqual("Orders", menus[1].Text);
            Assert.AreEqual("Products", menus[2].Text);
            Assert.AreEqual("Employees", menus[3].Text);
            Assert.AreEqual("Sales", menus[4].Text);
            Assert.AreEqual("Special Offers", menus[5].Text);
            Assert.AreEqual("Contacts", menus[6].Text);
            Assert.AreEqual("Vendors", menus[7].Text);
            Assert.AreEqual("Purchase Orders", menus[8].Text);
            Assert.AreEqual("Work Orders", menus[9].Text);
            AssertFooterExists();
        }

        protected void WaitForSplit(PaneType pane1Type, PaneType pane2Type,string title1 = null, string title2=null )
        {
            var pane1 = wait.Until(dr => dr.FindElement(By.CssSelector("#pane1 ."+ pane1Type.ToString().ToLower())));
            if (title1 != null)
            {
                Assert.AreEqual(title1, pane1.FindElement(By.CssSelector(".title")).Text);
            }
            var pane2 =wait.Until(dr => dr.FindElement(By.CssSelector("#pane2 ." + pane2Type.ToString().ToLower())));
            if (title2 != null)
            {
                Assert.AreEqual(title2, pane2.FindElement(By.CssSelector(".title")).Text);
            }
        }
        #endregion
        protected void AssertElementExists(string cssSelector)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count >= 1);
        }

        protected void AssertElementDoesNotExist(string cssSelector)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == 0);
        }

        protected void AssertElementCountIs(string cssSelector, int count)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == count);
        }

        #region Editing & Saving
        protected void EditObject()
        {
            Click(GetEditButton());
            GetSaveButton();
            GetCancelEditButton();
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsTrue(title.StartsWith("Editing"));
        }

        protected void SaveObject()
        {
            Click(GetSaveButton());
            GetEditButton(); //To wait for save completed
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsFalse(title.StartsWith("Editing"));
        }

        protected IWebElement GetEditButton()
        {
            wait.Until(d => d.FindElements(By.CssSelector(".header .action")).Count == 1);
            var edit = br.FindElement(By.CssSelector(".header .action"));
            Assert.AreEqual("Edit", edit.Text);
            return edit;
        }

        protected IWebElement GetSaveButton()
        {
            wait.Until(d => d.FindElements(By.CssSelector(".header .action")).Count == 2);
            var save = br.FindElements(By.CssSelector(".header .action"))[0];
            Assert.AreEqual("Save", save.Text);
            return save;
        }

        protected IWebElement GetCancelEditButton()
        {
            wait.Until(d => d.FindElements(By.CssSelector(".header .action")).Count == 2);
            var cancel = br.FindElements(By.CssSelector(".header .action"))[1];
            Assert.AreEqual("Cancel", cancel.Text);
            return cancel;
        }
        #endregion

        #region Object Actions
        protected ReadOnlyCollection<IWebElement> GetObjectActions(int? totalNumber = null)
        {
            if (totalNumber == null)
            {
                wait.Until(d => d.FindElements(By.CssSelector(".actions .action")).Count > 0);
            }
            else
            {
                wait.Until(d => d.FindElements(By.CssSelector(".actions .action")).Count == totalNumber.Value);
            }
            return br.FindElements(By.CssSelector(".actions .action"));
        }

        protected IWebElement GetObjectAction(string actionName)
        {
            return GetObjectActions().Where(a => a.Text == actionName).Single();
        }

        protected virtual void ClickAction(string name)
        {
            Click(GetObjectAction(name));
        }

        protected IWebElement OpenActionDialog(string actionName)
        {
            Click(GetObjectAction(actionName));
            var dialog = wait.Until(d => d.FindElement(By.CssSelector(".dialog")));
            string title = br.FindElement(By.CssSelector(".dialog > .title")).Text;
            Assert.AreEqual(actionName, title);
            //Check it has OK & cancel buttons
            wait.Until(d => br.FindElement(By.CssSelector(".ok")));
            wait.Until(d => br.FindElement(By.ClassName("cancel")));
            return dialog;
        }

        protected void ClickOK()
        {
            wait.Until(d => br.FindElement(By.CssSelector(".dialog .ok")));
            Click(br.FindElement(By.CssSelector(".ok")));
        }

        protected void CancelDialog()
        {
             Click(br.FindElement(By.CssSelector(".dialog  .cancel")));

                wait.Until(d => {
                    try
                    {
                        br.FindElement(By.ClassName("dialog"));
                        return false;
                    }
                    catch (NoSuchElementException)
                    {
                        return true;
                    }
                });

        }

        #endregion
    }

    public enum PaneType
    {
        Home = 1,
        Object = 2,
        Query = 3,
        Recent = 4
    }
}