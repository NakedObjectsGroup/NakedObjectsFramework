using OpenQA.Selenium;


namespace NakedFrameworkClient.TestFramework
{
    public class ActionWithDialog : MenuAction
    {
        public ActionWithDialog(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public override ActionWithDialog AssertIsEnabled() => (ActionWithDialog) base.AssertIsEnabled();

        public override ActionWithDialog AssertHasTooltip(string tooltip) => (ActionWithDialog) base.AssertHasTooltip(tooltip);

        public Dialog Open()
        {
            var actionName = element.GetAttribute("value");
            helper.Click(element);
            //TODO: Need to find the dialog from the enclosing view - it is not within the menu action
            helper.wait.Until(dr => enclosingView.element.FindElement(By.CssSelector(".dialog .title")).Text == actionName);
            var dialogEl = enclosingView.element.FindElement(By.CssSelector(".dialog"));
            return new Dialog(dialogEl, helper, enclosingView);
        }
    }
}
