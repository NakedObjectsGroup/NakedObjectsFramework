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
            var input = Input();
            if (input.GetAttribute("value") != "")
            {
                input.SendKeys(Keys.Control + "a");
                Thread.Sleep(100);
                input.SendKeys(Keys.Delete);
            }
            helper.wait.Until(dr => input.GetAttribute("value") == "");
            return this;
        }

        public override TextInputField Enter(string characters)
        {
            var input = Input();
            input.SendKeys(characters);
            helper.wait.Until(dr => input.GetAttribute("value") == characters);
            return this;
        }

        private IWebElement Input() => element.FindElement(By.TagName("input"));
    }
}
