// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using Expenses.ExpenseClaims;
using NakedObjects;

namespace Expenses.Fixtures {
    public class StatusFixture  {

        public IDomainObjectContainer Container { protected get; set; }

        public static ClaimStatus NEW_CLAIM;
        public static ClaimStatus PAID;
        public static ClaimStatus RETURNED;
        public static ClaimStatus SUBMITTED;

        public  void Install() {
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