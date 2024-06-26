﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace NakedFrameworkClient.TestFramework;

public class Helper {
    public Helper(string baseUrl, string product, IWebDriver webDriver, SafeWebDriverWait safeWebDriverWait) {
        BaseUrl = baseUrl;
        ProductBaseUrl = $"{baseUrl}{product}/";
        WebDriver = webDriver;
        Wait = safeWebDriverWait;
    }

    public string ProductBaseUrl { get; }
    public string BaseUrl { get; }

    public IWebDriver WebDriver { get; }
    public SafeWebDriverWait Wait { get; }

    #region CCAs

    public void WaitForSelectedCheckboxes(int number) {
        Wait.Until(dr => dr.FindElements(By.CssSelector("input[type='checkbox']")).Count(el => el.Selected && el.Enabled) == number);
    }

    #endregion

    public void OpenMainMenu(string menuName) {
        ClickHomeButton();
        WaitForView(Pane.Single, PaneType.Home, "Home");
        var menuSelector = $"nof-menu-bar nof-action input[title=\"{menuName}\"";
        Wait.Until(dr => dr.FindElement(By.CssSelector(menuSelector)));
        var menu = WebDriver.FindElement(By.CssSelector($"nof-menu-bar nof-action input[title=\"{menuName}\"]"));
        Click(menu);
    }

    public void OpenMainMenuAction(string menuName, string actionName) {
        OpenMainMenu(menuName);
        var actionSelector = $"nof-action-list nof-action input[value=\"{actionName}\"]";
        Click(WaitForCss(actionSelector));
    }

    public IWebElement WaitForTitle(string title, Pane pane = Pane.Single) =>
        WaitForTextEquals(CssSelectorFor(pane) + " .title", title);

    public void AssertElementExists(string cssSelector) {
        Wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count >= 1);
    }

    public void WaitUntilElementDoesNotExist(string cssSelector) {
        Wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == 0);
    }

    public void AssertElementCountIs(string cssSelector, int count) {
        Wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector)).Count == count);
    }

    #region Helpers

    public void WaitUntilGone<TResult>(Func<IWebDriver, TResult> condition) {
        Wait.Until(d => {
            try {
                condition(d);
                return false;
            }
            catch (NoSuchElementException) {
                return true;
            }
        });
    }

    public void Maximize() {
        const string script = "window.moveTo(0, 0); window.resizeTo(screen.availWidth, screen.availHeight);";
        ((IJavaScriptExecutor)WebDriver).ExecuteScript(script);
    }

    public void ScrollTo(IWebElement element) {
        var actions = new Actions(WebDriver);
        actions.MoveToElement(element);
        actions.Perform();
    }

    public void Click(IWebElement element) {
        WaitUntilEnabled(element);
        ScrollTo(element);
        element.Click();
    }

    public void WaitUntilEnabled(IWebElement element) {
        Wait.Until(dr => element.GetAttribute("disabled") == null);
    }

    public void RightClick(IWebElement element) {
        var webDriver = Wait.Driver;
        ScrollTo(element);
        var actions = new Actions(webDriver);
        actions.ContextClick(element);
        actions.Perform();
    }

    public IWebElement WaitForCss(string cssSelector) {
        return Wait.Until(d => d.FindElement(By.CssSelector(cssSelector)));
    }

    public IWebElement WaitForChildElement(IWebElement parent, string cssSelector) {
        return Wait.Until(d => parent.FindElement(By.CssSelector(cssSelector)));
    }

    public IWebElement WaitForTextEquals(string cssSelector, string text) {
        Wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).Text.Trim() == text.Trim());
        return WaitForCss(cssSelector);
    }

    public IWebElement WaitForTextEquals(string cssSelector, int index, string text) {
        Wait.Until(dr => dr.FindElements(By.CssSelector(cssSelector))[index].Text == text);
        return WaitForCss(cssSelector);
    }

    public IWebElement WaitForTextStarting(string cssSelector, string startOftext) {
        Wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).Text.StartsWith(startOftext));
        return WaitForCss(cssSelector);
    }

    /// <summary>
    ///     Waits until there are AT LEAST the specified count of matches & returns ALL matches
    /// </summary>
    public ReadOnlyCollection<IWebElement> WaitForCss(string cssSelector, int count) {
        Wait.Until(d => d.FindElements(By.CssSelector(cssSelector)).Count >= count);
        return WebDriver.FindElements(By.CssSelector(cssSelector));
    }

    /// <summary>
    ///     Waits for the Nth match and returns it (counting from zero).
    /// </summary>
    public IWebElement WaitForCssNo(string cssSelector, int number) => WaitForCss(cssSelector, number + 1)[number];

    public void WaitForMessage(string message, Pane pane = Pane.Single) {
        var p = CssSelectorFor(pane);
        Wait.Until(dr => dr.FindElement(By.CssSelector(p + ".header .messages")).Text == message);
    }

    public void ClearFieldThenType(string cssFieldId, string characters) {
        var input = WaitForCss(cssFieldId);
        if (input.GetAttribute("value") != "") {
            input.SendKeys(Keys.Control + "a");
            Thread.Sleep(100);
            input.SendKeys(Keys.Delete);
            Thread.Sleep(100);
            Wait.Until(dr => dr.FindElement(By.CssSelector(cssFieldId)).GetAttribute("value") == "");
        }

        input.SendKeys(characters);
    }

    public void ClearDateFieldThenType(string cssFieldId, string characters) {
        var input = WaitForCss(cssFieldId);
        if (input.GetAttribute("value") != "") {
            for (var i = 0; i < 3; i++) {
                input.SendKeys(Keys.Delete);
                Thread.Sleep(100);
                input.SendKeys(Keys.Tab);
                Thread.Sleep(100);
            }

            input.SendKeys(Keys.Shift + Keys.Tab);
            input.SendKeys(Keys.Shift + Keys.Tab);
            Wait.Until(dr => dr.FindElement(By.CssSelector(cssFieldId)).GetAttribute("value") == "");
        }

        input.SendKeys(characters);
    }

    public void TypeIntoFieldWithoutClearing(string cssFieldId, string characters) {
        var input = WaitForCss(cssFieldId);
        input.SendKeys(characters);
    }

    public void SelectCheckBox(string css, bool alreadySelected = false) {
        Wait.Until(dr => dr.FindElement(By.CssSelector(css)).Selected == alreadySelected);
        var checkbox = WebDriver.FindElement(By.CssSelector(css));
        checkbox.Click();
        Wait.Until(dr => dr.FindElement(By.CssSelector(css)).Selected == !alreadySelected);
    }

    /// <summary>
    ///     Returns a string of n backspace keys for typing into a field
    /// </summary>
    public static string Repeat(string keys, int n) {
        var sb = new StringBuilder();
        for (var i = 0; i < n; i++) {
            sb.Append(keys);
        }

        return sb.ToString();
    }

    public void SelectDropDownOnField(string cssFieldId, string characters) {
        var selected = new SelectElement(WaitForCss(cssFieldId));
        selected.SelectByText(characters);
        Wait.Until(dr => selected.SelectedOption.Text == characters);
    }

    public void SelectDropDownOnField(string cssFieldId, int index) {
        var selected = new SelectElement(WaitForCss(cssFieldId));
        selected.SelectByIndex(index);
    }

    public void WaitForMenus() {
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-menu-bar nof-action")).Count > 1);
    }

    public void GoToMenuFromHomePage(string menuName) {
        WaitForView(Pane.Single, PaneType.Home, "Home");

        WaitForMenus();

        var menus = WebDriver.FindElements(By.CssSelector("nof-action input"));
        var menu = menus.FirstOrDefault(s => s.GetAttribute("value") == menuName);
        if (menu != null) {
            Click(menu);
            Wait.Until(d => d.FindElements(By.CssSelector("nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
        }
        else {
            throw new NotFoundException($"menu not found {menuName}");
        }
    }

    public void OpenObjectActions(Pane pane = Pane.Single) {
        var paneSelector = CssSelectorFor(pane);
        var actions = Wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + "input")).Single(el => el.GetAttribute("value") == "Actions"));
        Click(actions);
        Wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + " nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
    }

    public void OpenSubMenu(string menuName, Pane pane = Pane.Single) {
        var paneSelector = CssSelectorFor(pane);
        var sub = Wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + " .submenu")).Single(el => el.Text == menuName));
        var expand = sub.FindElement(By.CssSelector(".icon-expand"));
        Click(expand);
        Wait.Until(dr => dr.FindElements(By.CssSelector(".icon-collapse")).Count > 0);
    }

    public void OpenMenu(string menuName, Pane pane = Pane.Single) {
        var paneSelector = CssSelectorFor(pane);
        var menu = Wait.Until(dr => dr.FindElements(By.CssSelector(paneSelector + "input")).Single(el => el.GetAttribute("value") == menuName));
        if (menu != null) {
            Click(menu);
            Wait.Until(d => d.FindElements(By.CssSelector("nof-action-list nof-action, nof-action-list div.submenu")).Count > 0);
        }
        else {
            throw new NotFoundException($"menu not found {menuName}");
        }
    }

    public void CloseSubMenu(string menuName) {
        var sub = Wait.Until(dr => dr.FindElements(By.CssSelector(".submenu")).Single(el => el.Text == menuName));
        var expand = sub.FindElement(By.CssSelector(".icon-collapse"));
        Click(expand);
        Assert.IsNotNull(sub.FindElement(By.CssSelector(".icon-expand")));
    }

    public static void Login() {
        Thread.Sleep(2000);
    }

    #endregion

    #region Resulting page view

    public enum ClickType {
        Left,
        Right
    }

    public string GetPropertyValue(string propertyName, Pane pane = Pane.Single) {
        var prop = GetProperty(propertyName, pane);
        return prop.FindElement(By.CssSelector(".value")).Text.Trim();
    }

    public IWebElement GetProperty(string propertyName, Pane pane = Pane.Single) {
        var propCss = CssSelectorFor(pane) + " " + "nof-view-property";
        return Wait.Until(dr => dr
                                .FindElements(By.CssSelector(propCss)).Single(we => we.FindElement(By.CssSelector(".name")).Text == propertyName + ":"));
    }

    public IWebElement GetReferenceFromProperty(string propertyName, Pane pane = Pane.Single) {
        var prop = GetProperty(propertyName, pane);
        return prop.FindElement(By.CssSelector(".reference"));
    }

    public IWebElement GetReferenceProperty(string propertyName, string refTitle, Pane pane = Pane.Single) {
        var propCss = CssSelectorFor(pane) + " " + ".property";
        var prop = Wait.Until(dr => dr
                                    .FindElements(By.CssSelector(propCss)).Single(we => we.FindElement(By.CssSelector(".name")).Text == propertyName + ":" &&
                                                                                        we.FindElement(By.CssSelector(".reference")).Text == refTitle)
        );
        return prop.FindElement(By.CssSelector(".reference"));
    }

    public static string CssSelectorFor(Pane pane) =>
        pane switch {
            Pane.Single => ".single ",
            Pane.Left => "#pane1 ",
            Pane.Right => "#pane2 ",
            _ => throw new NotImplementedException()
        };

    public static string CssSelectorFor(Pane pane, PaneType type) =>
        CssSelectorFor(pane) + " ." + type.ToString().ToLower();

    public void WaitForView(Pane pane, PaneType type, string title = null) {
        var selector = CssSelectorFor(pane) + " ." + type.ToString().ToLower();

        if (title != null) {
            selector += " .header .title";
            Wait.Until(dr => dr.FindElement(By.CssSelector(selector)).Text == title);
        }
        else {
            WaitForCss(selector);
        }

        WaitUntilElementDoesNotExist(pane == Pane.Single ? ".split" : ".single");

        AssertFooterExists();
    }

    public void AssertFooterExists() {
        Wait.Until(d => d.FindElement(By.CssSelector(".footer")));
        Wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.home")).Displayed);
        Wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.back")).Displayed);
        Wait.Until(d => d.FindElement(By.CssSelector(".footer .icon.forward")).Displayed);
    }

    public void AssertTopItemInListIs(string title) {
        var topItem = WaitForCss("tr td.reference").Text;

        Assert.AreEqual(title, topItem);
    }

    #endregion

    #region Editing & Saving

    public void EditObject() {
        Click(EditButton());
        SaveButton();
        GetCancelEditButton();
        var title = WebDriver.FindElement(By.CssSelector(".header .title")).Text;
        Assert.IsTrue(title.StartsWith("Editing"));
    }

    public void SaveObject(Pane pane = Pane.Single) {
        Click(SaveButton(pane));
        EditButton(pane); //To Wait for save completed
        var title = WebDriver.FindElement(By.CssSelector(".header .title")).Text;
        Assert.IsFalse(title.StartsWith("Editing"));
    }

    public void CancelObject(Pane pane = Pane.Single) {
        Click(GetCancelEditButton(pane));
        EditButton(pane); //To Wait for cancel completed
        var title = WebDriver.FindElement(By.CssSelector(".header .title")).Text;
        Assert.IsFalse(title.StartsWith("Editing"));
    }

    public IWebElement GetButton(string text, Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + ".header .action";
        return Wait.Until(dr => dr.FindElements(By.CssSelector(selector)).Single(e => e.Text == text));
    }

    public IWebElement GetInputButton(string text, Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + "input";
        return Wait.Until(dr => dr.FindElements(By.CssSelector(selector)).Single(e => e.GetAttribute("value") == text));
    }

    public IWebElement EditButton(Pane pane = Pane.Single) => GetInputButton("Edit", pane);

    public IWebElement SaveButton(Pane pane = Pane.Single) => GetInputButton("Save", pane);

    public IWebElement SaveVMButton(Pane pane = Pane.Single) => GetInputButton("Save", pane);

    private IWebElement SaveAndCloseButton(Pane pane = Pane.Single) => GetInputButton("Save & Close", pane);

    public IWebElement GetCancelEditButton(Pane pane = Pane.Single) =>
        GetInputButton("Cancel", pane);

    public void ClickHomeButton() {
        Click(WaitForCss(".icon.home"));
    }

    public void ClickBackButton() {
        Click(WaitForCss(".icon.back"));
    }

    public void ClickForwardButton() {
        Click(WaitForCss(".icon.forward"));
    }

    public void ClickRecentButton() {
        Click(WaitForCss(".icon.recent"));
    }

    public void ClickPropertiesButton() {
        Click(WaitForCss(".icon.properties"));
    }

    public void ClickLogOffButton() {
        Click(WaitForCss(".icon.logoff"));
    }

    #endregion

    #region Object Actions

    public ReadOnlyCollection<IWebElement> GetObjectActions(int totalNumber, Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + "nof-action-list nof-action > input";
        Wait.Until(d => d.FindElements(By.CssSelector(selector)).Count == totalNumber);
        return WebDriver.FindElements(By.CssSelector(selector));
    }

    public void AssertAction(int number, string actionName) {
        Wait.Until(dr => dr.FindElements(By.CssSelector("nof-action-list nof-action > input"))[number].GetAttribute("value") == actionName);
    }

    public void AssertActionNotDisplayed(string action) {
        Wait.Until(dr => dr.FindElements(By.CssSelector($"nof-action-list nof-action inputinput[type='{action}']")).FirstOrDefault() == null);
    }

    public IWebElement GetObjectAction(string actionName, Pane pane = Pane.Single, string subMenuName = null) {
        if (subMenuName != null) {
            OpenSubMenu(subMenuName);
        }

        var selector = CssSelectorFor(pane) + $"nof-action-list nof-action input[value='{actionName}']";
        return Wait.Until(d => d.FindElement(By.CssSelector(selector)));
    }

    public IWebElement GetLCA(string actionName, Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + $"nof-collection nof-action input[value='{actionName}']";
        return Wait.Until(d => d.FindElement(By.CssSelector(selector)));
    }

    public IWebElement GetObjectEnabledAction(string actionName, Pane pane = Pane.Single, string subMenuName = null) {
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

    public IWebElement OpenActionDialog(string actionName, Pane pane = Pane.Single, int? noOfParams = null) {
        Click(GetObjectEnabledAction(actionName, pane));

        var dialogSelector = CssSelectorFor(pane) + " .dialog ";
        Wait.Until(d => d.FindElement(By.CssSelector(dialogSelector + "> .title")).Text == actionName);
        //Check it has OK & cancel buttons
        Wait.Until(d => WebDriver.FindElement(By.CssSelector(dialogSelector + ".ok")));
        Wait.Until(d => WebDriver.FindElement(By.CssSelector(dialogSelector + ".cancel")));
        //Wait for params if required
        if (noOfParams != null) {
            Wait.Until(dr => dr.FindElements(By.CssSelector(dialogSelector + " .parameter")).Count == noOfParams.Value);
        }

        return WaitForCss(dialogSelector);
    }

    public IWebElement GetInputNumber(IWebElement dialog, int no) {
        Wait.Until(dr => dialog.FindElements(By.CssSelector(".parameter .value input")).Count >= no + 1);
        return dialog.FindElements(By.CssSelector(".parameter .value input"))[no];
    }

    public IWebElement OKButton() => WaitForCss(".dialog .ok");

    //For use with multi-line dialogs, lineNo starts from zero
    public IWebElement OKButtonOnLine(int lineNo) {
        return Wait.Until(dr => dr.FindElements(By.CssSelector(".lineDialog"))[lineNo].FindElement(By.CssSelector(".ok")));
    }

    public void WaitForOKButtonToDisappear(int lineNo) {
        var line = WaitForCssNo(".lineDialog", lineNo);
        Wait.Until(dr => line.FindElements(By.CssSelector(".ok")).Count == 0);
    }

    public void WaitForReadOnlyEnteredParam(int lineNo, int paramNo, string value) {
        var line = WaitForCssNo(".lineDialog", lineNo);

        Wait.Until(dr => line.FindElements(By.CssSelector(".parameter .value"))[paramNo].Text == value);
    }

    public void CancelDialog(Pane pane = Pane.Single) {
        var selector = CssSelectorFor(pane) + ".dialog ";
        Click(WaitForCss(selector + ".cancel"));

        Wait.Until(dr => {
            try {
                dr.FindElement(By.CssSelector(selector));
                return false;
            }
            catch (NoSuchElementException) {
                return true;
            }
        });
    }

    public void AssertHasFocus(IWebElement el) {
        Wait.Until(dr => dr.SwitchTo().ActiveElement() == el);
    }

    public void Reload(Pane pane = Pane.Single) {
        Click(GetInputButton("Reload", pane));
    }

    public void CancelDatePicker(string cssForInput) {
        var dp = WebDriver.FindElement(By.CssSelector(".ui-datepicker"));
        if (dp.Displayed) {
            WaitForCss(cssForInput).SendKeys(Keys.Escape);
            Wait.Until(br => !br.FindElement(By.CssSelector(".ui-datepicker")).Displayed);
        }
    }

    public void PageDownAndWait() {
        WebDriver.SwitchTo().ActiveElement().SendKeys(Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown + Keys.PageDown);
        Thread.Sleep(1000);
    }

    #endregion

    #region ToolBar icons

    public IWebElement HomeIcon() => WaitForCss(".footer .icon.home");

    public IWebElement SwapIcon() => WaitForCss(".footer .icon.swap");

    public IWebElement FullIcon() => WaitForCss(".footer .icon.full");

    public void GoBack(int clicks = 1) {
        for (var i = 1; i <= clicks; i++) {
            Click(WebDriver.FindElement(By.CssSelector(".icon.back")));
        }
    }

    #endregion

    #region Keyboard navigation

    public void CopyToClipboard(IWebElement element) {
        var title = element.Text;
        element.SendKeys(Keys.Control + "c");
        Wait.Until(dr => dr.FindElement(By.CssSelector(".footer .currentcopy .reference")).Text == title);
    }

    public IWebElement PasteIntoInputField(string cssSelector) {
        var target = WaitForCss(cssSelector);
        var copying = WaitForCss(".footer .currentcopy .reference").Text;
        target.Click();
        target.SendKeys(Keys.Control + "v");
        Wait.Until(dr => dr.FindElement(By.CssSelector(cssSelector)).GetAttribute("value") == copying);
        return WaitForCss(cssSelector);
    }

    public IWebElement PasteIntoInputField(IWebElement target) {
        var copying = WaitForCss(".footer .currentcopy .reference").Text;
        target.Click();
        target.SendKeys(Keys.Control + "v");
        Wait.Until(dr => target.GetAttribute("value") == copying);
        return target;
    }

    public IWebElement Tab(int numberIfTabs = 1) {
        for (var i = 1; i <= numberIfTabs; i++) {
            WebDriver.SwitchTo().ActiveElement().SendKeys(Keys.Tab);
        }

        return WebDriver.SwitchTo().ActiveElement();
    }

    #endregion

    #region New Framework

    //Goes to a single-pane view of home
    public HomeView GotoHome() {
        WebDriver.Navigate().GoToUrl(ProductBaseUrl + "home");
        WaitForView(Pane.Single, PaneType.Home);
        var el = WaitForCss(".home");
        WaitForMenus();
        return new HomeView(el, this);
    }

    //Going via home ensures that any existing object/list view will have been removed before
    //going to new URL
    public Helper GotoUrlViaHome(string url) {
        GotoHome(); //This is to ensure that the view has changed from any existing object/list view
        WebDriver.Navigate().GoToUrl(ProductBaseUrl + url);
        return this;
    }

    public Helper GotoProductUrlDirectly(string url) {
        WebDriver.Navigate().GoToUrl(ProductBaseUrl + url);
        return this;
    }

    public Helper GotoBaseUrlDirectly(string url) {
        WebDriver.Navigate().GoToUrl(BaseUrl + url);
        return this;
    }

    public Helper GotoUrlDirectly(string url) {
        WebDriver.Navigate().GoToUrl(url);
        return this;
    }

    public ObjectView GetObjectView(Pane pane = Pane.Single) {
        WaitForCss(CssSelectorFor(pane) + " .object .title");
        WaitForCss(CssSelectorFor(pane) + " .object .properties");
        WaitForCss(CssSelectorFor(pane) + " .object .collections");
        var el = WaitForCss(CssSelectorFor(pane) + " .object");
        return new ObjectView(el, this, pane);
    }

    public ObjectEdit GetObjectEdit(Pane pane = Pane.Single) {
        var el = WaitForCss(CssSelectorFor(pane) + " .object.edit");
        return new ObjectEdit(el, this, pane);
    }

    public CreateNewView GetCreateNewView(Pane pane = Pane.Single) {
        WaitForCss(CssSelectorFor(pane) + "nof-create-new-dialog .title");
        WaitForCss(CssSelectorFor(pane) + "nof-create-new-dialog .dialog");
        var el = WaitForCss(CssSelectorFor(pane) + "nof-create-new-dialog ");
        return new CreateNewView(el, this, pane);
    }

    public ListView GetReloadedListView(Pane pane = Pane.Single) {
        WaitForCss(CssSelectorFor(pane) + " .list");
        Reload(pane);
        WaitForCss(CssSelectorFor(pane) + " .list table tbody tr");
        var el = WaitForCss(CssSelectorFor(pane) + " .list");
        return new ListView(el, this, pane);
    }

    public ListView GetListView(Pane pane = Pane.Single) {
        WaitForCss(CssSelectorFor(pane) + " .list table tbody tr");
        var el = WaitForCss(CssSelectorFor(pane) + " .list");
        return new ListView(el, this, pane);
    }

    public HomeView GetHomeView(Pane pane = Pane.Single) {
        var el = WaitForCss(CssSelectorFor(pane) + " .home");
        return new HomeView(el, this, pane);
    }

    public Footer GetFooter() {
        var we = WaitForCss(".footer");
        return new Footer(we, this);
    }

    public static Pane GetNewPane(Pane pane, MouseClick button) {
        return pane switch {
            Pane.Single => button switch { MouseClick.MainButton => Pane.Single, _ => Pane.Right },
            Pane.Left => button switch { MouseClick.MainButton => Pane.Left, _ => Pane.Right },
            _ => button switch { MouseClick.MainButton => Pane.Right, _ => Pane.Left }
        };
    }

    public ListView WaitForNewListView(View enclosingView, MouseClick button) {
        var newPane = GetNewPane(enclosingView.pane, button);
        if (enclosingView is not ListView || button == MouseClick.SecondaryButton) {
            return GetListView(newPane);
        }

        if (!enclosingView.element.IsStale()) {
            var css = CssSelectorFor(newPane, PaneType.List) + " table";
            var original = enclosingView.element.FindElement(By.CssSelector("table"));
            Wait.Until(dr => dr.FindElement(By.CssSelector(css)) != original);
        }

        return GetListView(newPane);
    }

    public ListView WaitForNewEmptyListView(View enclosingView, MouseClick button) {
        var pane = GetNewPane(enclosingView.pane, button);
        var details = WaitForCss(CssSelectorFor(pane) + " .list .details");
        Assert.AreEqual("No items found", details.Text);
        var el = WaitForCss(CssSelectorFor(pane) + " .list");
        return new ListView(el, this, pane);
    }

    //Use this method only if the new list view is an updated version of the previous view
    //(this means that the list will need updating).
    public ListView WaitForUpdatedListView(View enclosingView, MouseClick button) {
        var newPane = GetNewPane(enclosingView.pane, button);
        var css = CssSelectorFor(newPane, PaneType.List) + " table";
        Wait.Until(dr => dr.FindElements(By.CssSelector(css)).Count == 0);
        Reload();
        return GetListView(newPane);
    }

    public ObjectView WaitForNewObjectView(View enclosingView, MouseClick button) {
        var newPane = GetNewPane(enclosingView.pane, button);
        if (enclosingView is not ObjectView || button == MouseClick.SecondaryButton) {
            return GetObjectView(newPane);
        }

        if (!enclosingView.element.IsStale()) {
            var css = CssSelectorFor(newPane, PaneType.Object);
            var original = PropsAndColls(enclosingView.element);
            Wait.Until(dr => PropsAndColls(dr.FindElement(By.CssSelector(css))) != original);
        }

        return GetObjectView(newPane);
    }

    public static string PropsAndColls(IWebElement objectView) =>
        objectView.FindElement(By.CssSelector("nof-properties")).Text +
        objectView.FindElement(By.CssSelector("nof-collections")).Text;

    public void Click(IWebElement we, MouseClick button) {
        if (button == MouseClick.MainButton) {
            we.Click();
        }
        else {
            RightClick(we);
        }
    }

    #endregion
}