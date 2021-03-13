using System;

namespace NakedFrameworkClient.SeleniumTestFramework
{
    public class ValueInputField : InputField
    {
        public override ValueInputField AssertDefaultValueIs(string value) => throw new NotImplementedException();

        public override ValueInputField AssertHasPlaceholder() => throw new NotImplementedException();

        public override ValueInputField AssertIsDisabled() => throw new NotImplementedException();

        public override ValueInputField AssertIsEnabled() => throw new NotImplementedException();

        public override ValueInputField AssertIsMandatory() => throw new NotImplementedException();

        public override ValueInputField AssertIsOptional() => throw new NotImplementedException();

        public override ValueInputField Clear()=> throw new NotImplementedException();

        public override ValueInputField Enter(string characters) => throw new NotImplementedException();
    }
}
