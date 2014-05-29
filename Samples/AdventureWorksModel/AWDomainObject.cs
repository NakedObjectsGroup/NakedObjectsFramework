// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.ComponentModel;
using NakedObjects;

// Toplevel domain object

namespace AdventureWorksModel {
    public abstract class AWDomainObject {
       
        #region Injected Services

        public IDomainObjectContainer Container { protected get; set; }

        #endregion

        #region Life Cycle Methods

        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }

        #endregion

        [DisplayName("Last Modified")]
        [Hidden(WhenTo.UntilPersisted)]
        public abstract DateTime ModifiedDate { get; set; }

        // Even though the database does not record a rowguid for all classes, it does for so many of them 
        // that it is convenient to implement this at the top of the hierarchy.  Classes that don't need it
        // just won't use it, or persist it. Classes that do need it, will still have to override the 
        // implementation to be recognised by EF.
        [Hidden]
        public virtual Guid rowguid { get; set; }
    }
}