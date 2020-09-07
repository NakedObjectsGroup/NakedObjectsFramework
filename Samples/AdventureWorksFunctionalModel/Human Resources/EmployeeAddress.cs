// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel {
    //This class models an association table, and is never viewed directly by the user.

        public record EmployeeAddress : IAddressRole {



        #region Properties

        [Hidden]
        public virtual int EmployeeID { get; init; }

        [Hidden]
        public virtual int AddressID { get; init; }

        [Hidden]
        public virtual Employee Employee { get; init; }

        [MemberOrder(2)] //Title
        public virtual Address Address { get; init; }

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        #endregion

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; init; }

        #endregion

        #endregion

        #endregion
    }

    public static class EmployeeAddressFunctions
    {
        //TODO: remains to be converted
        #region Life Cycle Methods
        public static EmployeeAddress Updating(this EmployeeAddress x, [Injected] DateTime now) => x with { ModifiedDate = now };

        public static EmployeeAddress Persisting(this EmployeeAddress x, [Injected] DateTime now) => x with { ModifiedDate = now };
        #endregion
    }
}