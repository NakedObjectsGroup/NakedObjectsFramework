// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using Expenses.ExpenseEmployees;

namespace Expenses.Fixtures {
    public class SvenClaim3Returned : AbstractClaimFixture {
        public static Employee DICK;
        public static Employee SVEN;
        public static Claim SVEN_CLAIM_3;

        public void Install() {
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