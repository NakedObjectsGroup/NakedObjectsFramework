

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedFrameworkClient.TestFramework
{
    public class CreateNewView : ActionResult
    {
        public CreateNewView(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

        public override CreateNewView AssertTitleIs(string expected)
        {         
            Assert.AreEqual(expected, helper.WaitForChildElement(element, ".title").Text);
            return this;
        }

        
        public CreateNewView AssertHasEmptyProperties(params string[] propertyNames)
        {
            var props = element.FindElements(By.CssSelector("nof-view-property"));
            Assert.AreEqual(propertyNames.Count(), props.Count, "Number of properties specified does not match the view");
            for (int i = 0; i < props.Count; i++)
            {
                Assert.AreEqual("", props[i].FindElement(By.CssSelector(".value")).Text);
                Assert.AreEqual(propertyNames[i] + ":", props[i].FindElement(By.CssSelector(".name")).Text);
            }
            return this;
        }

        public Dialog GetDialog()
        {
            var dialogEl = element.FindElement(By.CssSelector(".dialog"));
            return new Dialog(dialogEl, helper, this);
        }

        public ObjectView ClickSaveToViewSavedObject()
        {
            var save = GetSaveButton();
            helper.wait.Until(el => save.GetAttribute("disabled") is null) ;
            save.Click();
            return helper.GetObjectView(pane);
        }

        public CreateNewView AssertSaveIsDisabled(string withMessage = null)
        {
            var save = GetSaveButton();
            Assert.IsNotNull(save.GetAttribute("disabled"));
            Assert.AreEqual(withMessage.Trim(), save.GetAttribute("title").Trim());
            return this;
        }

        private IWebElement GetSaveButton() => element.FindElement(By.CssSelector("nof-action input[value='Save']"));
    }
}
