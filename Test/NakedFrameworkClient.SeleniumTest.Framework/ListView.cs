using System;

namespace NakedFrameworkClient.TestFramework
{
    public class ListView : ActionResult
    {   //TODO: actions

        public override ListView AssertTitleIs(string title) => throw new NotImplementedException();

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
        public Reference GetRowFromList(int rowNumber) => 
            throw new NotImplementedException(); //Should first assert that it is in list view

    }
}
