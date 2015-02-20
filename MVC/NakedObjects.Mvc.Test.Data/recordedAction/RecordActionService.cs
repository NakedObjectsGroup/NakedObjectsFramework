// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Expenses.Services;
using NakedObjects;
using NakedObjects.Services;

namespace Expenses.RecordedActions {
    public class RecordActionService : AbstractFactoryAndRepository {
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

        #region Injected Services

        #region Injected: UserFinder

        private IUserFinder m_userFinder;

        public IUserFinder UserFinder {
            set { m_userFinder = value; }
        }

        #endregion

        #endregion
    }
}