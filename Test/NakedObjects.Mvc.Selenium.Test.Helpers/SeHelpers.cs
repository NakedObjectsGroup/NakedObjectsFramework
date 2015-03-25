// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace NakedObjects.Mvc.Selenium.Test.Helper {
    public static class SeHelpers {
        public static IWebElement BrowserSpecificCheck(this IWebElement element, IWebDriver webDriver) {
            if (webDriver is InternetExplorerDriver) {
                element.SendKeys(Keys.Space);
            }
            else {
                element.Click();
            }
            return element;
        }

        #region new helpers

        // Find then click action and wait unitil field selector returns an element 
        public static IWebElement ClickAndWait(this SafeWebDriverWait wait, string actionSelector, string fieldSelector, int delay = 0) {
            IWebElement action = wait.Driver.FindElement(By.CssSelector(actionSelector));
            return wait.ClickAndWait(action, fieldSelector);
        }

        // Click action and wait unitil field selector returns an element 
        public static IWebElement ClickAndWait(this SafeWebDriverWait wait, IWebElement action, string fieldSelector, int delay = 0) {
            if (delay > 0) {
                Thread.Sleep(delay);
            }
            action.Click();
            IWebElement field = null;
            wait.Until(wd => (field = wd.FindElement(By.CssSelector(fieldSelector))) != null);
            Assert.IsNotNull(field);
            return field;
        }

        // Find then click action and wait until field selector stops returning an element 
        public static void ClickAndWaitGone(this SafeWebDriverWait wait, string actionSelector, string fieldSelector, int delay = 0) {
            IWebElement action = wait.Driver.FindElement(By.CssSelector(actionSelector));
            wait.ClickAndWaitGone(action, fieldSelector);
        }

        private static IWebElement SafeFunc(Func<IWebElement> f) {
            try {
                return f();
            }
            catch {
                return null;
            }
        }

        // Click action and wait until field selector stops returning an element 
        // must exist before action is invoked
        public static void ClickAndWaitGone(this SafeWebDriverWait wait, IWebElement action, string fieldSelector, int delay = 0) {
            if (delay > 0) {
                Thread.Sleep(delay);
            }
            IWebElement field = wait.Driver.FindElement(By.CssSelector(fieldSelector));
            Assert.IsNotNull(field);
            action.Click();
            wait.Until(wd => (field = SafeFunc(() => wd.FindElement(By.CssSelector(fieldSelector)))) == null);
            Assert.IsNull(field);
        }

        // find then click action then wait until f returns true
        public static void ClickAndWait(this SafeWebDriverWait wait, string actionSelector, Func<IWebDriver, bool> f) {
            IWebElement action = wait.Driver.FindElement(By.CssSelector(actionSelector));
            wait.ClickAndWait(action, f);
        }

        // click action then wait until f returns true
        public static void ClickAndWait(this SafeWebDriverWait wait, IWebElement action, Func<IWebDriver, bool> f) {
            action.Click();
            wait.Until(f);
        }

        #endregion

        #region Asserts


        public static IWebElement AssertSummaryEquals(this IWebElement collection, string expected) {
            AssertIsCollection(collection);
            Assert.IsTrue(collection.FindElements(By.TagName("div"))[1].GetAttribute("class") == "nof-collection-summary", "Collection is not in Summary view");
            Assert.AreEqual(expected, collection.FindElement(By.CssSelector("div.nof-object")).Text);
            return collection;
        }

        private static void AssertIsCollection(IWebElement collection) {
            try {
                collection.FindElement(By.CssSelector("div.nof-collection-table, div.nof-collection-list, div.nof-collection-summary"));
            }
            catch (NoSuchElementException) {
                Assert.Fail("Selected Div is not a Collection");
            }
        }

        private static IWebDriver AssertContainsElementWithClass(this IWebDriver webDriver, string className) {
            try {
                webDriver.FindElement(By.ClassName(className));
            }
            catch (NoSuchElementException) {
                Assert.Fail();
            }

            return webDriver;
        }

        public static IWebDriver AssertContainsObjectView(this IWebDriver webDriver) {
            return webDriver.AssertContainsElementWithClass("nof-objectview");
        }

        public static IWebDriver AssertContainsObjectEdit(this IWebDriver webDriver) {
            return webDriver.AssertContainsElementWithClass("nof-objectedit");
        }

        public static IWebDriver AssertContainsObjectEditTransient(this IWebDriver webDriver) {
            try {
                IWebElement elem = webDriver.FindElement(By.ClassName("nof-objectedit"));
                var cls = elem.GetAttribute("class");

                Assert.IsTrue(cls.Contains("nof-objectedit"));
                Assert.IsTrue(cls.Contains("nof-transient"));
                Assert.IsTrue(cls.Replace("nof-transient", "").Replace("nof-objectedit", "").Trim().Length == 0);
            }
            catch (NoSuchElementException) {
                Assert.Fail();
            }

            return webDriver;
        }

        public static IWebDriver AssertPageTitleEquals(this IWebDriver webDriver, string expectedTitle) {
            Assert.AreEqual(expectedTitle, webDriver.Title);
            return webDriver;
        }

        public static void AssertElementExists(this IWebDriver webDriver, By by) {
            try {
                webDriver.FindElement(by);
            }
            catch (WebDriverException) //Should be NoSuchElementException, but this doesn't work on Firefox
            {
                Assert.Fail("Element should exist");
            }
        }

        public static void AssertElementDoesNotExist(this IWebDriver webDriver, By by) {
            try {
                webDriver.FindElement(by);
                Assert.Fail("Element should  not exist");
            }
            catch (WebDriverException) {
                //Should be NoSuchElementException, but this doesn't work on Firefox
                //As expected; test is OK
            }
        }

        public static IWebElement AssertValueEquals(this IWebElement field, string expectedValue) {
            return field.FindElement(By.ClassName("nof-value")).AssertTextEquals(expectedValue);
        }

        public static IWebElement AssertTextEquals(this IWebElement field, string expectedValue) {
            Assert.AreEqual(expectedValue, field.Text);
            return field;
        }

        public static IWebElement AssertInputValueEquals(this IWebElement field, string expectedValue) {
            Assert.AreEqual(expectedValue, field.FindElement(By.TagName("input")).GetAttribute("value"));
            return field;
        }

        public static IWebElement AssertInputValueNotEquals(this IWebElement field, string expectedValue) {
            Assert.AreNotEqual(expectedValue, field.FindElement(By.TagName("input")).GetAttribute("value"));
            return field;
        }

        public static IWebElement AssertIsEmpty(this IWebElement field) {
            ReadOnlyCollection<IWebElement> links = field.FindElement(By.ClassName("nof-object")).FindElements(By.TagName("a"));
            Assert.AreEqual(0, links.Count());
            return field;
        }

        public static IWebElement AssertHasMandatoryIndicator(this IWebElement field) {
            try {
                field.FindElement(By.ClassName("nof-mandatory-field-indicator"));
            }
            catch (NoSuchElementException) {
                Assert.Fail();
            }

            return field;
        }

        public static IWebElement AssertValidationErrorIs(this IWebElement field, string error) {
            try {
                Assert.AreEqual(error, field.FindElement(By.CssSelector("span.field-validation-error")).Text);
            }
            catch (NoSuchElementException) {
                Assert.Fail();
            }

            return field;
        }

        public static IWebElement AssertNoValidationError(this IWebElement field) {
            try {
                field.FindElement(By.CssSelector("span.field-validation-error"));
                Assert.Fail("unexpected validation error");
            }
            catch (NoSuchElementException) {
                // expected  
            }
            return field;
        }

        public static IWebElement AssertObjectHasTitle(this IWebElement field, string expectedTitle) {
            string actual = field.FindElement(By.ClassName("nof-object")).FindElement(By.TagName("a")).Text;
            Assert.AreEqual(expectedTitle, actual);
            return field;
        }

        public static IWebElement AssertIsModifiable(this IWebElement field) {
            Assert.AreEqual(1, field.FindElements(By.TagName("input")).Count);
            Assert.AreEqual(field.GetAttribute("id") + "-Input", field.FindElement(By.TagName("input")).GetAttribute("id"));
            return field;
        }

        public static IWebElement AssertIsUnmodifiable(this IWebElement field) {
            Assert.AreEqual(0, field.FindElements(By.TagName("input")).Count);
            return field;
        }


        public static IWebElement AssertSelectedDropDownItemIs(this IWebElement field, string expected) {
            Assert.AreEqual(expected, field.FindElements(By.TagName("option")).Where(o => o.GetAttribute("selected") != null).Select(o => o.Text).SingleOrDefault());
            return field;
        }

        #endregion

        #region Tabbed History

        private enum ClearType {
            ClearThis =1,
            ClearOthers,
            ClearAll
        };

        private static void ClickClearContextMenu(this SafeWebDriverWait wait, int index, ClearType clearType) {
            var tabCount = wait.Driver.FindElements(By.CssSelector(".nof-tab")).Count;
            var newCount = clearType == ClearType.ClearThis ? tabCount - 1 : clearType == ClearType.ClearOthers ? 1 : 0;

            var webDriver = wait.Driver;
            var tab = webDriver.FindElements(By.CssSelector(".nof-tab"))[index];
            var loc = (ILocatable) tab;
            var mouse = ((IHasInputDevices) webDriver).Mouse;
            mouse.ContextClick(loc.Coordinates);

            var selector = string.Format(".ui-menu-item:nth-of-type({0}) a", (int)clearType);
            wait.Until(wd => wd.FindElement(By.CssSelector(selector)));
            wait.ClickAndWait(selector, wd => wd.FindElements(By.CssSelector(".nof-tab")).Count == newCount);
        }

        public static void ClickClearItem(this SafeWebDriverWait wait, int index) {
             wait.ClickClearContextMenu(index, ClearType.ClearThis);
        }

        public static void ClickClearOthers(this SafeWebDriverWait wait, int index) {
             wait.ClickClearContextMenu(index, ClearType.ClearOthers);
        }

        public static void ClickClearAll(this SafeWebDriverWait wait, int index) {
             wait.ClickClearContextMenu(index, ClearType.ClearAll);
        }

        #endregion

        #region Fields

        public static void TypeText(this IWebElement field, string text) {
            field.Clear();
            field.SendKeys(text);
        }

        #endregion

        #region Fields with Drop downs

        public static IWebElement SelectDropDownItem(this IWebElement field, string name, IWebDriver br) {
            field.FindElement(By.TagName("select")).SendKeys(name);
            field.FindElement(By.TagName("select")).SendKeys(Keys.Tab);
            return field;
        }

        public static IWebElement SelectListBoxItems(this IWebElement field, IWebDriver br, params string[] names) {
            IWebElement select = field.FindElement(By.TagName("select"));
            IWebElement[] options = select.FindElements(By.TagName("option")).Where(o => names.Contains(o.Text)).ToArray();
            Assert.AreEqual(names.Count(), options.Count(), "all options not found in list");

            foreach (string name in names) {
                IWebElement option = options.Single(o => o.Text == name);

                IKeyboard kb = ((IHasInputDevices) br).Keyboard;

                kb.PressKey(Keys.Control);
                option.Click();
                kb.ReleaseKey(Keys.Control);
            }
            select.SendKeys(Keys.Tab);

            return field;
        }

        #endregion

        #region checkboxes

        public static IWebElement CheckAll(this IWebElement collection, IWebDriver br) {
            IWebElement all = collection.FindElement(By.Id("checkboxAll"));
            Assert.IsFalse(all.Selected, "Box is already checked");
            all.BrowserSpecificCheck(br);
            Thread.Sleep(1000); // for javascript to run 
            return collection;
        }

        public static IWebElement UnCheckAll(this IWebElement collection, IWebDriver br) {
            IWebElement all = collection.FindElement(By.Id("checkboxAll"));

            Assert.IsTrue(all.Selected, "Box is not checked");
            all.BrowserSpecificCheck(br);
            Thread.Sleep(1000); // for javascript to run 
            return collection;
        }

        #endregion

        #region Row-based operations

        public static IWebElement GetRow(this IWebElement collection, int row) {
            IWebElement table = collection.FindElement(By.TagName("table"));
            int rowNumber = row;
      
            ReadOnlyCollection<IWebElement> rows = table.FindElements(By.TagName("tr"));
            Assert.IsTrue(rowNumber >= 0 && rowNumber <= rows.Count - 1, "Row number is out of range for table");
            return rows.ElementAt(rowNumber);
        }

        public static IWebElement CheckRow(this IWebElement row, IWebDriver br) {
            IWebElement box = row.FindElement(By.CssSelector("input[type=checkbox]"));
            Assert.IsFalse(box.Selected, "Box is already checked");
            box.BrowserSpecificCheck(br);
            return row;
        }

        public static IWebElement UnCheckRow(this IWebElement row, IWebDriver br) {
            IWebElement box = row.FindElement(By.CssSelector("input[type=checkbox]"));
            Assert.IsTrue(box.Selected, "Box is not checked");
            box.BrowserSpecificCheck(br);
            return row;
        }

        #endregion

        #region Cell-based operations

        /// <summary>
        /// Returns the text contents of a table cell which may be a value, or the title of a reference object
        /// </summary>
        /// <param name="coll">Collection that is expected to contain a Table</param>
        /// <param name="rowNumber">'0' is typically the header row</param>
        /// <param name="column">Leftmost column is '0', which may be a selector box</param>
        /// <returns></returns>
        public static string TextContentsOfCell(this IWebElement coll, int rowNumber, int column) {
            return coll.FindElements(By.TagName("tr")).ElementAt(rowNumber).FindElements(By.TagName("td")).ElementAt(column).Text;
        }

        #endregion
     
    }
}