// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims {
        [Bounded, Immutable(WhenTo.OncePersisted)]
        public class ExpenseType {
            #region Title & Icon

            public override string ToString() {
                return TitleString;
            }

            public virtual string IconName() {
                return TitleString;
            }

            #endregion

            #region TitleString

            public virtual string TitleString { get; set; }

            #endregion

            #region Corresponding Class

            /// <summary> This method potentially allows each instance of ExpenseType to have the same icon as its corresponding classname.</summary>
            [Hidden]
            public virtual string CorrespondingClassName { get; set; }

            /// <summary> Converts the correspondingClassName into a system type.</summary>
            public virtual Type CorrespondingClass() {
                return Type.GetType(CorrespondingClassName);
            }

            #endregion
        }
    }
} //end of root namespace