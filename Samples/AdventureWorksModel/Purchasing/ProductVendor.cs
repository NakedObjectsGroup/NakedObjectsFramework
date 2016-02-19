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
    [IconName("gear.png")]
    public class ProductVendor {

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int ProductID { get; set; }

        [NakedObjectsIgnore]
        public virtual int VendorID { get; set; }

        [MemberOrder(30)]
        public virtual int AverageLeadTime { get; set; }

        [Mask("C")]
        [MemberOrder(40)]
        public virtual decimal StandardPrice { get; set; }

        [Mask("C")]
        [MemberOrder(41)]
        public virtual decimal? LastReceiptCost { get; set; }

        [Mask("d")]
        [MemberOrder(50)]
        public virtual DateTime? LastReceiptDate { get; set; }

        [MemberOrder(60)]
        public virtual int MinOrderQty { get; set; }

        [MemberOrder(61)]
        public virtual int MaxOrderQty { get; set; }

        [MemberOrder(62)]
        public virtual int? OnOrderQty { get; set; }

        [Title]
        [MemberOrder(10)]
        public virtual Product Product { get; set; }


        [NakedObjectsIgnore]
        public virtual string UnitMeasureCode { get; set; }

        [MemberOrder(20)]
        public virtual UnitMeasure UnitMeasure { get; set; }

        [NakedObjectsIgnore]
        public virtual Vendor Vendor { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion
    }
}