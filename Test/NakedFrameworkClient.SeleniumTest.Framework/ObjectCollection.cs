using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{

    public class ObjectCollection : SubView
    {
        public ObjectCollection(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        //TODO local collection actions

        public ObjectCollection AssertDetails(string expected)
        {
            var actual = helper.WaitForChildElement(element, ".summary .details").Text;
            Assert.AreEqual(expected, actual);
            return this;
        }

        ObjectCollection AssertNoOfRowsIs(int rows) => throw new NotImplementedException();

        ObjectCollection AssertIsClosed() => throw new NotImplementedException();

        ObjectCollection ClickCloseCollection() => throw new NotImplementedException();

        ObjectCollection AssertIsOpenAsList() => throw new NotImplementedException();

        ObjectCollection ClickListView() => throw new NotImplementedException();

        ObjectCollection AssertIsOpenAsTable() => throw new NotImplementedException();

        ObjectCollection ClickTableView() => throw new NotImplementedException();

        ObjectCollection AssertTableHeaderHasColumns(params string[] columns) => throw new NotImplementedException();

        //Row number counts from zero
        public ObjectCollection CheckRow(int rowNumber) => throw new NotImplementedException();

        //Row number counts from zero
        public TableRow GetRowFromTable(int rowNumber) => throw new NotImplementedException();

        //Row number counts from zero
        public Reference GetRowFromList(int rowNumber) => throw new NotImplementedException();
    }
}