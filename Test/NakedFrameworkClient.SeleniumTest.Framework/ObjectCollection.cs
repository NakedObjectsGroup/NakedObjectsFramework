using System;

namespace NakedFrameworkClient.SeleniumTestFramework
{

    public class ObjectCollection
    {
        //TODO local collection actions

        ObjectCollection AssertHeaderIs(string header) => throw new NotImplementedException();

        ObjectCollection AssertNoOfRowsIs(int rows) => throw new NotImplementedException();

        ObjectCollection AssertIsClosed() => throw new NotImplementedException();

        ObjectCollection ClickCloseCollection() => throw new NotImplementedException();

        ObjectCollection AssertIsOpenAsList() => throw new NotImplementedException();

        ObjectCollection ClickListIcon() => throw new NotImplementedException();

        ObjectCollection AssertIsOpenAsTable() => throw new NotImplementedException();

        ObjectCollection ClickTableIcon() => throw new NotImplementedException();

        public ObjectCollection CheckRow(int rowNumber) => throw new NotImplementedException();

        public TableRow GetRowFromTable(int number) => throw new NotImplementedException();

        public Reference GetRowFromList(int number) => throw new NotImplementedException();

    }
}