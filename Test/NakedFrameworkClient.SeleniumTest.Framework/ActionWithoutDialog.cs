using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class ActionWithoutDialog : MenuAction
    {
        public ActionWithoutDialog(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public override ActionWithoutDialog AssertIsEnabled() => (ActionWithoutDialog) base.AssertIsEnabled();

        public override ActionWithoutDialog AssertHasTooltip(string tooltip) => (ActionWithoutDialog) base.AssertHasTooltip(tooltip);


        public ObjectView ClickToViewObject(MouseClick button = MouseClick.MainButton)
        {
            element.AssertIsEnabled();
            helper.Click(element, button);
            return helper.WaitForNewObjectView(enclosingView, button);
        }

        public ListView ClickToViewList(MouseClick button = MouseClick.MainButton)
        {
            element.AssertIsEnabled();
            helper.Click(element, button);
            return helper.WaitForNewListView(enclosingView, button);
        }
    }
}
