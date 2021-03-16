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

        public ObjectView ClickOKToViewObject(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();

        public ListView ClickOKToViewList(MouseClick button = MouseClick.MainButton)
        {
            GetOKButton().AssertIsEnabled().Click(); //TODO just doing left click now
            //TODO not checking if current view type is same (so then checking for different content).
            //Need better helper methods
            //Get new Pane from current pane, mouse click type
            helper.WaitForView(enclosingView.pane, PaneType.List); //TODO temp 
            var selector = helper.CssSelectorFor(enclosingView.pane) + " .list";
            var list = helper.WaitForCss(selector);
            return new ListView(list, helper, enclosingView.pane); //Todo specify new pane
        }

        private IWebElement GetOKButton() => element.FindElement(By.CssSelector(".ok"));

        public TextInputField GetTextField(string fieldName)
        {
            var param = element.FindElements(By.CssSelector("nof-edit-parameter")).Single(x => x.Text == fieldName);
            var input = param.FindElement(By.TagName("input"));
            Assert.AreEqual("text", input.GetAttribute("type"), $"{fieldName} is not a Text field");
            return new TextInputField(input, helper, enclosingView);
        }


        public ReferenceInputField GetReferenceField(string fieldName)
        {
            throw new NotImplementedException();
        }
    }
}
 