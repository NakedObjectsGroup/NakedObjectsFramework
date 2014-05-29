// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace TestData {
    public class Product : TestHelper {
        public virtual int Id { get; set; }

        [Title]
        public virtual string Name { get; set; }

        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        public override void Persisting() {
            ModifiedDate = DateTime.Now;
            base.Persisting();
        }

        public override void Updating() {
            ModifiedDate = DateTime.Now;
            base.Updating();
        }
    }
}