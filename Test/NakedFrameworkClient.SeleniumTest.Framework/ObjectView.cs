

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedFrameworkClient.TestFramework
{
    public class ObjectView : ActionResult
    {
        public ObjectView(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

        public override ObjectView AssertTitleIs(string title)
        {         
            Assert.AreEqual(title, helper.WaitForChildElement(element, ".title").Text);
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
            var prop = element.FindElements(By.CssSelector("nof-view-property"))
                .Single(el => el.FindElement(By.CssSelector(".name")).Text == propertyName + ":");
            return new Property(prop, helper, this);
        }

        public ObjectCollection GetCollection(string collectionName) => throw new NotImplementedException();

        public ObjectView DragTitleAndDropOnto(ReferenceInputField field)
        {
            var title = helper.WaitForChildElement(element, ".title");
            helper.CopyToClipboard(title);
            field.PasteReferenceFromClipboard();
            return this;
        }



    }
}
