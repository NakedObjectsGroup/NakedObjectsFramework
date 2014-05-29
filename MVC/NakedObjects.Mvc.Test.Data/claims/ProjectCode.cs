// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Text;
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims {
        [Bounded, Immutable(WhenTo.OncePersisted)]
        public class ProjectCode {
            #region Title & Icon

            public virtual string Title() {
                var t = new StringBuilder();
                t.Append(Code).Append(" ").Append(Description);
                return t.ToString();
            }

            public virtual string IconName() {
                return "LookUp";
            }

            #endregion

            #region Code

            public virtual string Code { get; set; }

            #endregion

            #region Description

            [MultiLine(NumberOfLines = 2, Width = 10)]
            public virtual string Description { get; set; }

            #endregion
       
        }
    }
} //end of root namespace