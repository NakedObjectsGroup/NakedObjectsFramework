using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedFrameworkClient.TestFramework
{
    public class Dialog : SubView
    {
        public Dialog(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public Dialog AssertHasFields(params string[] fieldNames) => throw new NotImplementedException();

        public Dialog AssertOKIsEnabled()
        {
            Assert.IsNull(GetOKButton().GetAttribute("disabled"));
            return this;
        }

        public Dialog AssertOKIsDisabled(string withToolTip)
        {
            var ok = GetOKButton();
            Assert.IsNotNull(ok.GetAttribute("disabled"));
            Assert.AreEqual(withToolTip, ok.GetAttribute("title"));
            return this;
        }

        public void ClickOKWithNoResultExpected(MouseClick button = MouseClick.MainButton)
        {
            var ok = GetOKButton().AssertIsEnabled();
            helper.Click(ok, button);
        }

        public ObjectView ClickOKToViewObject(MouseClick button = MouseClick.MainButton)
        {
            var ok = GetOKButton().AssertIsEnabled();
            helper.Click(ok, button);
            return helper.WaitForNewObjectView(enclosingView, button);
        }

        public ListView ClickOKToViewList(MouseClick button = MouseClick.MainButton)
        {
            var ok = GetOKButton().AssertIsEnabled();
            helper.Click(ok, button);
            return helper.WaitForNewListView(enclosingView, button);
        }

        private IWebElement GetOKButton() => element.FindElement(By.CssSelector(".ok"));

        public TextInputField GetTextField(string fieldName)
        {
            var param = element.FindElements(By.CssSelector("nof-edit-parameter")).Single(x => x.Text == fieldName);
            var input = param.FindElement(By.TagName("input"));
            //Not a valid test!
            //Assert.AreEqual("text", input.GetAttribute("type"), $"{fieldName} is not a Text field");
            return new TextInputField(param, helper, enclosingView);
        }

        //TODO: Factor out common logic
        public ReferenceInputField GetReferenceField(string fieldName)
        {
            var param = element.FindElements(By.CssSelector("nof-edit-parameter")).Single(x => x.Text == fieldName);
            var input = param.FindElement(By.TagName("input"));
            Assert.IsTrue(input.GetAttribute("class").Contains("droppable"));
            //Assert.AreEqual("text", input.GetAttribute("type"), $"{fieldName} is not a Text field");
            return new ReferenceInputField(param, helper, enclosingView);
        }
    }
}
