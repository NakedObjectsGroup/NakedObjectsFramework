using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class ActionWithoutDialog : SubView
    {
        public ActionWithoutDialog(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public ActionWithoutDialog AssertIsEnabled() => throw new NotImplementedException();


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
