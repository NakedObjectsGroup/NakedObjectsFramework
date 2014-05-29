// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using Expenses.ExpenseEmployees;

namespace Expenses.Fixtures {
    public class SvenClaim3Returned : AbstractClaimFixture {
        public static Employee DICK;
        public static Employee SVEN;
        public static Claim SVEN_CLAIM_3;

        public  void Install() {
            SVEN = EmployeeFixture.SVEN;
            DICK = EmployeeFixture.DICK;

            SVEN_CLAIM_3 = CreateNewClaim(SVEN, DICK, "23rd Feb - Sales trip, London", ProjectCodeFixture.CODE1, new DateTime(2007, 4, 3));
            var feb23rd = new DateTime(2007, 2, 23);
            AddTaxi(SVEN_CLAIM_3, feb23rd, null, 18M, "Euston", "City", false);
            AddTaxi(SVEN_CLAIM_3, feb23rd, null, 10M, "City", "West End", false);
            GeneralExpense meal = AddMeal(SVEN_CLAIM_3, feb23rd, "Lunch with client", 50M);
            SVEN_CLAIM_3.Submit(DICK, false);
            meal.Reject("Too expensive");
            SVEN_CLAIM_3.ApproveItems(true);
            SVEN_CLAIM_3.ReturnToClaimant("Please discuss Meal item with me", false);
        }
    }
}