using System;

namespace NakedFrameworkClient.TestFramework
{

    public class ObjectCollection
    {
        //TODO local collection actions

        ObjectCollection AssertHeaderIs(string header) => throw new NotImplementedException();

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