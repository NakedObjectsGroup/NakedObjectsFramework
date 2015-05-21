// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims {
        [Bounded, Immutable(WhenTo.OncePersisted)]
        public class ExpenseItemStatus : Status {
            [NakedObjectsIgnore, Key]
            public int Id { get; set; }

            #region Status tests

            public bool IsNewIncomplete() {
                return TitleString.Equals(NEW_INCOMPLETE);
            }

            public bool IsNewComplete() {
                return TitleString.Equals(NEW_COMPLETE);
            }

            public bool IsApproved() {
                return TitleString.Equals(APPROVED);
            }

            public bool IsRejected() {
                return TitleString.Equals(REJECTED);
            }

            public bool IsQueried() {
                return TitleString.Equals(QUERIED);
            }

            #endregion

            #region Status definitions

            public static string APPROVED = "Approved";
            public static string NEW_COMPLETE = "New - Complete";
            public static string NEW_INCOMPLETE = "New - Incomplete";
            public static string QUERIED = "Queried";
            public static string REJECTED = "Rejected";

            #endregion
        }
    }
} //end of root namespace