﻿using System.Threading;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework;

public class ActionWithoutDialog : MenuAction {
    public ActionWithoutDialog(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

    public override ActionWithoutDialog AssertIsEnabled() => (ActionWithoutDialog)base.AssertIsEnabled();

    public override ActionWithoutDialog AssertHasTooltip(string tooltip) => (ActionWithoutDialog)base.AssertHasTooltip(tooltip);

    public ObjectView ClickToViewObject(MouseClick button = MouseClick.MainButton) {
        element.AssertIsEnabled();
        helper.Click(element, button);
        Thread.Sleep(100);
        return helper.WaitForNewObjectView(enclosingView, button);
    }

    public ObjectEdit ClickToViewTransientObject(MouseClick button = MouseClick.MainButton) {
        element.AssertIsEnabled();
        helper.Click(element, button);
        var pane = helper.GetNewPane(enclosingView.pane, button);
        return helper.GetObjectEdit(pane);
    }

    public ListView ClickToViewList(MouseClick button = MouseClick.MainButton) {
        element.AssertIsEnabled();
        helper.Click(element, button);
        Thread.Sleep(100);
        return helper.WaitForNewListView(enclosingView, button);
    }

    public ListView ClickToViewEmptyList(MouseClick button = MouseClick.MainButton) {
        element.AssertIsEnabled();
        helper.Click(element, button);
        Thread.Sleep(100);
        return helper.WaitForNewEmptyListView(enclosingView, button);
    }

    public void ClickExpectingToStayOnHome() {
        element.AssertIsEnabled();
        helper.Click(element);
    }
}