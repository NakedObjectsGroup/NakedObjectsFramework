using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework; 

public class ReferenceInputField : InputField {
    public ReferenceInputField(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

    public override ReferenceInputField AssertDefaultValueIs(string value) => throw new NotImplementedException();

    public override ReferenceInputField AssertHasPlaceholder() => throw new NotImplementedException();

    public override ReferenceInputField AssertIsDisabled() => throw new NotImplementedException();

    public override ReferenceInputField AssertIsEnabled() => throw new NotImplementedException();

    public override ReferenceInputField AssertIsMandatory() => throw new NotImplementedException();

    public override ReferenceInputField AssertIsOptional() => throw new NotImplementedException();

    public override ReferenceInputField Clear() => throw new NotImplementedException();

    public override ReferenceInputField Enter(string characters) {
        var input = Input();
        input.SendKeys(characters);
        helper.wait.Until(dr => input.GetAttribute("value") == characters);
        return this;
    }

    private IWebElement Input() => element.FindElement(By.TagName("input"));

    //Use this to simulate 'dropping' a reference (previously copied to clipboard).
    public ReferenceInputField PasteReferenceFromClipboard() {
        helper.PasteIntoInputField(helper.WaitForChildElement(element, "input"));
        return this;
    }

    public ReferenceInputField AssertSupportsAutoComplete() {
        Assert.IsNotNull(element.FindElement(By.CssSelector("nof-auto-complete")));
        return this;
    }

    public ReferenceInputField AssertHasAutoCompleteOption(int index, string optionText) {
        helper.wait.Until(dr => element.FindElements(By.CssSelector(".suggestions li")).Count > index);
        Assert.AreEqual(optionText, element.FindElements(By.CssSelector(".suggestions li")).ElementAt(index).Text);
        return this;
    }

    public ReferenceInputField SelectAutoCompleteOption(int index) {
        helper.wait.Until(dr => element.FindElements(By.CssSelector(".suggestions li")).Count > index);
        var option = element.FindElements(By.CssSelector(".suggestions li")).ElementAt(index).FindElement(By.CssSelector("a"));
        option.Click();
        return this;
    }
}