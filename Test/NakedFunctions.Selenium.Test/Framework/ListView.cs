using System;

namespace NakedFunctions.Selenium.Test.Framework
{
    public class ListView : ActionResult
    {
        ListView AssertHeaderIs(string header) => throw new NotImplementedException();

        ListView AssertNoOfRowsIs(int rows) => throw new NotImplementedException();

        ListView AssertIsList() => throw new NotImplementedException();

        ListView ShowAsList() => throw new NotImplementedException();

        ListView AssertIsTable() => throw new NotImplementedException();

        ListView ShowAsTable() => throw new NotImplementedException();

        ListView AssertIsPaged() => throw new NotImplementedException();

        public ListView CheckRow(int rowNumber) => throw new NotImplementedException();

        public ListRow GetRow(int number) => throw new NotImplementedException();

        ListView Next() => throw new NotImplementedException();

        ListView Previous() => throw new NotImplementedException();

        public override ListView AssertTitleIs(string title) => throw new NotImplementedException();
    }
}
