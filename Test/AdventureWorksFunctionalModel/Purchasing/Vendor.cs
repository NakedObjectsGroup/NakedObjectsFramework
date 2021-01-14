// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions;
using static AW.Utilities;

namespace AW.Types {
    public record Vendor : IBusinessEntity {
        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [MemberOrder(10)]
        public virtual string AccountNumber { get; init; }

        //Title
        [MemberOrder(20)]
        public virtual string Name { get; init; }

        [MemberOrder(30)]
        public virtual byte CreditRating { get; init; }

        [MemberOrder(40)]
        public virtual bool PreferredVendorStatus { get; init; }

        [MemberOrder(50)]
        public virtual bool ActiveFlag { get; init; }


        [MemberOrder(60)]
        public virtual string PurchasingWebServiceURL { get; init; }

        public virtual IQueryable<string> AutoCompletePurchasingWebServiceURL([Length(2)] string value) {
            var matchingNames = new List<string> { "http://www.store1.com", "http://www.store2.com", "http://www.store3.com" };
            return from p in matchingNames.AsQueryable() select p.Trim();
        }

        private ICollection<ProductVendor> _ProductVendor = new List<ProductVendor>();

        [Named("Product - Order Info")]
        [TableView(true)] //  Not obvious which of many possible fields should be shown here
        [AWNotCounted] //To test this capability
        public virtual ICollection<ProductVendor> Products {
            get { return _ProductVendor; }
            set { _ProductVendor = value; }
        }

        //private ICollection<VendorAddress> _VendorAddress = new List<VendorAddress>();

        //[RenderEagerly]
        //[TableView(true)] // TableView == ListView
        //public virtual ICollection<VendorAddress> Addresses {
        //    get { return _VendorAddress; }
        //    set { _VendorAddress = value; }
        //}

        //private ICollection<VendorContact> _VendorContact = new List<VendorContact>();

        //[RenderEagerly]
        //[TableView(true)] // TableView == ListView
        //public virtual ICollection<VendorContact> Contacts {
        //    get { return _VendorContact; }
        //    set { _VendorContact = value; }
        //}

        [MemberOrder(99)]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{Name}";

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(Vendor other) => ReferenceEquals(this, other);
    }
}