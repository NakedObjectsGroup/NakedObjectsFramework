using OpenQA.Selenium;
using System;
using System.Threading;

namespace NakedFrameworkClient.TestFramework
{
    public class TextInputField : InputField
    {
        public TextInputField(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public override TextInputField AssertDefaultValueIs(string value) => throw new NotImplementedException();

        public override TextInputField AssertHasPlaceholder() => throw new NotImplementedException();

        public override TextInputField AssertIsDisabled() => throw new NotImplementedException();

        public override TextInputField AssertIsEnabled() => throw new NotImplementedException();

        public override TextInputField AssertIsMandatory() => throw new NotImplementedException();

        public override TextInputField AssertIsOptional() => throw new NotImplementedException();

        public override TextInputField Clear()
        {
            if (element.GetAttribute("value") != "")
            {
                element.SendKeys(Keys.Control + "a");
                Thread.Sleep(100);
                element.SendKeys(Keys.Delete);
            }
            helper.wait.Until(dr => element.GetAttribute("value") == "");
            return this;
        }

        public override TextInputField Enter(string characters)
        {
            element.SendKeys(characters);
            helper.wait.Until(dr => element.GetAttribute("value") == characters);
            return this;
        }
    }
}
