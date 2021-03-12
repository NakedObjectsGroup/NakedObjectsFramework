using System;

namespace NakedFunctions.Selenium.Test.Framework
{
    public class ReferenceInputField : InputField
    {
        public override ReferenceInputField AssertDefaultValueIs(string value) => throw new NotImplementedException();

        public override ReferenceInputField AssertHasPlaceholder() => throw new NotImplementedException();

        public override ReferenceInputField AssertIsDisabled() => throw new NotImplementedException();

        public override ReferenceInputField AssertIsEnabled() => throw new NotImplementedException();

        public override ReferenceInputField AssertIsMandatory() => throw new NotImplementedException();

        public override ReferenceInputField AssertIsOptional() => throw new NotImplementedException();

        public override ReferenceInputField Clear()=> throw new NotImplementedException();

        public override ReferenceInputField Enter(string characters) => throw new NotImplementedException();
    }
}
