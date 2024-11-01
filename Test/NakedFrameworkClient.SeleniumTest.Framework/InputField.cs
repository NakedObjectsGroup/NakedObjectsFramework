﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework;

public abstract class InputField : SubView {
    protected InputField(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

    public virtual InputField Clear() => throw new NotImplementedException();

    public virtual InputField Enter(string characters) => throw new NotImplementedException();

    public virtual InputField AssertIsMandatory() => throw new NotImplementedException();

    public virtual InputField AssertIsOptional() => throw new NotImplementedException();

    public virtual InputField AssertIsEnabled() => throw new NotImplementedException();

    public virtual InputField AssertIsDisabled() => throw new NotImplementedException();

    public virtual InputField AssertHasPlaceholder() => throw new NotImplementedException();

    public virtual InputField AssertDefaultValueIs(string value) => throw new NotImplementedException();

    public virtual InputField AssertNoValidationError() {
        helper.Wait.Until(dr => element.FindElement(By.CssSelector(".validation")).Text == "");
        return this;
    }

    public virtual InputField AssertHasValidationError(string message) {
        helper.Wait.Until(dr => element.FindElement(By.CssSelector(".validation")).Text != "");
        Assert.AreEqual(message, element.FindElement(By.CssSelector(".validation")).Text);
        return this;
    }
}