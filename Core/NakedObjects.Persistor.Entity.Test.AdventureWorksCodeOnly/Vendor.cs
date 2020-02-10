// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    [Table("Purchasing.Vendor")]
    public partial class Vendor {
        public Vendor() { }

        public int VendorID { get; set; }

        [Required]
        [StringLength(15)]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public byte CreditRating { get; set; }

        public bool PreferredVendorStatus { get; set; }

        public bool ActiveFlag { get; set; }

        [StringLength(1024)]
        public string PurchasingWebServiceURL { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<ProductVendor> ProductVendors { get; set; } = new HashSet<ProductVendor>();

        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; } = new HashSet<PurchaseOrderHeader>();

        public virtual ICollection<VendorAddress> VendorAddresses { get; set; } = new HashSet<VendorAddress>();

        public virtual ICollection<VendorContact> VendorContacts { get; set; } = new HashSet<VendorContact>();
    }
}