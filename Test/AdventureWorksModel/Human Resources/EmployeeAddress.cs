// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    //This class models an association table, and is never viewed directly by the user.

   public class EmployeeAddress : IAddressRole {
        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        #region Properties

        [NakedObjectsIgnore]
        public virtual int EmployeeID { get; set; }

        [NakedObjectsIgnore]
        public virtual int AddressID { get; set; }

        [NakedObjectsIgnore]
        public virtual Employee Employee { get; set; }

        [Title, Disabled, MemberOrder(2)]
        public virtual Address Address { get; set; }

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #endregion

        #endregion
    }
}