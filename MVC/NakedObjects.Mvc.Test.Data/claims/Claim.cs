// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Expenses.ExpenseClaims.Items;
using Expenses.ExpenseEmployees;
using Expenses.RecordedActions;
using Expenses.Services;
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims {
        public class Claim : IRecordedActionContext {
            public IDomainObjectContainer Container { protected get; set; }

            [Hidden, Key]
            public int Id { get; set; }

            #region Title

            public virtual string Title() {
                return Description;
            }

            #endregion

            #region Injected Services

            #region Injected: ClaimRepository

            protected ClaimRepository m_claimRepository;

            public ClaimRepository ClaimRepository {
                set { m_claimRepository = value; }
            }

            #endregion

            #region Injected: UserFinder

            protected IUserFinder m_userFinder;

            public IUserFinder UserFinder {
                set { m_userFinder = value; }
            }

            #endregion

            #region Injected: RecordActionService

            private RecordActionService m_recordActionService;

            public RecordActionService RecordActionService {
                set { m_recordActionService = value; }
            }

            #endregion

            #region Injected: EmailSender

            private IEmailSender m_emailSender;

            public IEmailSender EmailSender {
                set { m_emailSender = value; }
            }

            #endregion

            #endregion

            #region Life Cycle methods

            public virtual void Created() {
                ChangeStatusToNew();
                m_dateCreated = DateTime.Now;
            }

            #endregion

            #region Description

            public static string DESCRIPTION_MAY_NOT_BE_BLANK = "Description may not be blank";
            public static string DESCRIPTION_NOT_UNIQUE = "Description is not unique";

            [MemberOrder(Sequence = "1")]
            [StringLength(100)]
            public virtual string Description { get; set; }

            public virtual void ModifyDescription(string newDescription) {
                Description = newDescription;
            }

            public virtual void ClearDescription() {
                Description = null;
            }

            [Executed(Ajax.Disabled)]
            public virtual string ValidateDescription(string testDescription) {
                if (testDescription.Equals(Description)) {
                    return null;
                }
                if (testDescription.Length == 0) {
                    return DESCRIPTION_MAY_NOT_BE_BLANK;
                }
                return Convert.ToString(((((m_claimRepository.DescriptionIsUniqueForClaimant(employee, testDescription))) ? null : DESCRIPTION_NOT_UNIQUE)));
            }

            public virtual string DisableDescription() {
                return Modifiable();
            }

            #endregion

            #region Claimant

            private Employee employee;

            [MemberOrder(Sequence = "4"), Disabled]
            public virtual Employee Claimant {
                get { return employee; }

                set { employee = value; }
            }

            #endregion

            #region DateCreated

            private DateTime m_dateCreated;

            [Disabled, Mask("dd-MMM-yy"), MemberOrder(Sequence = "2")]
            public virtual DateTime DateCreated {
                get { return m_dateCreated; }

                set { m_dateCreated = value; }
            }

            #endregion

            #region Status

            #region Status field

            [MemberOrder(Sequence = "3"), Disabled]
            public virtual ClaimStatus Status { get; set; }

            #endregion

            #region Status functions

            protected internal bool IsNew() {
                return Status.IsNew();
            }

            protected internal bool IsSubmitted() {
                return Status.IsSubmitted();
            }

            protected bool IsReturned() {
                return Status.IsReturned();
            }

            protected internal bool IsToBePaid() {
                return Status.IsToBePaid();
            }

            protected internal bool IsPaid() {
                return Status.IsPaid();
            }

            private void ChangeStatusTo(string title) {
                Status = m_claimRepository.FindClaimStatus(title);
            }

            protected internal virtual void ChangeStatusToNew() {
                ChangeStatusTo(ClaimStatus.NEW_STATUS);
            }

            protected internal virtual void ChangeStatusToSubmitted() {
                ChangeStatusTo(ClaimStatus.SUBMITTED);
            }

            protected internal virtual void ChangeStatusToReturned() {
                ChangeStatusTo(ClaimStatus.RETURNED);
            }

            protected internal virtual void ChangeStatusToToBePaid() {
                ChangeStatusTo(ClaimStatus.TO_BE_PAID);
            }

            protected internal virtual void ChangeStatusToPaid() {
                ChangeStatusTo(ClaimStatus.PAID);
            }

            protected string Modifiable() {
                if (IsNew() || IsReturned()) {
                    return null;
                }
                return "Cannot modify claim";
            }

            #endregion

            #endregion

            #region Approver

            [MemberOrder(Sequence = "5"), Optionally, FindMenu]
            public virtual Employee Approver { get; set; }

            public virtual void ModifyApprover(Employee newApprover) {
                Approver = newApprover;
            }

            public virtual void ClearApprover() {
                Approver = null;
            }

            public virtual string ValidateApprover(Employee approverToValidate) {
                return Convert.ToString(((((approverToValidate.Equals(Claimant))) ? Employee.CANT_BE_APPROVER_FOR_OWN_CLAIMS : null)));
            }

            #endregion

            #region ExpenseItems

            private IList<AbstractExpenseItem> m_expenseItems = new List<AbstractExpenseItem>();

            [Disabled]
            public virtual IList<AbstractExpenseItem> ExpenseItems {
                get { return m_expenseItems; }
                set { m_expenseItems = value; }
            }

            public virtual void AddToExpenseItems(AbstractExpenseItem item) {
                ExpenseItems.Add(item);
                RecalculateTotal();
            }

            public virtual void RemoveFromExpenseItems(AbstractExpenseItem item) {
                ExpenseItems.Remove(item);
                RecalculateTotal();
            }

            #region Calculating the Total of Expense Items

            #region Total Field

            private decimal m_total;


            [MemberOrder(Sequence = "7"), Disabled]
            public virtual decimal Total {
                get { return m_total; }

                set { m_total = value; }
            }

            [Hidden]
            public virtual void InitialiseTotal() {
                m_total = 0M;
            }

            [Hidden]
            public virtual void RecalculateTotal() {
                decimal runningTotal = 0M;
                for (int i = 0; i < ExpenseItems.Count; i++) {
                    AbstractExpenseItem item = ExpenseItems[i];
                    decimal itemAmount = item.RequestedOrApprovedAmount();

                    runningTotal = runningTotal + itemAmount;
                }
                Total = runningTotal;
            }

            #endregion

            #endregion

            #endregion

            #region Methods for adding and copying expense items

            #region Creating a new Expense Item

            [MemberOrder(Sequence = "1.1")]
            public virtual AbstractExpenseItem CreateNewExpenseItem(ExpenseType type) {
                return m_claimRepository.CreateNewExpenseItem(this, type);
            }

            public virtual string DisableCreateNewExpenseItem() {
                return Modifiable();
            }

            #endregion

            #region Copying Expense Items

            [MemberOrder(Sequence = "1.2")]
            public virtual AbstractExpenseItem CopyAnExistingExpenseItem(AbstractExpenseItem otherItem) {
                AbstractExpenseItem newItem = m_claimRepository.CreateNewExpenseItem(this, otherItem.ExpenseType);
                newItem.CopyFrom(otherItem);
                return newItem;
            }

            public virtual string DisableCopyAnExistingExpenseItem() {
                return Modifiable();
            }

            #endregion

            #region Copy All Expense Items From Another Claim

            [MemberOrder(Sequence = "1.3")]
            public virtual void CopyAllExpenseItemsFromAnotherClaim([Named("Claim or Template")] Claim otherClaim, [Optionally, Named("New date to apply to all items"), Mask("d")] DateTime newDate) {
                for (int i = 0; i < otherClaim.ExpenseItems.Count; i++) {
                    AbstractExpenseItem otherItem = (otherClaim.ExpenseItems[i]);
                    AbstractExpenseItem newItem = CopyAnExistingExpenseItem(otherItem);
                    newItem.ModifyDateIncurred(newDate);
                    Container.Persist(ref newItem);
                }
            }

            public virtual string DisableCopyAllExpenseItemsFromAnotherClaim() {
                return Modifiable();
            }

            public virtual string ValidateCopyAllExpenseItemsFromAnotherClaim(Claim otherClaim, DateTime newDate) {
                if (otherClaim == this) {
                    return "Cannot copy to same claim";
                }
                return null;
            }

            #endregion

            #endregion

            #region Project Code

            private ProjectCode m_projectCode;

            [MemberOrder(Sequence = "6"), Optionally]
            public virtual ProjectCode ProjectCode {
                get { return m_projectCode; }

                set { m_projectCode = value; }
            }

            public virtual void ModifyProjectCode(ProjectCode newCode) {
                ProjectCode = newCode;
                for (int i = 0; i < ExpenseItems.Count; i++) {
                    AbstractExpenseItem item = (ExpenseItems[i]);
                    item.ModifyProjectCode(m_projectCode);
                }
            }

            public virtual void ClearProjectCode() {
                ProjectCode = null;
            }

            public virtual string DisableProjectCode() {
                return Modifiable();
            }

            #endregion

            #region Create New Claim

            [MemberOrder(Sequence = "3.1")]
            public virtual Claim CreateNewClaimFromThis(string description, [Optionally, Named("New date to apply to all items"), Mask("d")] DateTime d) {
                Claim newClaim = (m_claimRepository.CreateNewClaim(Claimant, description));
                CopyFieldsAndItemsTo(newClaim, d);
                return newClaim;
            }

            [Executed(Where.Remotely)]
            public virtual string Default0CreateNewClaimFromThis() {
                return m_claimRepository.DefaultUniqueClaimDescription(Claimant);
            }

            private void CopyFieldsAndItemsTo(Claim newClaim, DateTime newDate) {
                newClaim.ProjectCode = ProjectCode;
                newClaim.Approver = Approver;
                newClaim.CopyAllExpenseItemsFromAnotherClaim(this, newDate);
            }

            #endregion

            #region Workflow (submit, return etc)

            #region Submit

            public static string CAN_ONLY_SUBMIT_NEW_OR_RETURNED_CLAIMS = "Can only submit a claim that is 'New' or 'Returned'";
            public static string CLAIM_AWAITING_YOUR_APPROVAL = " has submitted an expenses claim for your approval";
            public static string HAS_INCOMPLETE_ITEMS = "Cannot submit claim with incomplete Items";
            public static string ONLY_CLAIMANT_MAY_SUBMIT = "Only the Claimant may submit a Claim";

            [MemberOrder(Sequence = "4.1")]
            public virtual void Submit(Employee approver, [Named("Advise approver by email")] bool sendEmail) {
                ChangeStatusToSubmitted();
                for (int i = 0; i < ExpenseItems.Count; i++) {
                    AbstractExpenseItem item = (ExpenseItems[i]);
                    item.IsLocked = (true);
                }
                string message = Claimant.Name + CLAIM_AWAITING_YOUR_APPROVAL;
                SendEmailIfPossible(sendEmail, approver.EmailAddress, message);
                m_recordActionService.RecordMenuAction(this, "Submit", "to " + approver.Title());
            }

            private void SendEmailIfPossible(bool sendEmail, string emailAddress, string message) {
                if (sendEmail && ! (emailAddress.Equals("")) && m_emailSender != null) {
                    try {
                        m_emailSender.SendTextEmail(emailAddress, message);
                    }
                    catch (Exception e) {
                        Container.WarnUser(e.Message ?? "Unknown problem sending email");
                    }
                }
            }

            public virtual Employee Default0Submit() {
                return Approver;
            }

            public virtual bool Default1Submit() {
                return true;
            }

            public virtual string DisableSubmit() {
                if (! (AllItemsComplete())) {
                    return HAS_INCOMPLETE_ITEMS;
                }
                if (! (IsNew()) && ! (IsReturned())) {
                    return CAN_ONLY_SUBMIT_NEW_OR_RETURNED_CLAIMS;
                }
                if (! (UserIsClaimant())) {
                    return ONLY_CLAIMANT_MAY_SUBMIT;
                }
                return null;
            }

            #endregion

            #region Return to Claimant

            public static string RETURNED_CLAIM = "A previously-submitted expenses claim has been returned to you";
            public static string STATUS_NOT_SUBMITTED = "Status of claim is not 'Submitted'";

            public virtual void ReturnToClaimant([Optionally] string messageToClaimant) {
                ReturnToClaimant(messageToClaimant, true);
            }

            [Hidden]
            public virtual void ReturnToClaimant(string message, bool sendEmail) {
                ChangeStatusToReturned();
                for (int i = 0; i < ExpenseItems.Count; i++) {
                    AbstractExpenseItem item = (ExpenseItems[i]);
                    item.IsLocked = (false);
                }
                m_recordActionService.RecordMenuAction(this, "Return To Claimant", message);
                string fullMessage = ("Your Expenses Claim: " + Title() + " has been returned to you with the message " + message);
                SendEmailIfPossible(sendEmail, Claimant.EmailAddress, fullMessage);
            }

            public virtual string DisableReturnToClaimant(string message) {
                return Convert.ToString((((IsSubmitted()) ? null : STATUS_NOT_SUBMITTED)));
            }

            #endregion

            #endregion

            #region Approving claims

            #region ApproveAllItems

            [MemberOrder(Sequence = "5.1")]
            public virtual void ApproveItems([Optionally] bool approveNewItemsOnly) {
                for (int i = 0; i < ExpenseItems.Count; i++) {
                    AbstractExpenseItem item = (ExpenseItems[i]);
                    if ((! approveNewItemsOnly) || (item.NewComplete())) {
                        item.Approve();
                    }
                }
            }

            public virtual string DisableApproveItems() {
                return DisableApproverActionsOnAllItems();
            }

            #endregion

            #region RejectAllItems

            [MemberOrder(Sequence = "5.3")]
            public virtual void RejectItems(string reason, [Optionally] bool newItemsOnly) {
                for (int i = 0; i < ExpenseItems.Count; i++) {
                    AbstractExpenseItem item = (ExpenseItems[i]);
                    if ((! newItemsOnly) || (item.NewComplete())) {
                        item.Reject(reason);
                    }
                }
            }

            [Executed(Ajax.Disabled)]
            public string Validate0RejectItems(string reason) {
                if (string.IsNullOrEmpty(reason)) {
                    return "Must have reason";
                }
                return null;
            }

            [Executed(Ajax.Enabled)]
            public string Validate1RejectItems(bool newItemsOnly) {
                return null;
            }

            public virtual string DisableRejectItems() {
                return DisableApproverActionsOnAllItems();
            }

            #endregion

            #region QueryAllItems

            [MemberOrder(Sequence = "5.2")]
            public virtual void QueryItems(string reason, bool newOnly) {
                for (int i = 0; i < ExpenseItems.Count; i++) {
                    AbstractExpenseItem item = (ExpenseItems[i]);
                    if ((! newOnly) || (item.NewComplete())) {
                        item.Query(reason);
                    }
                }
            }

            public virtual string DisableQueryItems() {
                return DisableApproverActionsOnAllItems();
            }

            #endregion

            public static string APPROVER_ACTIONS_NOT_VALID_ON_NEW_CLAIM = "Approver actions only available when status is Submitted";
            public static string CANNOT_CHANGE_APPROVER_ON_SUBMITTED_CLAIM = "Cannot change the Approver on a claim that has been submitted";

            public static string USER_IS_NOT_THE_APPROVER = "User is not the specified approver for this claim";

            [Hidden]
            public string DisableApproverActionsOnAllItems() {
                if (! (IsSubmitted())) {
                    return APPROVER_ACTIONS_NOT_VALID_ON_NEW_CLAIM;
                }
                if (! (UserIsTheApproverForThisClaim())) {
                    return USER_IS_NOT_THE_APPROVER;
                }
                return null;
            }

            public string DisableApprover() {
                if (IsNew() || IsReturned()) {
                    return null;
                }
                return CANNOT_CHANGE_APPROVER_ON_SUBMITTED_CLAIM;
            }

            #endregion

            #region Support for rules

            private bool AllItemsComplete() {
                if (ExpenseItems.Count == 0) {
                    return false;
                }
                for (int i = 0; i < ExpenseItems.Count; i++) {
                    AbstractExpenseItem item = (ExpenseItems[i]);
                    if (! (item.NewComplete())) {
                        return false;
                    }
                }
                return true;
            }

            [Hidden]
            public virtual bool UserIsTheApproverForThisClaim() {
                return m_userFinder.CurrentUserAsObject().Equals(Approver);
            }

            private bool UserIsClaimant() {
                return m_userFinder.CurrentUserAsObject().Equals(Claimant);
            }

            #endregion
        }
    }
}

//end of root namespace