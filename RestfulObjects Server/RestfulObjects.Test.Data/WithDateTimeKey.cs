// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class WithDateTimeKey {
        [Key, Title, Mask("d")]
        public virtual DateTime Id { get; set; }
    }
}