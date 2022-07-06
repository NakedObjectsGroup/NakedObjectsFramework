using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework; 

public class Property : SubView {
    public Property(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

    public Property AssertNameIs(string name) {
        Assert.AreEqual(name + ":", element.FindElement(By.CssSelector(".name")).Text);
        return this;
    }

    public Property AssertValueIs(string expected) {
        if (IsReferenceProperty()) {
            GetReference().AssertTitleIs(expected);
        }
        else {
            Assert.AreEqual(expected, GetValue());
        }

        return this;
    }

    public Property AssertIsMultiLine() {
        element.FindElement(By.CssSelector(".multiline"));
        return this;
    }

    public string GetValue() => element.FindElement(By.CssSelector(".value")).Text;

    private bool IsReferenceProperty() {
        try {
            element.FindElement(By.CssSelector(".reference"));
            return true;
        }
        catch (NoSuchElementException) {
            return false;
        }
    }

    public Reference GetReference() => new(element.FindElement(By.CssSelector(".reference")), helper, enclosingView);

    public Property AssertIsImage() {
        var imgSrc = element.FindElement(By.CssSelector("img")).GetAttribute("src");
        Assert.IsTrue(imgSrc.StartsWith("data:image/gif;"));
        return this;
    }

    public Property AssertIsCheckbox() {
        var box = element.FindElement(By.CssSelector("input"));
        Assert.AreEqual("checkbox", box.GetAttribute("type"));
        return this;
    }

    public Dialog ClickOnEditIcon() {
        var editIcon = element.FindElement(By.CssSelector(".icon.edit"));
        helper.Click(editIcon);
        //TODO: Need to find the dialog from the enclosing view - it is not within the menu action
        var dialogEl = helper.wait.Until(dr => enclosingView.element.FindElement(By.CssSelector("nof-edit-dialog")));
        return new Dialog(dialogEl, helper, enclosingView);
    }
}