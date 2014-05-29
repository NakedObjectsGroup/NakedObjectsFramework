// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using Expenses.Currencies;
using Expenses.RecordedActions;
using Expenses.Services;
using NakedObjects;

namespace Expenses.ExpenseEmployees {
    public class Employee : IActor, IRecordedActionContext {
        #region Title & icon

        public virtual string Title() {
            return Name;
        }

        #endregion

        #region Injected Services

        #region Injected: RecordActionService

        private RecordActionService m_recordActionService;

        public RecordActionService RecordActionService {
            set { m_recordActionService = value; }
        }

        #endregion

        #region Injected: UserFinder

        private IUserFinder m_userFinder;

        public IUserFinder UserFinder {
            set { m_userFinder = value; }
        }

        #endregion

        #endregion

        #region Name

        [MemberOrder(Sequence = "1"), Disabled]
        public virtual string Name { get; set; }

        #endregion

        #region UserName field

        [Hidden]
        public virtual string UserName { get; set; }

        #endregion

        #region EmailAddress

        private object currentUser;

        [RegEx(Validation = "^[\\-\\w\\.]+@[\\-\\w\\.]+\\.[A-Za-z]+$"), MemberOrder(Sequence = "2"), Optionally]
        public virtual string EmailAddress { get; set; }

        public virtual void ModifyEmailAddress(string newEmailAddress) {
            m_recordActionService.RecordFieldChange(this, "Email Address", newEmailAddress, newEmailAddress);
            EmailAddress = newEmailAddress;
        }

        public virtual void ClearEmailAddress() {
            m_recordActionService.RecordFieldChange(this, "Email Address", EmailAddress, "EMPTY");
            EmailAddress = null;
        }

        [Executed(Where.Remotely)]
        public virtual bool HideEmailAddress() {
            return ! (EmployeeIsCurrentUser());
        }

        private bool EmployeeIsCurrentUser() {
            if (currentUser == null) {
                currentUser = m_userFinder.CurrentUserAsObject();
            }
            return currentUser.Equals(this);
        }

        #endregion

        #region Currency

        [MemberOrder(Sequence = "3"), Disabled]
        public virtual Currency Currency { get; set; }

        #endregion

        #region NormalApprover

        public static string CANT_BE_APPROVER_FOR_OWN_CLAIMS = "Can't be the approver for your own claims";
        public static string NOT_MODIFIABLE = "Not modifiable by current user";
        private Employee m_normalApprover;

        [MemberOrder(Sequence = "4"), Optionally]
        public virtual Employee NormalApprover {
            get { return m_normalApprover; }

            set { m_normalApprover = value; }
        }

        public virtual void ModifyNormalApprover(Employee newNormalApprover) {
            m_recordActionService.RecordFieldChange(this, "Normal Approver", NormalApprover, m_normalApprover);
            NormalApprover = newNormalApprover;
        }

        public virtual void ClearNormalApprover() {
            m_recordActionService.RecordFieldChange(this, "Normal Approver", NormalApprover, "EMPTY");
            NormalApprover = null;
        }

        public virtual string ValidateNormalApprover(Employee newApprover) {
            return Convert.ToString(((Equals(newApprover)) ? CANT_BE_APPROVER_FOR_OWN_CLAIMS : null));
        }

        public virtual string DisableNormalApprover() {
            return Convert.ToString(((EmployeeIsCurrentUser()) ? null : NOT_MODIFIABLE));
        }

        #endregion
    }
}