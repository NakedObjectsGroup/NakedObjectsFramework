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
            throw new NotImplementedException();
        }

        public ReferenceInputField GetEditableReferenceProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void AssertPropertyIsEnabledForEdit(string propertyName)
        {
            //1. In edit view are ALL the properties nof-edit-property even if disabled?
            //If so, how do we find disabled?

        }

        public void AssertPropertyIsDisabledForEdit(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
