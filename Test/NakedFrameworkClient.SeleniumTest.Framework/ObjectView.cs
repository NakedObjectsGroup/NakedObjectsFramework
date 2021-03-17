

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

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

        //Properties, here, includes any collections. The list of names should be specified in display order
        public ObjectView AssertPropertiesAre(params string[] propertyNames) => throw new NotImplementedException();

        public Property GetProperty(string propertyName) => throw new NotImplementedException();

        public ObjectCollection GetCollection(string collectionName) => throw new NotImplementedException();

        public ObjectView DragTitleAndDropOnto(ReferenceInputField field)
        {
            var title = helper.WaitForChildElement(element, ".title");
            helper.CopyToClipboard(title);
            field.PasteReferenceFromClipboard();
            return this;
        }


        //Works only when there is already an open dialog in view - otherwise open one through action menu
        public Dialog GetOpenedDialog()
        {
            var we = helper.WaitForChildElement(element, ".dialog");
            return new Dialog(we, helper, this);
        }
    }
}
