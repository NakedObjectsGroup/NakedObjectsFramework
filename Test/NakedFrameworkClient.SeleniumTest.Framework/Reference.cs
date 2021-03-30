using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class Reference : SubView
    {
        public Reference(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public virtual Reference AssertTitleIs(string title)
        {
            Assert.AreEqual(title, element.Text);
            return this;
        }

        public ObjectView Click(MouseClick button = MouseClick.SecondaryButton)
        {
            helper.Click(element, button);
            return helper.WaitForNewObjectView(enclosingView, MouseClick.MainButton);
        }

        public Reference DragAndDropOnto(ReferenceInputField field)
        {
            helper.CopyToClipboard(element);
            field.PasteReferenceFromClipboard();
            return this;
        }

        public string GetTitle() => element.Text;
    }
}