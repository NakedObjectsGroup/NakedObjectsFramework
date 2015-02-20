// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Expenses.ExpenseClaims;
using NakedObjects;

namespace Expenses.Fixtures {
    public class StatusFixture {
        public static ClaimStatus NEW_CLAIM;
        public static ClaimStatus PAID;
        public static ClaimStatus RETURNED;
        public static ClaimStatus SUBMITTED;
        public IDomainObjectContainer Container { protected get; set; }

        public void Install() {
            CreateExpenseItemStatus(ExpenseItemStatus.NEW_COMPLETE);
            CreateExpenseItemStatus(ExpenseItemStatus.NEW_INCOMPLETE);
            CreateExpenseItemStatus(ExpenseItemStatus.REJECTED);
            CreateExpenseItemStatus(ExpenseItemStatus.APPROVED);
            CreateExpenseItemStatus(ExpenseItemStatus.QUERIED);

            NEW_CLAIM = CreateClaimStatus(ClaimStatus.NEW_STATUS);
            SUBMITTED = CreateClaimStatus(ClaimStatus.SUBMITTED);
            RETURNED = CreateClaimStatus(ClaimStatus.RETURNED);
            PAID = CreateClaimStatus(ClaimStatus.PAID);
        }

        [Hidden]
        public virtual ExpenseItemStatus CreateExpenseItemStatus(string description) {
            var status = Container.NewTransientInstance<ExpenseItemStatus>();
            status.TitleString = description;
            Container.Persist(ref status);
            return status;
        }

        [Hidden]
        public virtual ClaimStatus CreateClaimStatus(string description) {
            var status = Container.NewTransientInstance<ClaimStatus>();
            status.TitleString = description;
            Container.Persist(ref status);
            return status;
        }
    }
}