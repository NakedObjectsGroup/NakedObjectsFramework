// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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