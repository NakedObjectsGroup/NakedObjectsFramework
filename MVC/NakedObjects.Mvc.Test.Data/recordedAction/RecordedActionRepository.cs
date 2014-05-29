// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

namespace Expenses.RecordedActions {
    public class RecordedActionRepository : AbstractFactoryAndRepository {
        // Returns the RecordedActions for a given context.  Note:  This is a rather naive implementation.  In a real
        // application, objects might eventually accumulate too many recorded actions to be retrieved in one go, so it
        // would be more appropriate to specify a method that retrieved only the most recent 10, say, plus a
        // separate method for retrieving RecordedActions that match specified parameters and/or date range.
        [Hidden]
        public IList<RecordedAction> allRecordedActions(IRecordedActionContext context) {
            IQueryable<RecordedAction> query =
                from action in Instances<RecordedAction>()
                where action.Context == context
                select action;
            return query.ToList();
        }
    }
}