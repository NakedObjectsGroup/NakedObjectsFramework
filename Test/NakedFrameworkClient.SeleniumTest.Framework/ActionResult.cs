using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public abstract class ActionResult : View
    {
        protected ActionResult(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) {}

        public virtual ActionResult AssertTitleIs(string title) => throw new NotImplementedException();

        public Menu OpenActions() => throw new NotImplementedException();

        public virtual ActionResult ClickReload() => throw new NotImplementedException();
    }
}