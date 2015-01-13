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
    public partial class Address {
        #region Primitive Properties

        #region AddressID (Int32)

        [MemberOrder(100)]
        public virtual int AddressID { get; set; }

        #endregion

        #region AddressLine1 (String)

        [MemberOrder(110), StringLength(60)]
        public virtual string AddressLine1 { get; set; }

        #endregion

        #region AddressLine2 (String)

        [MemberOrder(120), Optionally, StringLength(60)]
        public virtual string AddressLine2 { get; set; }

        #endregion

        #region City (String)

        [MemberOrder(130), StringLength(30)]
        public virtual string City { get; set; }

        #endregion

        #region PostalCode (String)

        [MemberOrder(140), StringLength(15)]
        public virtual string PostalCode { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(150)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(160), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region EmployeeAddresses (Collection of EmployeeAddress)

        private ICollection<EmployeeAddress> _employeeAddresses = new List<EmployeeAddress>();

        [MemberOrder(170), Disabled]
        public virtual ICollection<EmployeeAddress> EmployeeAddresses {
            get { return _employeeAddresses; }
            set { _employeeAddresses = value; }
        }

        #endregion

        #region StateProvince (StateProvince)

        [MemberOrder(180)]
        public virtual StateProvince StateProvince { get; set; }

        #endregion

        #region CustomerAddresses (Collection of CustomerAddress)

        private ICollection<CustomerAddress> _customerAddresses = new List<CustomerAddress>();

        [MemberOrder(190), Disabled]
        public virtual ICollection<CustomerAddress> CustomerAddresses {
            get { return _customerAddresses; }
            set { _customerAddresses = value; }
        }

        #endregion

        #region SalesOrderHeaders (Collection of SalesOrderHeader)

        private ICollection<SalesOrderHeader> _salesOrderHeaders = new List<SalesOrderHeader>();

        [MemberOrder(200), Disabled]
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders {
            get { return _salesOrderHeaders; }
            set { _salesOrderHeaders = value; }
        }

        #endregion

        #region SalesOrderHeaders1 (Collection of SalesOrderHeader)

        private ICollection<SalesOrderHeader> _salesOrderHeaders1 = new List<SalesOrderHeader>();

        [MemberOrder(210), Disabled]
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders1 {
            get { return _salesOrderHeaders1; }
            set { _salesOrderHeaders1 = value; }
        }

        #endregion

        #region VendorAddresses (Collection of VendorAddress)

        private ICollection<VendorAddress> _vendorAddresses = new List<VendorAddress>();

        [MemberOrder(220), Disabled]
        public virtual ICollection<VendorAddress> VendorAddresses {
            get { return _vendorAddresses; }
            set { _vendorAddresses = value; }
        }

        #endregion

        #endregion
    }
}