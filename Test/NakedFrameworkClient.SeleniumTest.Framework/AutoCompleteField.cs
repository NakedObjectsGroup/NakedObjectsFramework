using System;
using OpenQA.Selenium;

namespace NakedFrameworkClient.TestFramework; 

public class AutoCompleteField : ReferenceInputField {
    public AutoCompleteField(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

    public override AutoCompleteField AssertDefaultValueIs(string value) => throw new NotImplementedException();

    public override AutoCompleteField AssertHasPlaceholder() => throw new NotImplementedException();

    public override AutoCompleteField AssertIsDisabled() => throw new NotImplementedException();

    public override AutoCompleteField AssertIsEnabled() => throw new NotImplementedException();

    public override AutoCompleteField AssertIsMandatory() => throw new NotImplementedException();

    public override AutoCompleteField AssertIsOptional() => throw new NotImplementedException();

    public override AutoCompleteField Clear() => throw new NotImplementedException();

    public override AutoCompleteField Enter(string characters) => throw new NotImplementedException();
}