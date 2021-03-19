using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

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

        public ListView AssertDetails(string expected)
        {
            var actual = helper.WaitForChildElement(element, ".summary .details").Text;
            Assert.AreEqual(expected, actual);
            return this;
        }

        public ListView AssertNoOfRowsIs(int rows)
        {
            Assert.AreEqual(rows, element.FindElements(By.CssSelector("table tbody tr")).Count);
            return this;
        }

        public ListView AssertIsList() => throw new NotImplementedException();

        public ListView ClickListView() => throw new NotImplementedException();

        public ListView AssertIsTable()
        {
            var head = element.FindElement(By.CssSelector("thead"));
            Assert.IsTrue(head.FindElements(By.CssSelector("th")).Any());
            return this;
        }

        public ListView ClickTableView()
        {
            element.FindElement(By.CssSelector(".icon.table")).Click();
            helper.WaitForChildElement(element, "thead th");
            return this;
        }

        public ListView AssertTableHeaderHasColumns(params string[] columnNames)
        {
            var cols = element.FindElements(By.CssSelector("thead th"));
            Assert.AreEqual(columnNames.Count(), cols.Count);
            for (int i = 0; i < columnNames.Count(); i++)
            {
                Assert.AreEqual(columnNames[i], cols[i].Text);
            }
            return this;
        }

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
