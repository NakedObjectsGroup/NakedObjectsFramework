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

    public abstract class GeminiTest {
        #region overhead

        protected const string BaseUrl = TestConfig.BaseUrl;
        protected const string GeminiBaseUrl = TestConfig.BaseUrl+ "#/gemini/";

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

        protected void Url(string url)
        {
            br.Navigate().GoToUrl(url);
        }

        protected void GeminiUrl(string url)
        {
            br.Navigate().GoToUrl(GeminiBaseUrl+ url);
        }

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
            
            var webDriver = wait.Driver;
            ScrollTo(element);
            var loc = (ILocatable)element;
            var mouse = ((IHasInputDevices)webDriver).Mouse;
            mouse.ContextClick(loc.Coordinates);
        }

        protected virtual IWebElement WaitForCss(string cssSelector)
        {
            return wait.Until(d => d.FindElement(By.CssSelector(cssSelector)));
        }

        /// <summary>
        /// Waits until there are AT LEAST the specified count of matches & returns ALL matches
        /// </summary>
        protected virtual ReadOnlyCollection<IWebElement> WaitForCss(string cssSelector, int count)
        {
            wait.Until(d => d.FindElements(By.CssSelector(cssSelector)).Count >= count);
            return br.FindElements(By.CssSelector(cssSelector));
        }

        /// <summary>
        /// Waits for the Nth match and returns it (counting from zero).
        /// </summary>
        protected virtual IWebElement WaitForCssNo(string cssSelector, int number)
        {
            return WaitForCss(cssSelector, number + 1)[number];
        }

        protected virtual void TypeIntoField(string cssFieldId, string characters)
        {
            WaitForCss(cssFieldId).SendKeys(characters);
        }

        protected virtual void SelectDropDownOnField(string cssFieldId, string characters)
        {
            var selected = new SelectElement(WaitForCss(cssFieldId));
            selected.SelectByText(characters);
        }

        protected virtual void SelectDropDownOnField(string cssFieldId, int index)
        {
            var selected = new SelectElement(WaitForCss(cssFieldId));
            selected.SelectByIndex(index);
        }

        protected virtual void GoToMenuFromHomePage(string menuName) {
            WaitForView(Pane.Single, PaneType.Home, "Home");
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
        protected enum Pane
        {
            Single,
            Left,
            Right
        }

        protected enum PaneType
        {
            Home,
            Object,
            List
        }

        protected enum ClickType
        {
            Left,
            Right
        }

        protected IWebElement GetReferenceProperty(string propertyName, string refTitle, Pane pane = Pane.Single) {
            string propCss = CssSelectorFor(pane) + " " + ".property";
            var prop = wait.Until(dr => dr.FindElements(By.CssSelector(propCss))
                    .Where(we => we.FindElement(By.CssSelector(".name")).Text == propertyName+":" &&
                    we.FindElement(By.CssSelector(".reference")).Text == refTitle).Single()                                 
            );
            return prop.FindElement(By.CssSelector(".reference"));
        }

        protected string CssSelectorFor(Pane pane)
        {
            switch (pane)
            {
                case Pane.Single:
                    return ".single ";
                case Pane.Left:
                    return "#pane1 ";
                case Pane.Right:
                    return "#pane2 ";
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void WaitForView(Pane pane, PaneType type, string title = null)
        {
            var selector =  CssSelectorFor(pane)+" ." + type.ToString().ToLower() + " .header .title";
            if (title != null)
            {
                wait.Until(dr => dr.FindElement(By.CssSelector(selector)).Text == title);
            } else
            {
                WaitForCss(selector);
            }
            if (pane == Pane.Single)
            {
                WaitUntilElementDoesNotExist(".split");
            } else
            {
                WaitUntilElementDoesNotExist(".single");
            }
            AssertFooterExists();
        }

        protected virtual void AssertFooterExists()
        {
            wait.Until(d => d.FindElement(By.CssSelector(".footer")));
            Assert.IsTrue(br.FindElement(By.CssSelector(".footer .icon-home")).Displayed);
            Assert.IsTrue(br.FindElement(By.CssSelector(".footer .icon-back")).Displayed);
            Assert.IsTrue(br.FindElement(By.CssSelector(".footer .icon-forward")).Displayed);
        }

        protected void AssertTopItemInListIs(string title)
        {
            string topItem = WaitForCss(".collection tr td.reference").Text;

            Assert.AreEqual(title, topItem);
        }
        #endregion
        protected void AssertElementExists(string cssSelector)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count >= 1);
        }

        protected void WaitUntilElementDoesNotExist(string cssSelector)
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
            Click(EditButton());
            SaveButton();
            GetCancelEditButton();
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsTrue(title.StartsWith("Editing"));
        }

        protected void SaveObject()
        {
            Click(SaveButton());
            EditButton(); //To wait for save completed
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsFalse(title.StartsWith("Editing"));
        }

        protected IWebElement GetButton(string text, Pane pane = Pane.Single) {
            string selector = CssSelectorFor(pane) + ".header .action";
            wait.Until(d => br.FindElements(By.CssSelector(selector)).Any(e => e.Text == text));
            return br.FindElements(By.CssSelector(selector)).Single(e => e.Text == text);
        }

        protected IWebElement EditButton() {
            return GetButton("Edit");
        }

        protected IWebElement SaveButton() {
            return GetButton("Save");
        }

        protected IWebElement SaveAndCloseButton()
        {
            return GetButton("Save & Close");
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
        protected ReadOnlyCollection<IWebElement> GetObjectActions(int totalNumber, Pane pane = Pane.Single)
        {
            var selector = CssSelectorFor(pane)+ ".actions .action";
            wait.Until(d => d.FindElements(By.CssSelector(selector)).Count == totalNumber);
            return br.FindElements(By.CssSelector(selector));
        }

        protected IWebElement GetObjectAction(string actionName, Pane pane = Pane.Single)
        {
            var selector = CssSelectorFor(pane) + ".actions .action";
            var action =wait.Until(d => d.FindElements(By.CssSelector(selector)).
                    Single(we => we.Text == actionName));
            return action;
        }

        protected IWebElement OpenActionDialog(string actionName, Pane pane = Pane.Single)
        {
            Click(GetObjectAction(actionName, pane));
            var selector = CssSelectorFor(pane)+" .dialog ";
            var dialog = wait.Until(d => d.FindElement(By.CssSelector(selector)));

            Assert.AreEqual(actionName, WaitForCss(selector + "> .title").Text);
            //Check it has OK & cancel buttons
            wait.Until(d => br.FindElement(By.CssSelector(selector +".ok")));
            wait.Until(d => br.FindElement(By.CssSelector(".cancel")));
            return dialog;
        }

        protected IWebElement OKButton()
        {
            return wait.Until(d => br.FindElement(By.CssSelector(".dialog .ok")));
        }

        protected void CancelDialog(Pane pane = Pane.Single)
        {
           var selector = CssSelectorFor(pane)+".dialog ";
             Click(WaitForCss(selector + ".cancel"));

                wait.Until(dr => {
                    try
                    {
                        dr.FindElement(By.CssSelector(selector));
                        return false;
                    }
                    catch (NoSuchElementException)
                    {
                        return true;
                    }
                });
            
        }

        protected IWebElement AssertHasFocus(IWebElement el)
        {
            Assert.AreEqual(el, br.SwitchTo().ActiveElement());
            return el;
        }

        protected void Reload(Pane pane = Pane.Single)
        {
            Click(GetButton("Reload", pane));
        }
        
        #endregion

        #region ToolBar icons
        protected IWebElement HomeIcon()
        {
            return WaitForCss(".footer .icon-home");
        }

        protected IWebElement SwapIcon()
        {
            return WaitForCss(".footer .icon-swap");
        }

        protected IWebElement FullIcon()
        {
            return WaitForCss(".footer .icon-full");
        }
        #endregion

        #region Keyboard navigation 
        protected void CopyToClipboard(IWebElement element)
        {
            var title = element.Text;
            element.SendKeys(Keys.Control + "c");
            wait.Until(dr => dr.FindElement(By.CssSelector(".footer .currentcopy .reference")).Text == title);
        }

        protected IWebElement PasteIntoInputField(string cssSelector)
        {
            var target = WaitForCss(cssSelector);
            var copying = WaitForCss(".footer .currentcopy .reference").Text;
            target.Click();
            target.SendKeys(Keys.Control + "v");
            wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).GetAttribute("value") == copying);
            return WaitForCss(cssSelector);
        }

        protected IWebElement Tab(int numberIfTabs = 1)
        {
            for (int i = 1; i <= numberIfTabs; i++)
            {
                br.SwitchTo().ActiveElement().SendKeys(Keys.Tab);
            }
            return br.SwitchTo().ActiveElement();
        }
        #endregion
    }
}