using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework;

public abstract class ObjectPresentation : ActionResult {
    protected ObjectPresentation(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }
}