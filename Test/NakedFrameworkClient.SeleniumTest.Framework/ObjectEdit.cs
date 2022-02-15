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

        public ObjectView Save(Pane pane = Pane.Single)
        {
            GetSaveButton().Click();
            return helper.GetObjectView(pane);
        }

        private IWebElement GetSaveButton()
        {
            return helper.WaitForCss("nof-action-bar nof-action input[value=\"Save\"]");
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
            var prop = GetEditableProperty(propertyName);
            Assert.IsTrue(prop.FindElement(By.TagName("input")) is not null);
            return new ReferenceInputField(prop, helper, this);
        }

        public ObjectEdit AssertPropertyIsDisabledForEdit(string propertyName)
        {
            GetEditableProperty(propertyName).FindElement(By.CssSelector(".value"));
            return this;
        }

        //Properties. The list of names should be specified in display order
        public ObjectEdit AssertPropertiesAre(params string[] propertyNames)
        {
            var props = element.FindElements(By.CssSelector("nof-edit-property"));
            Assert.AreEqual(propertyNames.Count(), props.Count, "Number of properties specified does not match the view");
            for (int i = 0; i < props.Count; i++)
            {
                Assert.AreEqual(propertyNames[i] + ":", props[i].FindElement(By.CssSelector(".name")).Text);
            }
            return this;
        }

        public ObjectEdit AssertSaveIsEnabled(string message = "")
        {
            GetSaveButton().AssertIsEnabled().AssertHasTooltip(message);
            return this;
        }

        public ObjectEdit AssertSaveIsDisabled(string message = "")
        {
            GetSaveButton().AssertIsDisabled().AssertHasTooltip(message);
            return this;
        }
    }
}
