// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;

namespace TestData {
    [ComplexType]
    public class Address : TestHelper {
        [Root]
        [NotMapped]
        public Person Parent { protected get; set; }

        public virtual string Line1 { get; set; }
        public virtual string Line2 { get; set; }

        #region test code

        public bool HasParent {
            get { return Parent != null; }
        }

        public bool ParentIsType(Type type) {
            return type.IsAssignableFrom(Parent.GetType());
        }

        #endregion
    }
}