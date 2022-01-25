using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedFrameworkClient.TestFramework
{
    public class ObjectEdit : ObjectPresentation
    {
        public ObjectEdit(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

        public ObjectView Cancel()
        {
            helper.WaitForCss("nof-action-bar nof-action input[value=\"Cancel\"]").Click();
            return helper.GetObjectView();
        }

        public ObjectView Save()
        {
            helper.WaitForCss("nof-action-bar nof-action input[value=\"Save\"]").Click();
            return helper.GetObjectView();
        }

        public ObjectEdit AttemptUnsuccessfulSave()
        {
            helper.WaitForCss("nof-action-bar nof-action input[value=\"Save\"]").Click();
            helper.wait.Until(e => e.FindElement(By.CssSelector(".messages")).Text != "");
            return this;
        }

        public override ObjectEdit AssertTitleIs(string expected)
        {
            Assert.AreEqual(expected, helper.WaitForChildElement(element, ".title").Text);
            return this;
        }

        private IWebElement GetEditableProperty(string propertyName)
        {
            helper.WaitForChildElement(element, "nof-properties");
            return helper.wait.Until(e => element.FindElements(By.CssSelector("nof-edit-property"))
                .Single(el => el.FindElement(By.CssSelector(".name")).Text == propertyName + ":"));
        }

        public SelectionInputField GetEditableSelectionProperty(string propertyName)
        {
            var prop = GetEditableProperty(propertyName);
            Assert.IsTrue(prop.FindElement(By.TagName("select")) is not null);
            return new SelectionInputField(prop, helper, this);
        }

        public TextInputField GetEditableTextInputProperty(string propertyName)
        {
            var prop = GetEditableProperty(propertyName);
            Assert.IsTrue(prop.FindElement(By.TagName("input")) is not null);
            return new TextInputField(prop, helper, this);
        }

        public ReferenceInputField GetEditableReferenceProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public ObjectEdit AssertPropertyIsDisabledForEdit(string propertyName)
        {
            GetEditableProperty(propertyName).FindElement(By.CssSelector(".value"));
            return this;
        }

    }
}
