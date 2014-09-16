// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    [Immutable]
    public class Immutable {
        private ICollection<MostSimple> aCollection = new List<MostSimple>();

        [Key, Title]
        public virtual int Id { get; set; }

        public virtual int AValue { get; set; }

        public virtual MostSimple AReference { get; set; }

        public virtual ICollection<MostSimple> ACollection {
            get { return aCollection; }
            set { aCollection = value; }
        }
    }
}