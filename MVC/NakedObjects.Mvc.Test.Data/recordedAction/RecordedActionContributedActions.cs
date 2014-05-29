// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects;

namespace Expenses.RecordedActions {
    /// <summary>
    ///     Defines user actions made available on objects implementing RecordedActionContext
    /// </summary>
    [Named("Recorded Actions")]
    public class RecordedActionContributedActions {
        public IDomainObjectContainer Container { protected get; set; }

        #region Injected Services

        #region Injected: RecordedActionRepository

        private RecordedActionRepository m_recordedActionRepository;

        public RecordedActionRepository RecordedActionRepository {
            set { m_recordedActionRepository = value; }
        }

        #endregion

        //
        //		* This region contains references to the services (Repositories, 
        //		* Factories or other Services) used by this domain object.  The 
        //		* references are injected by the application container.
        //		

        #endregion

        public virtual IList<RecordedAction> AllRecordedActions(IRecordedActionContext context) {
            return m_recordedActionRepository.allRecordedActions(context);
        }
    }
}