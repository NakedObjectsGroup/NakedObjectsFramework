using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework; 

public class CreateNewView : ActionResult {
    public CreateNewView(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

    public override CreateNewView AssertTitleIs(string expected) {
        Assert.AreEqual(expected, helper.WaitForChildElement(element, ".title").Text);
        return this;
    }

    public CreateNewView AssertHasEmptyProperties(params string[] propertyNames) {
        var props = element.FindElements(By.CssSelector("nof-view-property"));
        Assert.AreEqual(propertyNames.Count(), props.Count, "Number of properties specified does not match the view");
        for (var i = 0; i < props.Count; i++) {
            Assert.AreEqual("", props[i].FindElement(By.CssSelector(".value")).Text);
            Assert.AreEqual(propertyNames[i] + ":", props[i].FindElement(By.CssSelector(".name")).Text);
        }

        return this;
    }

    public Dialog GetDialog() {
        var dialogEl = element.FindElement(By.CssSelector(".dialog"));
        return new Dialog(dialogEl, helper, this);
    }

    public ObjectView ClickSaveToViewSavedObject() {
        helper.wait.Until(el => GetSaveButton().GetAttribute("disabled") is null);
        var save = GetSaveButton();
        save.Click();
        return helper.GetObjectView(pane);
    }

    public CreateNewView AssertSaveIsDisabled(string withMessage = null) {
        helper.wait.Until(el => GetSaveButton().GetAttribute("disabled") is not null);
        var save = GetSaveButton();
        Assert.AreEqual(withMessage.Trim(), save.GetAttribute("title").Trim());
        return this;
    }

    private IWebElement GetSaveButton() => element.FindElement(By.CssSelector("nof-action input[value='Save']"));
}