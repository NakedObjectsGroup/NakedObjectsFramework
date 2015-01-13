// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    public partial class CustomerAddress {
        // ReSharper disable once PartialTypeWithSinglePart

        #region Primitive Properties

        #region CustomerID (Int32)

        [MemberOrder(100)]
        public virtual int CustomerID { get; set; }

        #endregion

        #region AddressID (Int32)

        [MemberOrder(110)]
        public virtual int AddressID { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(120)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(130), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Address (Address)

        [MemberOrder(140)]
        public virtual Address Address { get; set; }

        #endregion

        #region AddressType (AddressType)

        [MemberOrder(150)]
        public virtual AddressType AddressType { get; set; }

        #endregion

        #region Customer (Customer)

        [MemberOrder(160)]
        public virtual Customer Customer { get; set; }

        #endregion

        #endregion
    }
}