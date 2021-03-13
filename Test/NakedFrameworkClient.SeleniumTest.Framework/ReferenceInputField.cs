using System;

namespace NakedFrameworkClient.SeleniumTestFramework
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

        //Use this to simulate 'dropping' a reference (previously copied to clipboard).
        public ReferenceInputField PasteReferenceFromClipboard() => throw new NotImplementedException();

    }
}
