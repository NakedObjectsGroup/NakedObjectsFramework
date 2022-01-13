using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
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

        public SelectionInputField GetEditableSelectionProperty(string propertyName)
        {
            helper.WaitForChildElement(element, "nof-properties");
            var prop = helper.wait.Until(e => element.FindElements(By.CssSelector("nof-edit-property"))
                .Single(el => el.FindElement(By.CssSelector(".name")).Text == propertyName + ":"));
            Assert.IsTrue(prop.FindElement(By.TagName("select")) is not null);
            return new SelectionInputField(prop, helper, this);
        }
    }
}
