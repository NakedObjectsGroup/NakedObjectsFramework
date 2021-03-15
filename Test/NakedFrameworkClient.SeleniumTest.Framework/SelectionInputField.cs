using System;

namespace NakedFrameworkClient.TestFramework
{
    public class SelectionInputField : InputField
    {
        public override SelectionInputField AssertDefaultValueIs(string value) => throw new NotImplementedException();

        public override SelectionInputField AssertIsMandatory() => throw new NotImplementedException();

        public override SelectionInputField AssertIsOptional() => throw new NotImplementedException();

        public override SelectionInputField Clear()=> throw new NotImplementedException();

        public override SelectionInputField Enter(string selection) => throw new NotImplementedException();

        //OptionNumber counts from zero
        public SelectionInputField Select(int optionNumber) => throw new NotImplementedException();

        public SelectionInputField SelectMultiple(params int[] options) => throw new NotImplementedException();

        public SelectionInputField AssertNoOfOptionsIs(int number) => throw new NotImplementedException();

        public SelectionInputField AssertOptionsAre(params string[] titles) => throw new NotImplementedException();

        public SelectionInputField AssertOptionIs(int optionNumber, string title) => throw new NotImplementedException();

    }
}
