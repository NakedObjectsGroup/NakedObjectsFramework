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
using NakedFramework.Selenium.Helpers.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace NakedFrameworkClient.TestFramework
{
    public class Helper
    {
        private readonly string GeminiBaseUrl;
        internal readonly IWebDriver br; 
        internal readonly SafeWebDriverWait wait;

        public Helper(string baseUrl)
        {
            GeminiBaseUrl = baseUrl + "gemini/";
            br = new ChromeDriver();
            wait = new SafeWebDriverWait(br, TimeSpan.FromSeconds(TimeOut));
            br.Manage().Window.Maximize();
        }

        #region chrome helper

        public static string FilePath(string resourcename)
        {
            string fileName = resourcename.Remove(0, resourcename.IndexOf(".") + 1);

            string newFile = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            if (File.Exists(newFile))
            {
                File.Delete(newFile);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("NakedFrameworkClient.TestFramework." + resourcename))
            {
                using (FileStream fileStream = File.Create(newFile, (int)stream.Length))
                {
                    var bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }
            }

            return newFile;
        }

        #endregion



        #region overhead





        private static int timeOut = 0;

        private static int TimeOut
        {
            get
            {
                if (timeOut != 0) { return timeOut; }
                timeOut = 20;
                return 40;
            }
        }

        public void CleanUp()
        {
            if (br != null)
            {
                try
                {
                    br.Manage().Cookies.DeleteAllCookies();
                    br.Quit();
                    br.Dispose();
                    //br = null;
                }
                catch
                {
                    // to suppress error 
                }
            }
        }


        #endregion

        #region Helpers

        private void WaitUntilGone<TResult>(Func<IWebDriver, TResult> condition)
        {
            wait.Until(d =>
            {
                try
                {
                    condition(d);
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return true;
                }
            });
        }

        private void Maximize()
        {
            const string script = "window.moveTo(0, 0); window.resizeTo(screen.availWidth, screen.availHeight);";
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }

        private void ScrollTo(IWebElement element)
        {
            //string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            //((IJavaScriptExecutor) br).ExecuteScript(script);

            Actions actions = new Actions(br);
            actions.MoveToElement(element);
            //actions.click();
            actions.Perform();
        }

        internal void Click(IWebElement element)
        {
            WaitUntilEnabled(element);
            ScrollTo(element);
            element.Click();
        }

        private void WaitUntilEnabled(IWebElement element)
        {
            wait.Until(dr => element.GetAttribute("disabled") == null);
        }

        internal void RightClick(IWebElement element)
        {
            var webDriver = wait.Driver;
            ScrollTo(element);
            //var loc = (ILocatable) element;
            //var mouse = ((IHasInputDevices) webDriver).Mouse;
            //mouse.ContextClick(loc.Coordinates);

            Actions actions = new Actions(webDriver);
            actions.ContextClick(element);
            actions.Perform();
        }

        internal IWebElement WaitForCss(string cssSelector)
        {
            return wait.Until(d => d.FindElement(By.CssSelector(cssSelector)));
        }

        internal IWebElement WaitForChildElement(IWebElement parent, string cssSelector)
        {
            return wait.Until(d => parent.FindElement(By.CssSelector(cssSelector)));
        }

        private IWebElement WaitForTextEquals(string cssSelector, string text)
        {
            wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).Text.Trim() == text.Trim());
            return WaitForCss(cssSelector);
        }

        private IWebElement WaitForTextEquals(string cssSelector, int index, string text)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector))[index].Text == text);
            return WaitForCss(cssSelector);
        }

        private IWebElement WaitForTextStarting(string cssSelector, string startOftext)
        {
            wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).Text.StartsWith(startOftext));
            return WaitForCss(cssSelector);
        }

        /// <summary>
        /// Waits until there are AT LEAST the specified count of matches & returns ALL matches
        /// </summary>
        private ReadOnlyCollection<IWebElement> WaitForCss(string cssSelector, int count)
        {
            wait.Until(d => d.FindElements(By.CssSelector(cssSelector)).Count >= count);
            return br.FindElements(By.CssSelector(cssSelector));
        }

        /// <summary>
        /// Waits for the Nth match and returns it (counting from zero).
        /// </summary>
        internal IWebElement WaitForCssNo(string cssSelector, int number)
        {
            return WaitForCss(cssSelector, number + 1)[number];
        }

        private void WaitForMessage(string message, Pane pane = Pane.Single)
        {
            string p = CssSelectorFor(pane);
            wait.Until(dr => dr.FindElement(By.CssSelector(p + ".header .messages")).Text == message);
        }

        internal  void ClearFieldThenType(string cssFieldId, string characters)
        {
            var input = WaitForCss(cssFieldId);
            if (input.GetAttribute("value") != "")
            {
                input.SendKeys(Keys.Control + "a");
                Thread.Sleep(100);
                input.SendKeys(Keys.Delete);
                Thread.Sleep(100);
                wait.Until(dr => dr.FindElement(By.CssSelector(cssFieldId)).GetAttribute("value") == "");
            }
            input.SendKeys(characters);
        }

        private void ClearDateFieldThenType(string cssFieldId, string characters)
        {
            var input = WaitForCss(cssFieldId);
            if (input.GetAttribute("value") != "")
            {
                for (int i = 0; i < 3; i++)
                {
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

        internal void TypeIntoFieldWithoutClearing(string cssFieldId, string characters)
        {
            var input = WaitForCss(cssFieldId);
            input.SendKeys(characters);
        }

        internal void SelectCheckBox(string css, bool alreadySelected = false)
        {
            wait.Until(dr => dr.FindElement(By.CssSelector(css)).Selected == alreadySelected);
            var checkbox = br.FindElement(By.CssSelector(css));
            checkbox.Click();
            wait.Until(dr => dr.FindElement(By.CssSelector(css)).Selected == !alreadySelected);
        }

        /// <summary>
        /// Returns a string of n backspace keys for typing into a field
        /// </summary>
        private string Repeat(string keys, int n)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                sb.Append(keys);
            }
            return sb.ToString();
        }

        internal void SelectDropDownOnField(string cssFieldId, string characters)
        {
            var selected = new SelectElement(WaitForCss(cssFieldId));
            selected.SelectByText(characters);
            wait.Until(dr => selected.SelectedOption.Text == characters);
        }

        internal void SelectDropDownOnField(string cssFieldId, int index)
        {
            var selected = new SelectElement(WaitForCss(cssFieldId));
            selected.SelectByIndex(index);
        }

        private void WaitForMenus()
        {
            wait.Until(dr => dr.FindElements(By.CssSelector("nof-menu-bar nof-action")).Count == 10);
        }

        private void GoToMenuFromHomePage(string menuName)
        {
            WaitForView(Pane.Single, PaneType.Home, "Home");

            WaitForMenus();

            ReadOnlyCollection<IWebElement> menus = br.FindElements(By.CssSelector("nof-action input"));
            IWebElement menu = menus.FirstOrDefault(s => s.GetAttribute("value") == menuName);
            if (menu != null)
            {
                Click(menu);
                wait.Until(d => d.FindElements(By.CssSelector("nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
            }
            else
            {
                throw new NotFoundException(string.Format("menu not found {0}", menuName));
            }
        }

        internal void OpenObjectActions(Pane pane = Pane.Single)
        {
            string paneSelector = CssSelectorFor(pane);
            var actions = wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + "input")).Single(el => el.GetAttribute("value") == "Actions"));
            Click(actions);
            wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + " nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
        }

        internal void OpenSubMenu(string menuName, Pane pane = Pane.Single)
        {
            string paneSelector = CssSelectorFor(pane);
            var sub = wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + " .submenu")).Single(el => el.Text == menuName));
            var expand = sub.FindElement(By.CssSelector(".icon-expand"));
            Click(expand);
            wait.Until(dr => dr.FindElements(By.CssSelector(".icon-collapse")).Count > 0);
        }

        private void OpenMenu(string menuName, Pane pane = Pane.Single)
        {
            string paneSelector = CssSelectorFor(pane);
            var menu = wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + "input")).Single(el => el.GetAttribute("value") == menuName));
            if (menu != null)
            {
                Click(menu);
                wait.Until(d => d.FindElements(By.CssSelector("nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
            }
            else
            {
                throw new NotFoundException(string.Format("menu not found {0}", menuName));
            }
        }

        private void CloseSubMenu(string menuName)
        {
            var sub = wait.Until(dr => dr.FindElements(By.CssSelector(".submenu")).Single(el => el.Text == menuName));
            var expand = sub.FindElement(By.CssSelector(".icon-collapse"));
            Click(expand);
            Assert.IsNotNull(sub.FindElement(By.CssSelector(".icon-expand")));
        }

        private void Login()
        {
            Thread.Sleep(2000);
        }

        #endregion

        #region Resulting page view

     

        internal enum ClickType
        {
            Left,
            Right
        }

        internal string GetPropertyValue(string propertyName, Pane pane = Pane.Single)
        {
            var prop = GetProperty(propertyName, pane);
            return prop.FindElement(By.CssSelector(".value")).Text.Trim();
        }

        internal IWebElement GetProperty(string propertyName, Pane pane = Pane.Single)
        {
            string propCss = CssSelectorFor(pane) + " " + "nof-view-property";
            return wait.Until(dr => dr.FindElements(By.CssSelector(propCss))
                .Where(we => we.FindElement(By.CssSelector(".name")).Text == propertyName + ":").Single());
        }

        internal IWebElement GetReferenceFromProperty(string propertyName, Pane pane = Pane.Single)
        {
            var prop = GetProperty(propertyName, pane);
            return prop.FindElement(By.CssSelector(".reference"));
        }

        private IWebElement GetReferenceProperty(string propertyName, string refTitle, Pane pane = Pane.Single)
        {
            string propCss = CssSelectorFor(pane) + " " + ".property";
            var prop = wait.Until(dr => dr.FindElements(By.CssSelector(propCss))
                .Where(we => we.FindElement(By.CssSelector(".name")).Text == propertyName + ":" &&
                             we.FindElement(By.CssSelector(".reference")).Text == refTitle).Single()
            );
            return prop.FindElement(By.CssSelector(".reference"));
        }

        internal string CssSelectorFor(Pane pane)
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

        internal string CssSelectorFor(Pane pane, PaneType type) =>
            CssSelectorFor(pane) + " ." + type.ToString().ToLower();


        internal void WaitForView(Pane pane, PaneType type, string title = null)
        {
            var selector = CssSelectorFor(pane) + " ." + type.ToString().ToLower();

            if (title != null)
            {
                selector += " .header .title";
                wait.Until(dr => dr.FindElement(By.CssSelector(selector)).Text == title);
            }
            else
            {
                WaitForCss(selector);
            }
            if (pane == Pane.Single)
            {
                WaitUntilElementDoesNotExist(".split");
            }
            else
            {
                WaitUntilElementDoesNotExist(".single");
            }
            AssertFooterExists();
        }

        private void AssertFooterExists()
        {
            wait.Until(d => d.FindElement(By.CssSelector(".footer")));
            wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.home")).Displayed);
            wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.back")).Displayed);
            wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.forward")).Displayed);
        }

        internal void AssertTopItemInListIs(string title)
        {
            string topItem = WaitForCss("tr td.reference").Text;

            Assert.AreEqual(title, topItem);
        }

        #endregion

        #region Editing & Saving

        private void EditObject()
        {
            Click(EditButton());
            SaveButton();
            GetCancelEditButton();
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsTrue(title.StartsWith("Editing"));
        }

        private void SaveObject(Pane pane = Pane.Single)
        {
            Click(SaveButton(pane));
            EditButton(pane); //To wait for save completed
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsFalse(title.StartsWith("Editing"));
        }

        private void CancelObject(Pane pane = Pane.Single)
        {
            Click(GetCancelEditButton(pane));
            EditButton(pane); //To wait for cancel completed
            var title = br.FindElement(By.CssSelector(".header .title")).Text;
            Assert.IsFalse(title.StartsWith("Editing"));
        }

        private IWebElement GetButton(string text, Pane pane = Pane.Single)
        {
            string selector = CssSelectorFor(pane) + ".header .action";
            return wait.Until(dr => dr.FindElements(By.CssSelector(selector)).Single(e => e.Text == text));
        }

        private IWebElement GetInputButton(string text, Pane pane = Pane.Single)
        {
            string selector = CssSelectorFor(pane) + "input";
            return wait.Until(dr => dr.FindElements(By.CssSelector(selector)).Single(e => e.GetAttribute("value") == text));
        }

        private IWebElement EditButton(Pane pane = Pane.Single)
        {
            return GetInputButton("Edit", pane);
        }

        private IWebElement SaveButton(Pane pane = Pane.Single)
        {
            return GetInputButton("Save", pane);
        }

        private IWebElement SaveVMButton(Pane pane = Pane.Single)
        {
            return GetInputButton("Save", pane);
        }

        private IWebElement SaveAndCloseButton(Pane pane = Pane.Single)
        {
            return GetInputButton("Save & Close", pane);
        }

        private IWebElement GetCancelEditButton(Pane pane = Pane.Single)
        {
            //string p = CssSelectorFor(pane);
            //return wait.Until(d => d.FindElements(By.CssSelector(p + ".header .action")).Single(el => el.Text == "Cancel"));
            return GetInputButton("Cancel", pane);
        }

        private void ClickHomeButton()
        {
            Click(WaitForCss(".icon.home"));
        }

        private void ClickBackButton()
        {
            Click(WaitForCss(".icon.back"));
        }

        private void ClickForwardButton()
        {
            Click(WaitForCss(".icon.forward"));
        }

        private void ClickRecentButton()
        {
            Click(WaitForCss(".icon.recent"));
        }

        private void ClickPropertiesButton()
        {
            Click(WaitForCss(".icon.properties"));
        }

        private void ClickLogOffButton()
        {
            Click(WaitForCss(".icon.logoff"));
        }

        #endregion

        #region Object Actions

        private ReadOnlyCollection<IWebElement> GetObjectActions(int totalNumber, Pane pane = Pane.Single)
        {
            var selector = CssSelectorFor(pane) + "nof-action-list nof-action > input";
            wait.Until(d => d.FindElements(By.CssSelector(selector)).Count == totalNumber);
            return br.FindElements(By.CssSelector(selector));
        }

        private void AssertAction(int number, string actionName)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector("nof-action-list nof-action > input"))[number].GetAttribute("value") == actionName);
        }

        private void AssertActionNotDisplayed(string action)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector($"nof-action-list nof-action inputinput[type='{action}']")).FirstOrDefault() == null);
        }

        internal IWebElement GetObjectAction(string actionName, Pane pane = Pane.Single, string subMenuName = null)
        {
            if (subMenuName != null)
            {
                OpenSubMenu(subMenuName);
            }
            var selector = CssSelectorFor(pane) + $"nof-action-list nof-action input[value='{actionName}']";
            return wait.Until(d => d.FindElement(By.CssSelector(selector)));
        }

        private IWebElement GetLCA(string actionName, Pane pane = Pane.Single)
        {
            var selector = CssSelectorFor(pane) + $"nof-collection nof-action input[value='{actionName}']";
            return wait.Until(d => d.FindElement(By.CssSelector(selector)));
        }

        private IWebElement GetObjectEnabledAction(string actionName, Pane pane = Pane.Single, string subMenuName = null)
        {
            var a = GetObjectAction(actionName, pane, subMenuName);

            if (a.Enabled)
            {
                return a;
            }

            Thread.Sleep(1000);

            a = GetObjectAction(actionName, pane, subMenuName);

            if (a.Enabled)
            {
                return a;
            }

            throw new Exception("Action not enabled");
        }

        internal IWebElement OpenActionDialog(string actionName, Pane pane = Pane.Single, int? noOfParams = null)
        {
            Click(GetObjectEnabledAction(actionName, pane));

            var dialogSelector = CssSelectorFor(pane) + " .dialog ";
            wait.Until(d => d.FindElement(By.CssSelector(dialogSelector + "> .title")).Text == actionName);
            //Check it has OK & cancel buttons
            wait.Until(d => br.FindElement(By.CssSelector(dialogSelector + ".ok")));
            wait.Until(d => br.FindElement(By.CssSelector(dialogSelector + ".cancel")));
            //Wait for params if required
            if (noOfParams != null)
            {
                wait.Until(dr => dr.FindElements(By.CssSelector(dialogSelector + " .parameter")).Count == noOfParams.Value);
            }
            return WaitForCss(dialogSelector);
        }

        private IWebElement GetInputNumber(IWebElement dialog, int no)
        {
            wait.Until(dr => dialog.FindElements(By.CssSelector(".parameter .value input")).Count >= no + 1);
            return dialog.FindElements(By.CssSelector(".parameter .value input"))[no];
        }

        internal IWebElement OKButton()
        {
            return WaitForCss(".dialog .ok");
        }

        //For use with multi-line dialogs, lineNo starts from zero
        private IWebElement OKButtonOnLine(int lineNo)
        {
            return wait.Until(dr => dr.FindElements(By.CssSelector(".lineDialog"))[lineNo].FindElement(By.CssSelector(".ok")));
        }

        private void WaitForOKButtonToDisappear(int lineNo)
        {
            var line = WaitForCssNo(".lineDialog", lineNo);
            wait.Until(dr => line.FindElements(By.CssSelector(".ok")).Count == 0);
        }

        private void WaitForReadOnlyEnteredParam(int lineNo, int paramNo, string value)
        {
            var line = WaitForCssNo(".lineDialog", lineNo);

            wait.Until(dr => line.FindElements(By.CssSelector(".parameter .value"))[paramNo].Text == value);
        }

        private void CancelDialog(Pane pane = Pane.Single)
        {
            var selector = CssSelectorFor(pane) + ".dialog ";
            Click(WaitForCss(selector + ".cancel"));

            wait.Until(dr =>
            {
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

        private void AssertHasFocus(IWebElement el)
        {
            wait.Until(dr => dr.SwitchTo().ActiveElement() == el);
        }

        internal void Reload(Pane pane = Pane.Single)
        {
            Click(GetInputButton("Reload", pane));
        }

        private void CancelDatePicker(string cssForInput)
        {
            var dp = br.FindElement(By.CssSelector(".ui-datepicker"));
            if (dp.Displayed)
            {
                WaitForCss(cssForInput).SendKeys(Keys.Escape);
                wait.Until(br => !br.FindElement(By.CssSelector(".ui-datepicker")).Displayed);
            }
        }

        private void PageDownAndWait()
        {
            br.SwitchTo().ActiveElement().SendKeys(Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown);
            Thread.Sleep(1000);
        }

        #endregion

        #region CCAs

    
        private void WaitForSelectedCheckboxes(int number)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector("input[type='checkbox']")).Count(el => el.Selected && el.Enabled) == number);
        }

        #endregion

        #region ToolBar icons

        private IWebElement HomeIcon()
        {
            return WaitForCss(".footer .icon.home");
        }

        private IWebElement SwapIcon()
        {
            return WaitForCss(".footer .icon.swap");
        }

        internal IWebElement FullIcon()
        {
            return WaitForCss(".footer .icon.full");
        }

        private void GoBack(int clicks = 1)
        {
            for (int i = 1; i <= clicks; i++)
            {
                Click(br.FindElement(By.CssSelector(".icon.back")));
            }
        }

        #endregion

        #region Keyboard navigation 

        internal void CopyToClipboard(IWebElement element)
        {
            var title = element.Text;
            element.SendKeys(Keys.Control + "c");
            wait.Until(dr => dr.FindElement(By.CssSelector(".footer .currentcopy .reference")).Text == title);
        }

        internal IWebElement PasteIntoInputField(string cssSelector)
        {
            var target = WaitForCss(cssSelector);
            var copying = WaitForCss(".footer .currentcopy .reference").Text;
            target.Click();
            target.SendKeys(Keys.Control + "v");
            wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).GetAttribute("value") == copying);
            return WaitForCss(cssSelector);
        }

        internal IWebElement PasteIntoInputField(IWebElement target)
        {
            var copying = WaitForCss(".footer .currentcopy .reference").Text;
            target.Click();
            target.SendKeys(Keys.Control + "v");
            wait.Until(dr => target.GetAttribute("value") == copying);
            return target;
        }

        private IWebElement Tab(int numberIfTabs = 1)
        {
            for (int i = 1; i <= numberIfTabs; i++)
            {
                br.SwitchTo().ActiveElement().SendKeys(Keys.Tab);
            }
            return br.SwitchTo().ActiveElement();
        }

        #endregion


        private void OpenMainMenu(string menuName)
        {
            ClickHomeButton();
            WaitForView(Pane.Single, PaneType.Home, "Home");
            string menuSelector = $"nof-menu-bar nof-action input[title=\"{menuName}\"";
            wait.Until(dr => dr.FindElement(By.CssSelector(menuSelector)));
            IWebElement menu = br.FindElement(By.CssSelector($"nof-menu-bar nof-action input[title=\"{menuName}\"]"));
            Click(menu);
        }
        internal void OpenMainMenuAction(string menuName, string actionName)
        {
            OpenMainMenu(menuName);
            string actionSelector = $"nof-action-list nof-action input[value=\"{actionName}\"]";
            Click(WaitForCss(actionSelector));
        }

        internal IWebElement WaitForTitle(string title, Pane pane = Pane.Single) => 
            WaitForTextEquals(CssSelectorFor(pane)+" .title", title);

        private void AssertElementExists(string cssSelector)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count >= 1);
        }

        private void WaitUntilElementDoesNotExist(string cssSelector)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == 0);
        }

        private void AssertElementCountIs(string cssSelector, int count)
        {
            wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == count);
        }


        #region New Framework
        //Goes to a single-pane view of home
        public HomeView GotoHome()
        {
            br.Navigate().GoToUrl(GeminiBaseUrl + "home");
            WaitForView(Pane.Single, PaneType.Home);
            var el = WaitForCss(".home");            
            return new HomeView(el, this);
        }

        //Going via home ensures that any existing object/list view will have been removed before
        //going to new URL
        public Helper GotoUrlViaHome(string url)
        {
            GotoHome(); //This is to ensure that the view has changed from any existing object/list view
            br.Navigate().GoToUrl(GeminiBaseUrl + url);
            return this;
        }

        public Helper GotoUrlDirectly(string url)
        {
            br.Navigate().GoToUrl(GeminiBaseUrl + url);
            return this;
        }

        public ObjectView GetObjectView(Pane pane = Pane.Single)
        {

            WaitForCss(CssSelectorFor(pane) + " .object .title");
            WaitForCss(CssSelectorFor(pane) + " .object .properties");
            WaitForCss(CssSelectorFor(pane) + " .object .collections");
            var el = WaitForCss(CssSelectorFor(pane) + " .object");
            return new ObjectView(el, this, pane);
        }

        public ListView GetListView(Pane pane = Pane.Single) => throw new NotImplementedException();

        public HomeView GetHomeView(Pane pane) => throw new NotImplementedException();

        public Footer GetFooter()
        {
            var we = WaitForCss(".footer");
            return new Footer(we, this);
        }

        private Pane GetNewPane(Pane pane, MouseClick button)
        {
            return pane switch
            {
                Pane.Single => button switch { MouseClick.MainButton => Pane.Single, _ => Pane.Right },
                Pane.Left => button switch { MouseClick.MainButton => Pane.Left, _ => Pane.Right },
                _ => button switch { MouseClick.MainButton => Pane.Right, _ => Pane.Left }
            };
        }

       internal ListView WaitForNewListView(View enclosingView, MouseClick button)
        { 
            Pane newPane = GetNewPane(enclosingView.pane, button);
            if (enclosingView is not ListView)
            {
                return new ListView(WaitForCss(CssSelectorFor(newPane, PaneType.List)), this, newPane);
            } else
            {
                //wait for list view new pane where contents differ
                throw new NotImplementedException();
            }
        }

        public ObjectView WaitForNewObjectView(View enclosingView, MouseClick button)
        {
            Pane newPane = GetNewPane(enclosingView.pane, button);
            var css = CssSelectorFor(newPane, PaneType.Object);
            if (enclosingView is not ObjectView)
            {
                return new ObjectView(WaitForCss(css), this, newPane);
            }
            else
            {
                var original = PropsAndColls(enclosingView.element);
                wait.Until(dr => PropsAndColls(dr.FindElement(By.CssSelector(css))) != original);
                return new ObjectView(WaitForCss(css), this, newPane);
            }
        }

        private string PropsAndColls(IWebElement objectView)
        {
            return objectView.FindElement(By.CssSelector("nof-properties")).Text +
                objectView.FindElement(By.CssSelector("nof-collections")).Text;
        }

        internal void Click(IWebElement we, MouseClick button)
        {
            if (button == MouseClick.MainButton) { we.Click(); } else { RightClick(we); }
        }
        #endregion
    }


}