// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcTestApp.Tests.Helpers {
    [ComplexType]
    public class InlineTestClass {
        public virtual string SubProp1 { get; set; }
        public virtual int SubProp2 { get; set; }

        public virtual IList<string> ChoicesSubProp1(int subProp2) {
            return new[] {"1", "2"};
        }

        public virtual string ValidateSubProp2(int val) {
            return null;
        }

        public virtual void AnAction(string aValue) {}
    }
}