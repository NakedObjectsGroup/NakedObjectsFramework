// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using Expenses.Services;
using NakedObjects;
using NakedObjects.Services;

namespace Expenses.RecordedActions {
    public class RecordActionService : AbstractFactoryAndRepository {
        #region Injected Services

        #region Injected: UserFinder

        private IUserFinder m_userFinder;

        public IUserFinder UserFinder {
            set { m_userFinder = value; }
        }

        #endregion

        #endregion

        private void RecordAction(IRecordedActionContext context, string type, string action, string details) {
            var ra = NewTransientInstance<RecordedAction>();
            ra.Context = context;
            ra.Type = type;
            ra.Name = action;
            ra.Details = details;
            ra.Actor = (IActor) (m_userFinder.CurrentUserAsObject());
            ra.Date = DateTime.Now;
            Persist(ref ra);
        }

        [Hidden]
        public virtual void RecordMenuAction(IRecordedActionContext context, string action, string details) {
            RecordAction(context, RecordedAction.ACTION, action, details);
        }

        [Hidden]
        public virtual void RecordFieldChange(IRecordedActionContext context, string fieldName, object previousContents, object newContents) {
            string fromValue = previousContents == null ? "null" : previousContents.ToString();

            string toValue = newContents == null ? "null" : newContents.ToString();

            if (fromValue.Equals(toValue)) {
                return;
            }

            string details = "From: " + fromValue + " to: " + toValue;
            RecordAction(context, RecordedAction.CHANGE, fieldName, details);
        }
    }
}