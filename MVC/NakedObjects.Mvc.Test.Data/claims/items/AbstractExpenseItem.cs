// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Expenses.ExpenseEmployees;
using Expenses.Services;
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims.Items {
        public abstract class AbstractExpenseItem {
            [Hidden, Key]
            public virtual int Id { get; set; }

            public IDomainObjectContainer Container { protected get; set; }

            #region ExpenseType

            [Hidden]
            public virtual ExpenseType ExpenseType { get; set; }

            #endregion

            #region Find similar items

            [MemberOrder(Sequence = "4")]
            public virtual IList<AbstractExpenseItem> FindSimilarExpenseItems() {
                return m_claimRepository.FindExpenseItemsLike(this);
            }

            #endregion

            #region Title & Icon

            public virtual string Title() {
                var t = new StringBuilder();
                t.Append(ExpenseType);
                return t.ToString();
            }

            public virtual string IconName() {
                return ExpenseType.IconName();
            }

            #endregion

            #region Injected Services

            #region Injected: ClaimRepository

            private ClaimRepository m_claimRepository;

            public ClaimRepository ClaimRepository {
                set { m_claimRepository = value; }
            }

            #endregion

            #region Injected: UserFinder

            private IUserFinder m_userFinder;

            public IUserFinder UserFinder {
                set { m_userFinder = value; }
            }

            #endregion

            #endregion

            #region Life Cycle methods

            public virtual void Created() {
                ChangeStatusToNewIncomplete();
            }

            public virtual void Persisted() {
                m_claim.AddToExpenseItems(this);
            }

            #endregion

            #region Project Code

            [MemberOrder(Sequence = "4")]
            public virtual ProjectCode ProjectCode { get; set; }

            [Hidden]
            public virtual void NewProjectCode(ProjectCode newCode) {
                ModifyProjectCode(newCode);
            }

            public virtual void ModifyProjectCode(ProjectCode newCode) {
                ProjectCode = (newCode);
                CheckIfComplete();
            }

            public virtual void ClearProjectCode() {
                ProjectCode = (null);
                CheckIfComplete();
            }

            public virtual string DisableProjectCode() {
                return DisabledIfLocked();
            }

            #endregion

            #region Claim (& claimant)

            private Claim m_claim;

            [Hidden]
            public virtual Claim Claim {
                get { return m_claim; }

                set { m_claim = value; }
            }

            private Employee Claimant {
                get { return Claim.Claimant; }
            }

            #endregion

            #region Amount

            public static string AMOUNT_CANNOT_BE_NEGATIVE = "Amount cannot be negative";
            public static string CURRENCY_NOT_VALID_FOR_THIS_CLAIM = "Currency not valid for this claim";
#pragma warning disable 612,618

            [MemberOrder(Sequence = "3")]
            public virtual decimal Amount { get; set; }

            public virtual void ModifyAmount(decimal newAmount) {
                // coerce currency to current currency
                Amount = newAmount;
                CheckIfComplete();
                RecalculateClaimTotalIfPersistent();
            }

            public virtual string ValidateAmount(decimal newAmount) {
                return ValidateAnyAmountField(newAmount);
            }

            public virtual string DisableAmount() {
                return DisabledIfLocked();
            }

            protected internal virtual string ValidateAnyAmountField(decimal newAmount) {
                return newAmount < 0M ? AMOUNT_CANNOT_BE_NEGATIVE : "";
            }

            [Hidden]
            public virtual void InitialiseAmount() {
                Amount = 0M;
            }

            #endregion

            #region Date Incurred

            [MemberOrder(Sequence = "1"), Mask("dd-MMM-yy")]
            public virtual DateTime? DateIncurred { get; set; }

            public virtual void ModifyDateIncurred(DateTime? newDate) {
                DateIncurred = (newDate);
                CheckIfComplete();
            }

            public virtual void ClearDateIncurred() {
                DateIncurred = (null);
                CheckIfComplete();
            }

            public virtual string DisableDateIncurred() {
                return DisabledIfLocked();
            }

            #endregion

            #region Description

            private const string DESCRIPTION_WARN = "Description cannot be empty";

            [MemberOrder(Sequence = "2")]
            public virtual string Description { get; set; }

            public virtual void ModifyDescription(string newTitle) {
                Description = (newTitle);
                CheckIfComplete();
            }

            public virtual void ClearDescription() {
                Description = (null);
                CheckIfComplete();
            }

            public virtual string ValidateDescription(string newTitle) {
                return Convert.ToString((((!(string.IsNullOrEmpty(newTitle)))) ? null : DESCRIPTION_WARN));
            }

            public virtual string DisableDescription() {
                return DisabledIfLocked();
            }

            #endregion

            #region Status

            #region Reading the status

            [Hidden]
            public bool NewIncomplete() {
                return Status.IsNewIncomplete();
            }

            [Hidden]
            public bool NewComplete() {
                return Status.IsNewComplete();
            }

            [Hidden]
            public bool Approved() {
                return Status.IsApproved();
            }

            [Hidden]
            public bool Rejected() {
                return Status.IsRejected();
            }

            [Hidden]
            public bool Queried() {
                return Status.IsQueried();
            }

            #endregion

            #region Changing the status

            private void ChangeStatusTo(string title) {
                Status = (m_claimRepository.FindExpenseItemStatus(title));
            }

            [Hidden]
            public virtual void ChangeStatusToNewIncomplete() {
                ChangeStatusTo(ExpenseItemStatus.NEW_INCOMPLETE);
            }

            [Hidden]
            public virtual void ChangeStatusToNewComplete() {
                ChangeStatusTo(ExpenseItemStatus.NEW_COMPLETE);
            }

            [Hidden]
            public virtual void ChangeStatusToApproved() {
                ChangeStatusTo(ExpenseItemStatus.APPROVED);
            }

            [Hidden]
            public virtual void ChangeStatusToRejected() {
                ChangeStatusTo(ExpenseItemStatus.REJECTED);
            }

            [Hidden]
            public virtual void ChangeStatusToQueried() {
                ChangeStatusTo(ExpenseItemStatus.QUERIED);
            }

            #endregion

            #region Status property

            [MemberOrder(Sequence = "5"), Disabled]
            public virtual ExpenseItemStatus Status { get; set; }

            #endregion

            #endregion

            #region Comment (visible only when status is Rejected or Queried)

            [Disabled]
            public virtual string Comment { get; set; }

            public virtual bool HideComment() {
                return !(Rejected() || Queried());
            }

            #endregion

            #region Controls

            protected internal virtual bool MandatoryFieldsComplete() {
                return (!(Amount == 0M) && Description != null && !(Description.Equals("")) && ProjectCode != null && MandatorySubClassFieldsComplete());
            }

            protected internal abstract bool MandatorySubClassFieldsComplete();

            protected internal virtual void CheckIfComplete() {
                if (NewComplete() && !(MandatoryFieldsComplete())) {
                    ChangeStatusToNewIncomplete();
                }
                if (NewIncomplete() && MandatoryFieldsComplete()) {
                    ChangeStatusToNewComplete();
                }
            }

            #region Locked

            private const string SUBMITTED_WARN = "Read-only : submitted";

            [Hidden]
            public virtual bool IsLocked { get; set; }

            protected internal virtual string DisabledIfLocked() {
                return Convert.ToString(((((IsLocked)) ? SUBMITTED_WARN : null)));
            }

            #endregion

            #endregion

            #region Copy From

            private const string COPY_WARN = "Cannot copy";

            [MemberOrder(Sequence = "5")]
            public virtual void CopyFrom(AbstractExpenseItem otherItem) {
                if (BelongsToSameClaim(otherItem)) {
                    if (DateIncurred == null) {
                        ModifyDateIncurred(otherItem.DateIncurred);
                    }
                }
                else if (GetType().IsInstanceOfType(otherItem)) {
                    CopyAllSameClassFields(otherItem);
                }
            }

            public virtual string DisableCopyFrom() {
                return DisabledIfLocked();
            }

            public virtual string ValidateCopyFrom(AbstractExpenseItem otherItem) {
                if (BelongsToSameClaim(otherItem) || (GetType().IsInstanceOfType(otherItem))) {
                    return null;
                }
                return COPY_WARN;
            }

            protected internal virtual void CopyAllSameClassFields(AbstractExpenseItem otherItem) {
                CopyAllFieldsFromAbstractExpenseItem(otherItem);
                CopyAnyEmptyFieldsSpecificToSubclassOfAbstractExpenseItem(otherItem);
            }

            protected internal virtual void CopyAllFieldsFromAbstractExpenseItem(AbstractExpenseItem otherItem) {
                if (Amount == 0M) {
                    // Guard against different currency

                    ModifyAmount(otherItem.Amount);
                }
                if (Description == null || Description.Equals("")) {
                    ModifyDescription(otherItem.Description);
                }
                if (ProjectCode == null) {
                    ModifyProjectCode(otherItem.ProjectCode);
                }
            }

            protected internal abstract void CopyAnyEmptyFieldsSpecificToSubclassOfAbstractExpenseItem(AbstractExpenseItem otherItem);

            protected internal virtual bool BelongsToSameClaim(AbstractExpenseItem otherItem) {
                return m_claim.Equals(otherItem.Claim);
            }

            #endregion

            #region Recalculate

            protected internal virtual void CheckIfCompleteAndRecalculateClaimTotalIfPersistent() {
                RecalculateClaimTotalIfPersistent();
                if (!(IsLocked)) {
                    CheckIfComplete();
                }
            }

            protected internal virtual void RecalculateClaimTotalIfPersistent() {
                if (Container.IsPersistent(this)) {
                    Claim.RecalculateTotal();
                }
            }

            #endregion

            #region Approvals

            public static string CANNOT_APPROVE_AN_INCOMPLETE_ITEM = "Cannot approve an incomplete item";

            [MemberOrder(Sequence = "1")]
            public virtual void Approve() {
                ChangeStatusToApproved();
                RecalculateClaimTotalIfPersistent();
            }

            public virtual string DisableApprove() {
                return DisableApproverActions();
            }

            [MemberOrder(Sequence = "3")]
            public virtual void Reject(string reason) {
                Comment = reason;
                ChangeStatusToRejected();
                RecalculateClaimTotalIfPersistent();
            }

            public virtual string DisableReject(string reason) {
                return DisableApproverActions();
            }

            [MemberOrder(Sequence = "2")]
            public virtual void Query(string reason) {
                Comment = reason;
                ChangeStatusToQueried();
                RecalculateClaimTotalIfPersistent();
            }

            public virtual string DisableQuery(string reason) {
                return DisableApproverActions();
            }

            private string DisableApproverActions() {
                if (NewIncomplete()) {
                    return CANNOT_APPROVE_AN_INCOMPLETE_ITEM;
                }
                return Claim.DisableApproverActionsOnAllItems();
            }

            [Hidden]
            public virtual decimal RequestedOrApprovedAmount() {
                if (Rejected() || Queried()) {
                    return 0M;
                }
                return Amount;
            }
#pragma warning restore 612,618

            #endregion
        }
    }
}

//end of root namespace