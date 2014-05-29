// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;

namespace Expenses.Fixtures {
    public class SvenClaim1NewStatus : AbstractClaimFixture {
        public static Employee DICK;
        public static Employee SVEN;
        public static Claim SVEN_CLAIM_1;

        public  void Install() {
            SVEN = EmployeeFixture.SVEN;
            DICK = EmployeeFixture.DICK;

            SVEN_CLAIM_1 = CreateNewClaim(SVEN, DICK, "28th Mar - Sales call, London", ProjectCodeFixture.CODE1, new DateTime(2007, 4, 3));
            var mar28th = new DateTime(2007, 3, 28);
            AddTaxi(SVEN_CLAIM_1, mar28th, null, Convert.ToDecimal(8.5), "Euston", "Mayfair", false);
            AddMeal(SVEN_CLAIM_1, mar28th, "Lunch with client", Convert.ToDecimal(31.9));
            AddTaxi(SVEN_CLAIM_1, mar28th, null, 11M, "Mayfair", "City", false);
        }
    }
}