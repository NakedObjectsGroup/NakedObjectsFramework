// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims {
        [Bounded, Immutable(WhenTo.OncePersisted)]
        public class ClaimStatus : Status {
            #region Status tests

            [Hidden]
            public bool IsNew() {
                return TitleString.Equals(NEW_STATUS);
            }

            [Hidden]
            public bool IsSubmitted() {
                return TitleString.Equals(SUBMITTED);
            }

            [Hidden]
            public bool IsReturned() {
                return TitleString.Equals(RETURNED);
            }

            [Hidden]
            public bool IsToBePaid() {
                return TitleString.Equals(TO_BE_PAID);
            }

            [Hidden]
            public bool IsPaid() {
                return TitleString.Equals(PAID);
            }

            #endregion

            #region Status definitions

            public static string NEW_STATUS = "New";
            public static string PAID = "Paid";

            public static string RETURNED = "Returned To Claimant";
            public static string SUBMITTED = "Submitted For Approval";

            public static string TO_BE_PAID = "Ready to be paid";

            #endregion
        }
    }
} //end of root namespace