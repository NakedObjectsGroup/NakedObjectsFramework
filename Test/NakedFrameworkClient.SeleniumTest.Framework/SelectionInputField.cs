﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedFrameworkClient.TestFramework;

public class SelectionInputField : InputField {
    private readonly SelectElement SelectElement;

    public SelectionInputField(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) => SelectElement = new SelectElement(element.FindElement(By.TagName("select")));

    public override SelectionInputField AssertDefaultValueIs(string value) {
        Assert.AreEqual(value, SelectElement.SelectedOption.Text);
        return this;
    }

    public override SelectionInputField AssertIsMandatory() => throw new NotImplementedException();

    public override SelectionInputField AssertIsOptional() => throw new NotImplementedException();

    public override SelectionInputField Clear() => throw new NotImplementedException();

    public override SelectionInputField Enter(string selection) {
        SelectElement.SelectByText(selection);
        helper.Wait.Until(dr => SelectElement.SelectedOption.Text == selection);
        return this;
    }

    //OptionNumber counts from zero
    public SelectionInputField Select(int optionNumber) {
        SelectElement.SelectByIndex(optionNumber);
        helper.Wait.Until(dr => SelectElement.SelectedOption.Text == SelectElement.Options[optionNumber].Text);
        return this;
    }

    public SelectionInputField SelectMultiple(params int[] options) => throw new NotImplementedException();

    public SelectionInputField AssertNoOfOptionsIs(int number) {
        helper.Wait.Until(dr => number == 0 || SelectElement.Options.Count > 0);
        Assert.AreEqual(number, SelectElement.Options.Count);
        return this;
    }

    public SelectionInputField AssertOptionsAre(params string[] titles) {
        var s = SelectElement.Options;
        for (var i = 0; i < s.Count; i++) {
            Assert.AreEqual(titles[i], s[i].Text);
        }

        return this;
    }

    public SelectionInputField AssertOptionIs(int optionNumber, string title) {
        Assert.AreEqual(title, SelectElement.Options[optionNumber].Text);
        return this;
    }
}