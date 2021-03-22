using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class Property : SubView
    {
        public Property(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public Property AssertNameIs(string name)
        {
            Assert.AreEqual(name + ":", element.FindElement(By.CssSelector(".name")).Text);
            return this;
        }

        public Property AssertValueIs(string expected)
        {
            Assert.AreEqual(expected, GetValue());
            return this;
        }


        public Property AssertIsMultiLine()
        {
            element.FindElement(By.CssSelector(".multiline"));
            return this;
        }

        public string GetValue() => element.FindElement(By.CssSelector(".value")).Text;

        public Reference GetReference() =>
            new Reference(element.FindElement(By.CssSelector(".reference")), helper, enclosingView);

    }
}