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

        public virtual Dialog AssertNoValidationError() =>
          AssertHasValidationError(string.Empty);

        public virtual Dialog AssertHasValidationError(string message)
        {
            helper.wait.Until(dr => element.FindElement(By.CssSelector(".co-validation")).Text != "");
            Assert.AreEqual(message, element.FindElement(By.CssSelector(".co-validation")).Text);
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
            var ok = GetEnabledOKButton();
            helper.Click(ok, button);
        }

        public ObjectView ClickOKToViewObject(MouseClick button = MouseClick.MainButton)
        {
            var element = enclosingView.element;
            var oldText =element.Text;
            var pane = helper.GetNewPane(enclosingView.pane, button);
            helper.Click(GetEnabledOKButton());
            if (enclosingView is ObjectView && button == MouseClick.MainButton)
            {
                helper.wait.Until(dr => element.IsStale() || element.Text != oldText);
            }
            return helper.GetObjectView(pane);
        }

        public ListView ClickOKToViewNewList(MouseClick button = MouseClick.MainButton)
        {
            var ok = GetEnabledOKButton();
            helper.Click(ok, button);
            return helper.WaitForNewListView(enclosingView, button);
        }

        public ListView ClickOKToViewUpdatedList(MouseClick button = MouseClick.MainButton)
        {
            var ok = GetEnabledOKButton();
            helper.Click(ok, button);
            return helper.WaitForUpdatedListView(enclosingView, button);
        }

        private IWebElement GetOKButton() => element.FindElement(By.CssSelector(".ok"));

        private IWebElement GetEnabledOKButton()
        {
            try
            {
                helper.wait.Until(dr => GetOKButton().GetAttribute("disabled") == null);
            }
            catch (Exception)
            {
                Assert.Fail("OK button was disabled");
            }
            return GetOKButton();
        }

        public TextInputField GetTextField(string fieldName)
        {
            var field = element.FindElements(By.CssSelector("nof-edit-parameter"))
                .Single(x => x.FindElement(By.CssSelector("label")).Text == fieldName);
            var input = field.FindElement(By.TagName("input"));
            //Not a valid test!
            //Assert.AreEqual("text", input.GetAttribute("type"), $"{fieldName} is not a Text field");
            return new TextInputField(field, helper, enclosingView);
        }

        //TODO: Factor out common logic
        public ReferenceInputField GetReferenceField(string fieldName)
        {
            var field = element.FindElements(By.CssSelector("nof-edit-parameter")).Single(x => x.Text == fieldName);
            var input = field.FindElement(By.TagName("input"));
            Assert.IsTrue(input.GetAttribute("class").Contains("droppable"));
            //Assert.AreEqual("text", input.GetAttribute("type"), $"{fieldName} is not a Text field");
            return new ReferenceInputField(field, helper, enclosingView);
        }

        public SelectionInputField GetSelectionField(string fieldName)
        {
            var field = element.FindElements(By.CssSelector("nof-edit-parameter")).Single(x => x.FindElement(By.CssSelector(".name")).Text == fieldName);
            Assert.IsTrue(field.FindElement(By.TagName("select")) is not null);
            return new SelectionInputField(field, helper, enclosingView);
        }

        public Dialog AssertHasImageField(string fieldName)
        {
            var field = element.FindElements(By.CssSelector("nof-edit-parameter")).Single(x => x.FindElement(By.CssSelector(".name")).Text == fieldName);
            Assert.IsTrue(field.FindElement(By.CssSelector("input[type='file']")) is not null);
            return this;
        }
    }
}
