using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class DateInputField : InputField
    {
        public DateInputField(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public override DateInputField AssertDefaultValueIs(string value) => throw new NotImplementedException();

        public override DateInputField AssertHasPlaceholder() => throw new NotImplementedException();

        public override DateInputField AssertIsDisabled() => throw new NotImplementedException();

        public override DateInputField AssertIsEnabled() => throw new NotImplementedException();

        public override DateInputField AssertIsMandatory() => throw new NotImplementedException();

        public override DateInputField AssertIsOptional() => throw new NotImplementedException();

        public override DateInputField Clear() => throw new NotImplementedException();

        public override DateInputField Enter(string characters) => throw new NotImplementedException();
    }
}
