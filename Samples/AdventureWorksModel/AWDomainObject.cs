// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using NakedObjects;
using System.ComponentModel.DataAnnotations.Schema;

// Toplevel domain object

namespace AdventureWorksModel {

    public interface AWDomainObject {
        //[DisplayName("Last Modified")]
        //[Hidden(WhenTo.UntilPersisted)]
        //DateTime ModifiedDate { get; set; }

        // Even though the database does not record a rowguid for all classes, it does for so many of them 
        // that it is convenient to implement this at the top of the hierarchy.  Classes that don't need it
        // just won't use it, or persist it. Classes that do need it, will still have to override the 
        // implementation to be recognised by EF.
        //[NakedObjectsIgnore]
        //Guid rowguid { get; set; }

        #region Life Cycle Methods

        //public virtual void Persisting() {
        //    rowguid = Guid.NewGuid();
        //    //ModifiedDate = DateTime.Now;
        //}

        //public virtual void Updating() {
        //    //ModifiedDate = DateTime.Now;
        //}

        #endregion
    }
}