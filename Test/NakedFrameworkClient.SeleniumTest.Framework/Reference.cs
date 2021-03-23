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

        public ObjectView Click(MouseClick button = MouseClick.MainButton)
        {
            element.Click();
            return helper.WaitForNewObjectView(enclosingView, MouseClick.MainButton);
        }
        public Reference DragAndDropOnto(ReferenceInputField field) => throw new NotImplementedException();

        public string GetTitle() => element.Text;
    }
}