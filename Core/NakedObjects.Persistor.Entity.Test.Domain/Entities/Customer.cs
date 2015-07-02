// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class Customer {
        #region Primitive Properties

        #region CustomerID (Int32)

        [MemberOrder(100)]
        public virtual int CustomerID { get; set; }

        #endregion

        #region AccountNumber (String)

        [MemberOrder(110), StringLength(10)]
        public virtual string AccountNumber { get; set; }

        #endregion

        #region CustomerType (String)

        [MemberOrder(120), StringLength(1)]
        public virtual string CustomerType { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(130)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(140), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region SalesTerritory (SalesTerritory)

        [MemberOrder(150)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #endregion

        #region CustomerAddresses (Collection of CustomerAddress)

        private ICollection<CustomerAddress> _customerAddresses = new List<CustomerAddress>();

        [MemberOrder(160), Disabled]
        public virtual ICollection<CustomerAddress> CustomerAddresses {
            get { return _customerAddresses; }
            set { _customerAddresses = value; }
        }

        #endregion

        #region Individual (Individual)

        [MemberOrder(170)]
        public virtual Individual Individual { get; set; }

        #endregion

        #region SalesOrderHeaders (Collection of SalesOrderHeader)

        private ICollection<SalesOrderHeader> _salesOrderHeaders = new List<SalesOrderHeader>();

        [MemberOrder(180), Disabled]
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders {
            get { return _salesOrderHeaders; }
            set { _salesOrderHeaders = value; }
        }

        #endregion

        #region Store (Store)

        [MemberOrder(190)]
        public virtual Store Store { get; set; }

        #endregion

        #endregion
    }
}