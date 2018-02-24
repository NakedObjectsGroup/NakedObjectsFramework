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
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Selenium {
    public abstract class GeminiTest {
        #region chrome helper

        protected static string FilePath(string resourcename) {
            string fileName = resourcename.Remove(0, resourcename.IndexOf(".") + 1);

            string newFile = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            if (File.Exists(newFile)) {
                File.Delete(newFile);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("NakedObjects.Selenium." + resourcename)) {
                using (FileStream fileStream = File.Create(newFile, (int) stream.Length)) {
                    var bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }
            }

            return newFile;
        }

        #endregion

        protected void AssertElementExists(string cssSelector) {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count >= 1);
        }

        protected void WaitUntilElementDoesNotExist(string cssSelector) {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == 0);
        }

        protected void AssertElementCountIs(string cssSelector, int count) {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == count);
        }

        #region overhead

        protected const string BaseUrl = TestConfig.BaseUrl;
        protected const string GeminiBaseUrl = TestConfig.BaseUrl + "gemini/";

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
            br = new ChromeDriver();
            wait = new SafeWebDriverWait(br, TimeSpan.FromSeconds(TimeOut));
            br.Manage().Window.Maximize();
        }

        #endregion

        #region Helpers

        protected void Url(string url, bool trw = false) {
            br.Navigate().GoToUrl(url);
        }

        protected void GeminiUrl(string url) {
            Url(GeminiBaseUrl + url);
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
            //string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            //((IJavaScriptExecutor) br).ExecuteScript(script);

            Actions actions = new Actions(br);
            actions.MoveToElement(element);
            //actions.click();
            actions.Perform();
        }

        protected virtual void Click(IWebElement element) {
            WaitUntilEnabled(element);
            ScrollTo(element);
            element.Click();
        }

        protected void WaitUntilEnabled(IWebElement element) {
            wait.Until(dr => element.GetAttribute("disabled") == null);
        }

        protected virtual void RightClick(IWebElement element) {
            var webDriver = wait.Driver;
            ScrollTo(element);
            var loc = (ILocatable) element;
            var mouse = ((IHasInputDevices) webDriver).Mouse;
            mouse.ContextClick(loc.Coordinates);
        }

        protected virtual IWebElement WaitForCss(string cssSelector) {
            return wait.Until(d => d.FindElement(By.CssSelector(cssSelector)));
        }

        protected IWebElement WaitForTextEquals(string cssSelector, string text) {
            wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).Text == text);
            return WaitForCss(cssSelector);
        }

        protected IWebElement WaitForTextEquals(string cssSelector, int index, string text) {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector))[index].Text == text);
            return WaitForCss(cssSelector);
        }

        protected IWebElement WaitForTextStarting(string cssSelector, string startOftext) {
            wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).Text.StartsWith(startOftext));
            return WaitForCss(cssSelector);
        }

        /// <summary>
        /// Waits until there are AT LEAST the specified count of matches & returns ALL matches
        /// </summary>
        protected virtual ReadOnlyCollection<IWebElement> WaitForCss(string cssSelector, int count) {
            wait.Until(d => d.FindElements(By.CssSelector(cssSelector)).Count >= count);
            return br.FindElements(By.CssSelector(cssSelector));
        }

        /// <summary>
        /// Waits for the Nth match and returns it (counting from zero).
        /// </summary>
        protected virtual IWebElement WaitForCssNo(string cssSelector, int number) {
            return WaitForCss(cssSelector, number + 1)[number];
        }

        protected void WaitForMessage(string message, Pane pane = Pane.Single) {
            string p = CssSelectorFor(pane);
            wait.Until(dr => dr.FindElement(By.CssSelector(p + ".header .messages")).Text == message);
        }

        protected virtual void ClearFieldThenType(string cssFieldId, string characters) {
            var input = WaitForCss(cssFieldId);
            if (input.GetAttribute("value") != "") {
                input.SendKeys(Keys.Control + "a");
                Thread.Sleep(100);
                input.SendKeys(Keys.Delete);
                Thread.Sleep(100);
                wait.Until(dr => dr.FindElement(By.CssSelector(cssFieldId)).GetAttribute("value") == "");
            }
            input.SendKeys(characters);
        }

        protected virtual void ClearDateFieldThenType(string cssFieldId, string characters) {
            var input = WaitForCss(cssFieldId);
            if (input.GetAttribute("value") != "") {
                for (int i = 0; i < 3; i++) {
                    input.SendKeys(Keys.Delete);
                    Thread.Sleep(100);
                    input.SendKeys(Keys.Tab);
                    Thread.Sleep(100);
                }
                input.SendKeys(Keys.Shift + Keys.Tab);
                input.SendKeys(Keys.Shift + Keys.Tab);
                wait.Until(dr => dr.FindElement(By.CssSelector(cssFieldId)).GetAttribute("value") == "");
            }
            input.SendKeys(characters);
        }

        protected virtual void TypeIntoFieldWithoutClearing(string cssFieldId, string characters) {
            var input = WaitForCss(cssFieldId);
            input.SendKeys(characters);
        }

        protected void SelectCheckBox(string css, bool alreadySelected = false) {
            wait.Until(dr => dr.FindElement(By.CssSelector(css)).Selected == alreadySelected);
            var checkbox = br.FindElement(By.CssSelector(css));
            checkbox.Click();
            wait.Until(dr => dr.FindElement(By.CssSelector(css)).Selected == !alreadySelected);
        }

        /// <summary>
        /// Returns a string of n backspace keys for typing into a field
        /// </summary>
        protected string Repeat(string keys, int n) {
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++) {
                sb.Append(keys);
            }
            return sb.ToString();
        }

        protected virtual void SelectDropDownOnField(string cssFieldId, string characters) {
            var selected = new SelectElement(WaitForCss(cssFieldId));
            selected.SelectByText(characters);
            wait.Until(dr => selected.SelectedOption.Text == characters);
        }

        protected virtual void SelectDropDownOnField(string cssFieldId, int index) {
            var selected = new SelectElement(WaitForCss(cssFieldId));
            selected.SelectByIndex(index);
        }

        protected virtual void WaitForMenus() {
            wait.Until(dr => dr.FindElements(By.CssSelector("nof-menu-bar nof-action")).Count == 10);
        }

        protected virtual void GoToMenuFromHomePage(string menuName) {
            WaitForView(Pane.Single, PaneType.Home, "Home");

            WaitForMenus();

            ReadOnlyCollection<IWebElement> menus = br.FindElements(By.CssSelector("nof-action input"));
            IWebElement menu = menus.FirstOrDefault(s => s.GetAttribute("value") == menuName);
            if (menu != null) {
                Click(menu);
                wait.Until(d => d.FindElements(By.CssSelector("nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
            }
            else {
                throw new NotFoundException(string.Format("menu not found {0}", menuName));
            }
        }

        protected virtual void OpenObjectActions(Pane pane = Pane.Single) {
            string paneSelector = CssSelectorFor(pane);
            var actions = wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + "input")).Single(el => el.GetAttribute("value") == "Actions"));
            Click(actions);
            wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + " nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
        }

        protected virtual void OpenSubMenu(string menuName, Pane pane = Pane.Single) {
            string paneSelector = CssSelectorFor(pane);
            var sub = wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + " .submenu")).Single(el => el.Text == menuName));
            var expand = sub.FindElement(By.CssSelector(".icon-expand"));
            Click(expand);
            wait.Until(dr => dr.FindElements(By.CssSelector(".icon-collapse")).Count > 0);
        }

        protected virtual void OpenMenu(string menuName, Pane pane = Pane.Single) {
            string paneSelector = CssSelectorFor(pane);
            var menu = wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + "input")).Single(el => el.GetAttribute("value") == menuName));
            if (menu != null) {
                Click(menu);
                wait.Until(d => d.FindElements(By.CssSelector("nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
            }
            else {
                throw new NotFoundException(string.Format("menu not found {0}", menuName));
            }
        }

        protected virtual void CloseSubMenu(string menuName) {
            var sub = wait.Until(dr => dr.FindElements(By.CssSelector(".submenu")).Single(el => el.Text == menuName));
            var expand = sub.FindElement(By.CssSelector(".icon-collapse"));
            Click(expand);
            Assert.IsNotNull(sub.FindElement(By.CssSelector(".icon-expand")));
        }

        protected void Login() {
            Thread.Sleep(2000);
        }

        #endregion

        #region Resulting page view

        protected enum Pane {
            Single,
            Left,
            Right
        }

        protected enum PaneType {
            Home,
            Object,
            List,
            Recent,
            MultiLineDialog,
            Attachment,
            Properties,
            Error,
            Logoff
        }

        protected enum ClickType {
            Left,
            Right
        }

        protected IWebElement GetReferenceFromProperty(string propertyName, Pane pane = Pane.Single) {
            string propCss = CssSelectorFor(pane) + " " + ".property";
            var prop = wait.Until(dr => dr.FindElements(By.CssSelector(propCss))
                .Where(we => we.FindElement(By.CssSelector(".name")).Text == propertyName + ":").Single()
            );
            return prop.FindElement(By.CssSelector(".reference"));
        }

        protected IWebElement GetReferenceProperty(string propertyName, string refTitle, Pane pane = Pane.Single) {
            string propCss = CssSelectorFor(pane) + " " + ".property";
            var prop = wait.Until(dr => dr.FindElements(By.CssSelector(propCss))
                .Where(we => we.FindElement(By.CssSelector(".name")).Text == propertyName + ":" &&
                             we.FindElement(By.CssSelector(".reference")).Text == refTitle).Single()
            );
            return prop.FindElement(By.CssSelector(".reference"));
        }

        protected string CssSelectorFor(Pane pane) {
            switch (pane) {
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

        protected virtual void WaitForView(Pane pane, PaneType type, string title = null) {
            var selector = CssSelectorFor(pane) + " ." + type.ToString().ToLower();

            if (title != null) {
                selector += " .header .title";
                wait.Until(dr => dr.FindElement(By.CssSelector(selector)).Text == title);
            }
            else {
                WaitForCss(selector);
            }
            if (pane == Pane.Single) {
                WaitUntilElementDoesNotExist(".split");
            }
            else {
                WaitUntilElementDoesNotExist(".single");
            }
            AssertFooterExists();
        }

        protected virtual void AssertFooterExists() {
            wait.Until(d => d.FindElement(By.CssSelector(".footer")));
            wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.home")).Displayed);
            wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.back")).Displayed);
            wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.forward")).Displayed);
        }

        protected void AssertTopItemInListIs(string title) {
            string topItem = WaitForCss(".list tr td.reference").Text;

            Assert.AreEqual(title, topItem);
        }

        #endregion

        #region Editing & Saving

        protected void EditObject() {
            Click(EditButton());
            SaveButton();
            GetCancelEditButton();
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsTrue(title.StartsWith("Editing"));
        }

        protected void SaveObject(Pane pane = Pane.Single) {
            Click(SaveButton(pane));
            EditButton(pane); //To wait for save completed
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsFalse(title.StartsWith("Editing"));
        }

        protected void CancelObject(Pane pane = Pane.Single) {
            Click(GetCancelEditButton(pane));
            EditButton(pane); //To wait for cancel completed
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsFalse(title.StartsWith("Editing"));
        }

        protected IWebElement GetButton(string text, Pane pane = Pane.Single) {
            string selector = CssSelectorFor(pane) + ".header .action";
            return wait.Until(dr => dr.FindElements(By.CssSelector(selector)).Single(e => e.Text == text));
        }

        protected IWebElement GetInputButton(string text, Pane pane = Pane.Single) {
            string selector = CssSelectorFor(pane) + "input";
            return wait.Until(dr => dr.FindElements(By.CssSelector(selector)).Single(e => e.GetAttribute("value") == text));
        }

        protected IWebElement EditButton(Pane pane = Pane.Single) {
            return GetInputButton("Edit", pane);
        }

        protected IWebElement SaveButton(Pane pane = Pane.Single) {
            return GetInputButton("Save", pane);
        }

        protected IWebElement SaveVMButton(Pane pane = Pane.Single) {
            return GetInputButton("Save", pane);
        }

        protected IWebElement SaveAndCloseButton(Pane pane = Pane.Single) {
            return GetInputButton("Save & Close", pane);
        }

        protected IWebElement GetCancelEditButton(Pane pane = Pane.Single) {
            //string p = CssSelectorFor(pane);
            //return wait.Until(d => d.FindElements(By.CssSelector(p + ".header .action")).Single(el => el.Text == "Cancel"));
            return GetInputButton("Cancel", pane);
        }

        protected void ClickHomeButton() {
            Click(WaitForCss(".icon.home"));
        }

        protected void ClickBackButton() {
            Click(WaitForCss(".icon.back"));
        }

        protected void ClickForwardButton() {
            Click(WaitForCss(".icon.forward"));
        }

        protected void ClickRecentButton() {
            Click(WaitForCss(".icon.recent"));
        }

        protected void ClickPropertiesButton() {
            Click(WaitForCss(".icon.properties"));
        }

        protected void ClickLogOffButton() {
            Click(WaitForCss(".icon.logoff"));
        }

        #endregion

        #region Object Actions

        protected ReadOnlyCollection<IWebElement> GetObjectActions(int totalNumber, Pane pane = Pane.Single) {
            var selector = CssSelectorFor(pane) + "nof-action-list nof-action > input";
            wait.Until(d => d.FindElements(By.CssSelector(selector)).Count == totalNumber);
            return br.FindElements(By.CssSelector(selector));
        }

        protected void AssertAction(int number, string actionName) {
            wait.Until(dr => dr.FindElements(By.CssSelector("nof-action-list nof-action > input"))[number].GetAttribute("value") == actionName);
        }

        protected virtual void AssertActionNotDisplayed(string action) {
            wait.Until(dr => dr.FindElements(By.CssSelector($"nof-action-list nof-action inputinput[type='{action}']")).FirstOrDefault() == null);
        }

        protected IWebElement GetObjectAction(string actionName, Pane pane = Pane.Single, string subMenuName = null) {
            if (subMenuName != null) {
                OpenSubMenu(subMenuName);
            }
            var selector = CssSelectorFor(pane) + $"nof-action-list nof-action input[value='{actionName}']";
            return wait.Until(d => d.FindElement(By.CssSelector(selector)));
        }

        protected IWebElement GetLCA(string actionName, Pane pane = Pane.Single) {
            var selector = CssSelectorFor(pane) + $"nof-collection nof-action input[value='{actionName}']";
            return wait.Until(d => d.FindElement(By.CssSelector(selector)));
        }

        protected IWebElement GetObjectEnabledAction(string actionName, Pane pane = Pane.Single, string subMenuName = null) {
            var a = GetObjectAction(actionName, pane, subMenuName);

            if (a.Enabled) {
                return a;
            }

            Thread.Sleep(1000);

            a = GetObjectAction(actionName, pane, subMenuName);

            if (a.Enabled) {
                return a;
            }

            throw new Exception("Action not enabled");
        }

        protected IWebElement OpenActionDialog(string actionName, Pane pane = Pane.Single, int? noOfParams = null) {
            Click(GetObjectEnabledAction(actionName, pane));

            var dialogSelector = CssSelectorFor(pane) + " .dialog ";
            wait.Until(d => d.FindElement(By.CssSelector(dialogSelector + "> .title")).Text == actionName);
            //Check it has OK & cancel buttons
            wait.Until(d => br.FindElement(By.CssSelector(dialogSelector + ".ok")));
            wait.Until(d => br.FindElement(By.CssSelector(dialogSelector + ".cancel")));
            //Wait for params if required
            if (noOfParams != null) {
                wait.Until(dr => dr.FindElements(By.CssSelector(dialogSelector + " .parameter")).Count == noOfParams.Value);
            }
            return WaitForCss(dialogSelector);
        }

        protected IWebElement GetInputNumber(IWebElement dialog, int no) {
            wait.Until(dr => dialog.FindElements(By.CssSelector(".parameter .value input")).Count >= no + 1);
            return dialog.FindElements(By.CssSelector(".parameter .value input"))[no];
        }

        protected IWebElement OKButton() {
            return WaitForCss(".dialog .ok");
        }

        //For use with multi-line dialogs, lineNo starts from zero
        protected IWebElement OKButtonOnLine(int lineNo) {
            return wait.Until(dr => dr.FindElements(By.CssSelector(".lineDialog"))[lineNo].FindElement(By.CssSelector(".ok")));
        }

        protected void WaitForOKButtonToDisappear(int lineNo) {
            var line = WaitForCssNo(".lineDialog", lineNo);
            wait.Until(dr => line.FindElements(By.CssSelector(".ok")).Count == 0);
        }

        protected void WaitForReadOnlyEnteredParam(int lineNo, int paramNo, string value) {
            var line = WaitForCssNo(".lineDialog", lineNo);

            wait.Until(dr => line.FindElements(By.CssSelector(".parameter .value"))[paramNo].Text == value);
        }

        protected void CancelDialog(Pane pane = Pane.Single) {
            var selector = CssSelectorFor(pane) + ".dialog ";
            Click(WaitForCss(selector + ".cancel"));

            wait.Until(dr => {
                try {
                    dr.FindElement(By.CssSelector(selector));
                    return false;
                }
                catch (NoSuchElementException) {
                    return true;
                }
            });
        }

        protected void AssertHasFocus(IWebElement el) {
            wait.Until(dr => dr.SwitchTo().ActiveElement() == el);
        }

        protected void Reload(Pane pane = Pane.Single) {
            Click(GetInputButton("Reload", pane));
        }

        protected void CancelDatePicker(string cssForInput) {
            var dp = br.FindElement(By.CssSelector(".ui-datepicker"));
            if (dp.Displayed) {
                WaitForCss(cssForInput).SendKeys(Keys.Escape);
                wait.Until(br => !br.FindElement(By.CssSelector(".ui-datepicker")).Displayed);
            }
        }

        protected void PageDownAndWait() {
            br.SwitchTo().ActiveElement().SendKeys(Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown);
            Thread.Sleep(1000);
        }

        #endregion

        #region CCAs

        protected void CheckIndividualItem(int itemNo, string label, string value, bool equal = true) {
            GeminiUrl("object?o1=___1.SpecialOffer--" + (itemNo + 1));
            var html = label + "\r\n" + value;
            if (equal) {
                //Thread.Sleep(2000);

                //var t = br.FindElements(By.CssSelector(".property")).First().Text;

                wait.Until(dr => dr.FindElements(By.CssSelector(".property")).First(p => p.Text.StartsWith(label)).Text == html);
            }
            else {
                wait.Until(dr => dr.FindElements(By.CssSelector(".property")).First(p => p.Text.StartsWith(label)).Text != html);
            }
        }

        protected void WaitForSelectedCheckboxes(int number) {
            wait.Until(dr => dr.FindElements(By.CssSelector("input[type='checkbox']")).Count(el => el.Selected && el.Enabled) == number);
        }

        #endregion

        #region ToolBar icons

        protected IWebElement HomeIcon() {
            return WaitForCss(".footer .icon.home");
        }

        protected IWebElement SwapIcon() {
            return WaitForCss(".footer .icon.swap");
        }

        protected IWebElement FullIcon() {
            return WaitForCss(".footer .icon.full");
        }

        protected void GoBack(int clicks = 1) {
            for (int i = 1; i <= clicks; i++) {
                Click(br.FindElement(By.CssSelector(".icon.back")));
            }
        }

        #endregion

        #region Keyboard navigation 

        protected void CopyToClipboard(IWebElement element) {
            var title = element.Text;
            element.SendKeys(Keys.Control + "c");
            wait.Until(dr => dr.FindElement(By.CssSelector(".footer .currentcopy .reference")).Text == title);
        }

        protected IWebElement PasteIntoInputField(string cssSelector) {
            var target = WaitForCss(cssSelector);
            var copying = WaitForCss(".footer .currentcopy .reference").Text;
            target.Click();
            target.SendKeys(Keys.Control + "v");
            wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).GetAttribute("value") == copying);
            return WaitForCss(cssSelector);
        }

        protected IWebElement PasteIntoReferenceField(string cssSelector) {
            var target = WaitForCss(cssSelector);
            var copying = WaitForCss(".footer .currentcopy .reference").Text;
            target.Click();
            target.SendKeys(Keys.Control + "v");
            wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).GetAttribute("value") == copying);
            return WaitForCss(cssSelector);
        }

        protected IWebElement Tab(int numberIfTabs = 1) {
            for (int i = 1; i <= numberIfTabs; i++) {
                br.SwitchTo().ActiveElement().SendKeys(Keys.Tab);
            }
            return br.SwitchTo().ActiveElement();
        }

        #endregion

        #region Cicero helper methods

        protected void CiceroUrl(string url) {
            br.Navigate().GoToUrl(TestConfig.BaseUrl + "cicero/" + url);
        }

        protected void WaitForOutput(string output) {
            wait.Until(dr => dr.FindElement(By.CssSelector(".output")).Text == output);
        }

        protected void WaitForOutputStarting(string output) {
            wait.Until(dr => dr.FindElement(By.CssSelector(".output")).Text.StartsWith(output));
        }

        protected void WaitForOutputContaining(string output) {
            wait.Until(dr => dr.FindElement(By.CssSelector(".output")).Text.Contains(output));
        }

        protected void EnterCommand(string command) {
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).Text == "");
            TypeIntoFieldWithoutClearing("input", command);
            Thread.Sleep(300); //To make it easier to see that the command has been entered
            TypeIntoFieldWithoutClearing("input", Keys.Enter);
        }

        #endregion
    }

    public static class ExtensionMethods {
        public static IWebElement AssertIsDisabled(this IWebElement a, string reason = null) {
            Assert.IsNotNull(a.GetAttribute("disabled"), "Element " + a.Text + " is not disabled");
            if (reason != null) {
                Assert.AreEqual(reason, a.GetAttribute("title"));
            }
            return a;
        }

        public static IWebElement AssertIsEnabled(this IWebElement a) {
            Assert.IsNull(a.GetAttribute("disabled"), "Element " + a.Text + " is disabled");
            return a;
        }

        public static IWebElement AssertHasTooltip(this IWebElement a, string tooltip) {
            Assert.AreEqual(tooltip, a.GetAttribute("title"));
            return a;
        }

        public static IWebElement AssertIsInvisible(this IWebElement a) {
            Assert.IsNull(a.GetAttribute("displayed"));
            return a;
        }
    }
}