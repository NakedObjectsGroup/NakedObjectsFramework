﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace NakedFrameworkClient.TestFramework
{
    public class Reference : SubView
    {
        public Reference(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public virtual Reference AssertTitleIs(string expected)
        {
            Assert.AreEqual(expected, element.Text);
            return this;
        }

        public ObjectView Click(MouseClick button = MouseClick.MainButton)
        {
            helper.Click(element, button);
            Thread.Sleep(100);
            return helper.WaitForNewObjectView(enclosingView, button);
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