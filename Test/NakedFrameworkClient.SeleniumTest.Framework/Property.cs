using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class Property : SubView
    {
        public Property(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public Property AssertValueIs(string expected)
        {
            Assert.AreEqual(expected, element.FindElement(By.CssSelector(".value")).Text);
            return this;
        }

        public Property AssertIsMultiLine()
        {
            element.FindElement(By.CssSelector(".multiline"));
            return this;
        }
    }
}