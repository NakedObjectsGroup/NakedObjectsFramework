using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework; 

public class ObjectPresentation : ActionResult {
    public ObjectPresentation(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }
}