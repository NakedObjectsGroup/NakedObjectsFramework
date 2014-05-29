// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims {
        [Bounded, Immutable(WhenTo.OncePersisted)]
        public class ExpenseItemStatus : Status {
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