// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects;

namespace ModelFirst {
    [ComplexType]
    public class ComplexType1 {
        [Root]
        public object Parent { get; set; }

        #region Primitive Properties

        public string s1 { get; set; }
        public string s2 { get; set; }

        #endregion
    }
}