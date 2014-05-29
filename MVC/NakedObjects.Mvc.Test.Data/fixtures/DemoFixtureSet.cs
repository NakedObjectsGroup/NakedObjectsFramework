// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;

namespace Expenses.Fixtures {
    public static class DemoFixtureSet {
        public static object[] FixtureSet() {
            var list = new List<object> {
                new ExpenseTypeFixture(),
                new StatusFixture(),
                new ProjectCodeFixture(),
                new CurrencyFixture(),
                new EmployeeFixture(),
                new SvenClaim1NewStatus(),
                new SvenClaim2Submitted(),
                new SvenClaim5New(),
                new SvenClaim3Returned(),
                new SvenClaim4Approved(),
                new ChoicesTestFixture()
            };
            return list.ToArray();
        }
    }
}