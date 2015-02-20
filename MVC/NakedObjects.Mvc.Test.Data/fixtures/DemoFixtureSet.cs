// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
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
                new ChoicesTestFixture(),
                new ClaimsTestFixture()
            };
            return list.ToArray();
        }
    }

    public class ClaimsTestFixture : AbstractClaimFixture {
        public void Install() {
            for (int i = 0; i < 200; i++) {
                var c = CreateNewClaim(EmployeeFixture.DICK, EmployeeFixture.BOB, i.ToString(), ProjectCodeFixture.CODE1, new DateTime(2007, 4, 3));
            }
        }
    }
}