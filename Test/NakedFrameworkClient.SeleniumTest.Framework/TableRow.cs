using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedFrameworkClient.TestFramework
{
    public class TableRow : SubView
    {
        public TableRow(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public TableRow AssertColumnValuesAre(params string[] textValues) => throw new NotImplementedException();

        public TableRow AssertColumnValueIs(int col, string textValue)
        {
           
            Assert.AreEqual(textValue, GetColumnValue(col));
            return this;
        }

        public ObjectView Click(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();

        public TableRow DragAndDropOnto(ReferenceInputField field)
        {
            helper.CopyToClipboard(element);
            field.PasteReferenceFromClipboard();
            return this;
        }

        public string GetColumnValue(int col) =>
            element.FindElements(By.CssSelector("td")).ElementAt(col + 1).Text;

    }
}