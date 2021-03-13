using System;

namespace NakedFrameworkClient.TestFramework
{
    public class ListView : ActionResult
    {
        public override ListView AssertTitleIs(string title) => throw new NotImplementedException();

        ListView AssertHeaderIs(string header) => throw new NotImplementedException();

        ListView AssertNoOfRowsIs(int rows) => throw new NotImplementedException();

        ListView AssertIsList() => throw new NotImplementedException();

        ListView ShowAsList() => throw new NotImplementedException();

        ListView AssertIsTable() => throw new NotImplementedException();

        ListView ShowAsTable() => throw new NotImplementedException();

        ListView AssertIsPaged() => throw new NotImplementedException();

        ListView ClickFirst() => throw new NotImplementedException();

        ListView ClickPrevious() => throw new NotImplementedException();

        ListView ClickNext() => throw new NotImplementedException();

        ListView ClickLast() => throw new NotImplementedException();

        public ListView CheckRow(int rowNumber) => throw new NotImplementedException();

        public TableRow GetRowFromTable(int number) => throw new NotImplementedException();

        public Reference GetRowFromList(int number) => throw new NotImplementedException();

    }
}
