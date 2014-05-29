// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects;

namespace Expenses {
    namespace ExpenseClaims {
        public abstract class Status {
            [Hidden]
            public virtual string TitleString { get; set; }

            public virtual string Title() {
                return TitleString;
            }
        }
    }
} //end of root namespace