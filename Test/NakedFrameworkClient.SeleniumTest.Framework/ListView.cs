using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class ListView : ActionResult
    {   //TODO: actions
        public ListView(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

        public override ListView AssertTitleIs(string title)
        {
            Assert.AreEqual(title, element.FindElement(By.CssSelector(".title")).Text);
            return this;
        }

        public ListView AssertHeaderIs(string header) => throw new NotImplementedException();

        public ListView AssertNoOfRowsIs(int rows) => throw new NotImplementedException();

        public ListView AssertIsList() => throw new NotImplementedException();

        public ListView ClickListView() => throw new NotImplementedException();

        public ListView AssertIsTable() => throw new NotImplementedException();

        public ListView ClickTableView() => throw new NotImplementedException();

        public ListView AssertTableHeaderHasColumns(params string[] columns) => throw new NotImplementedException();

        //Row number counts from zero
        public ListView CheckRow(int rowNumber) => throw new NotImplementedException();

        //Row number counts from zero
        public TableRow GetRowFromTable(int rowNumber) => throw new NotImplementedException();
        
        //Row number counts from zero
        public Reference GetRowFromList(int rowNumber)
        {
            //TODO: First assert that it is in display as list mode
            helper.wait.Until(dr => element.FindElements(By.CssSelector("tbody tr td.reference")).Count > rowNumber);
            var row = element.FindElements(By.CssSelector("tbody tr td.reference"))[rowNumber];
            return new Reference(row, helper, this);
        }

    }
}
