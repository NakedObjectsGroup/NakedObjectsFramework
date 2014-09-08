// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class MostSimple {
        [Key, Title]
        public virtual int Id { get; set; }

        [ConcurrencyCheck, Hidden]
        public virtual DateTime ModifiedDate { get; set; }
    }
}