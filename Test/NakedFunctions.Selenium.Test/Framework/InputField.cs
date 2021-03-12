using System;

namespace NakedFunctions.Selenium.Test.Framework
{
    public abstract class InputField
    {
        public virtual InputField Clear() => throw new NotImplementedException();

        public virtual InputField Enter(string characters) => throw new NotImplementedException();

        public virtual InputField AssertIsMandatory() => throw new NotImplementedException();

        public virtual InputField AssertIsOptional() => throw new NotImplementedException();

        public virtual InputField AssertIsEnabled() => throw new NotImplementedException();

        public virtual InputField AssertIsDisabled() => throw new NotImplementedException();

        public virtual InputField AssertHasPlaceholder() => throw new NotImplementedException();

        public virtual InputField AssertDefaultValueIs(string value) => throw new NotImplementedException();

        public virtual InputField AssertNoValidationError() => throw new NotImplementedException();

        public virtual InputField AssertHasValidationError() => throw new NotImplementedException();
    }
}