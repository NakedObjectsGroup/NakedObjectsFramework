﻿using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework;

public class TextInputField : InputField {
    public TextInputField(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

    public override TextInputField AssertDefaultValueIs(string value) {
        Assert.AreEqual(value, Input().GetAttribute("value"));
        return this;
    }

    public TextInputField AssertHasPlaceholder(string text) {
        Assert.AreEqual(text, Input().GetAttribute("placeholder"));
        return this;
    }

    public override TextInputField AssertIsDisabled() => throw new NotImplementedException();

    public override TextInputField AssertIsEnabled() => throw new NotImplementedException();

    public override TextInputField AssertIsMandatory() => throw new NotImplementedException();

    public override TextInputField AssertIsOptional() => throw new NotImplementedException();

    public TextInputField AssertIsEmpty() {
        Assert.AreEqual("", Input().GetAttribute("value"));
        return this;
    }

    public override TextInputField Clear() {
        var input = Input();
        if (input.GetAttribute("value") != "") {
            input.SendKeys(Keys.Control + "a");
            Thread.Sleep(100);
            input.SendKeys(Keys.Delete);
        }

        helper.Wait.Until(dr => input.GetAttribute("value") == "");
        return this;
    }

    public override TextInputField Enter(string characters) {
        var input = Input();
        input.SendKeys(characters);
        helper.Wait.Until(dr => input.GetAttribute("value") == characters);
        return this;
    }

    private IWebElement Input() => element.FindElement(By.TagName("input"));

    public TextInputField AssertIsPassword() {
        Assert.AreEqual("password", Input().GetAttribute("type"));
        return this;
    }

    public TextInputField AssertHasValue(string value) {
        Assert.AreEqual(value, Input().GetAttribute("value"));
        return this;
    }
}