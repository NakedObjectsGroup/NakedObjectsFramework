

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace NakedFrameworkClient.TestFramework
{
    public class ObjectView : ActionResult
    {
        public ObjectView(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

        public override ObjectView AssertTitleIs(string expected)
        {         
            Assert.AreEqual(expected, helper.WaitForChildElement(element, ".title").Text);
            return this;
        }

        //Properties. The list of names should be specified in display order
        public ObjectView AssertPropertiesAre(params string[] propertyNames)
        {
            var props = element.FindElements(By.CssSelector("nof-view-property"));
            Assert.AreEqual(propertyNames.Count(), props.Count, "Number of properties specified does not match the view");
            for (int i = 0; i < props.Count; i++)
            {
                Assert.AreEqual(propertyNames[i] + ":", props[i].FindElement(By.CssSelector(".name")).Text);
            }
            return this;
        }

        public Property GetProperty(string propertyName)
        {
            helper.WaitForChildElement(element, "nof-properties");
            var prop = helper.wait.Until(e => element.FindElements(By.CssSelector("nof-view-property"))
                .Single(el => el.FindElement(By.CssSelector(".name")).Text == propertyName + ":"));
            return new Property(prop, helper, this);
        }

        public Property GetProperty(int number)
        {
            helper.WaitForChildElement(element, "nof-properties");
            var prop = element.FindElements(By.CssSelector("nof-view-property")).ElementAt(number);
            return new Property(prop, helper, this);
        }
        public ObjectCollection GetCollection(string collectionName)
        {
            helper.wait.Until(dr => FindCollection(collectionName) != null);
            var coll = FindCollection(collectionName);
            return new ObjectCollection(coll, helper, this);
        }

        private IWebElement FindCollection(string name) =>
            element.FindElements(By.CssSelector("nof-collection"))
                .Single(el => el.FindElement(By.CssSelector(".name")).Text == name + ":");

        public ObjectView DragTitleAndDropOnto(ReferenceInputField field)
        {
            var title = helper.WaitForChildElement(element, ".title");
            helper.CopyToClipboard(title);
            field.PasteReferenceFromClipboard();
            return this;
        }

        public ObjectView AssertIsNotEditable()
        {
            helper.WaitForChildElement(element, "nof-action-bar nof-action");
            helper.wait.Until(dr => element.FindElement(By.CssSelector("nof-action-bar nof-action input")).GetAttribute("value") != "");
            var buttons = element.FindElements(By.CssSelector("nof-action-bar nof-action input")).Select(el => el.GetAttribute("value"));
            Assert.IsTrue(buttons.Contains("Actions"));
            Assert.IsFalse(IsEditable());
            return this;
        }

        public ObjectView AssertIsEditable()
        {
            Assert.IsTrue(IsEditable());
            return this;
        }

        public ObjectEdit Edit()
        {
            helper.WaitForCss("nof-action-bar nof-action input[value=\"Edit\"]").Click();
            return helper.GetObjectEdit();
        }

        private bool IsEditable()
        {
            helper.WaitForChildElement(element, "nof-action-bar nof-action");
            var buttons = element.FindElements(By.CssSelector("nof-action-bar nof-action")).Select(b => b.GetAttribute("value"));
            return buttons.Contains("Edit");
        }



        public ObjectView Reload()
        {
            helper.Reload(pane);
            helper.WaitForNewObjectView(this, MouseClick.MainButton);
            return this;
        }

        public ObjectView FreshView() => helper.GetObjectView(pane);
    }
}
