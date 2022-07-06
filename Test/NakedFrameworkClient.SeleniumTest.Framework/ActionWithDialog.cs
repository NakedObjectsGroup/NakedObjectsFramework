using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework; 

public class ActionWithDialog : MenuAction {
    public ActionWithDialog(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

    public override ActionWithDialog AssertIsEnabled() => (ActionWithDialog)base.AssertIsEnabled();

    public override ActionWithDialog AssertHasTooltip(string tooltip) => (ActionWithDialog)base.AssertHasTooltip(tooltip);

    public Dialog Open() {
        helper.wait.Until(el => element.GetAttribute("value") != null);
        var actionName = element.GetAttribute("value");
        helper.Click(element);
        helper.wait.Until(dr => enclosingView.element.FindElement(By.CssSelector(".dialog .title")).Text == actionName);
        var dialogEl = enclosingView.element.FindElement(By.CssSelector(".dialog"));
        return new Dialog(dialogEl, helper, enclosingView);
    }

    public CreateNewView OpenToCreateNewView() {
        var actionName = element.GetAttribute("value");
        helper.Click(element);
        return helper.GetCreateNewView().AssertTitleIs(actionName);
    }
}